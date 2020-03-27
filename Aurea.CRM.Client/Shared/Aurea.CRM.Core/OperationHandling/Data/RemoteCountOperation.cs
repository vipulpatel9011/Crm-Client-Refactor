// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteCountOperation.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Defines search operation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;
    using Newtonsoft.Json;

    /// <summary>
    /// Record count operation performed on remote server (i.e. Online)
    /// </summary>
    /// <seealso cref="RemoteSearchOperation" />
    public class RemoteCountOperation : RemoteSearchOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCountOperation"/> class.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="linkRecordIdentification">
        /// The link record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="theHandler">
        /// The handler.
        /// </param>
        public RemoteCountOperation(
            UPContainerMetaInfo containerMetaInfo,
            string recordIdentification,
            string linkRecordIdentification,
            int linkId,
            ISearchOperationHandler theHandler)
            : base(containerMetaInfo, recordIdentification, linkRecordIdentification, linkId, theHandler)
        {
            this.QueryParameters["CountOnly"] = "1";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCountOperation"/> class.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="theHandler">
        /// The handler.
        /// </param>
        public RemoteCountOperation(
            UPContainerMetaInfo containerMetaInfo,
            string recordIdentification,
            ISearchOperationHandler theHandler)
            : this(containerMetaInfo, recordIdentification, null, -1, theHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCountOperation"/> class.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="linkRecordIdentification">
        /// The _link record identification.
        /// </param>
        /// <param name="linkId">
        /// The _link identifier.
        /// </param>
        /// <param name="theHandler">
        /// The handler.
        /// </param>
        public RemoteCountOperation(
            UPContainerMetaInfo containerMetaInfo,
            string linkRecordIdentification,
            int linkId,
            ISearchOperationHandler theHandler)
            : this(containerMetaInfo, null, linkRecordIdentification, linkId, theHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCountOperation"/> class.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="theHandler">
        /// The handler.
        /// </param>
        public RemoteCountOperation(UPContainerMetaInfo containerMetaInfo, ISearchOperationHandler theHandler)
            : this(containerMetaInfo, null, null, -1, theHandler)
        {
        }

        /// <summary>
        /// Gets the request parameters.
        /// </summary>
        /// <value>
        /// The request parameters.
        /// </value>
        public override Dictionary<string, string> RequestParameters
        {
            get
            {
                var dictionary = new Dictionary<string, string>() { { "Service", "RecordData" } };

                dictionary.Append(this.QueryParameters);
                if (this.SupportsMultipleQueries)
                {
                    dictionary["QueryCount"] = $" {this.NumberOfQueries}";
                }

                return dictionary;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [skip local merge].
        /// </summary>
        /// <value>
        /// <c>true</c> if [skip local merge]; otherwise, <c>false</c>.
        /// </value>
        public override bool SkipLocalMerge => true;

        /// <summary>
        /// Adds the container meta information.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="linkRecordIdentification">
        /// The link record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public override void AddContainerMetaInfo(
            UPContainerMetaInfo containerMetaInfo,
            string recordIdentification,
            string linkRecordIdentification,
            int linkId)
        {
            if (this.SupportsMultipleQueries == false)
            {
                return;
            }

            var queryDefinitionValue = JsonConvert.SerializeObject(containerMetaInfo.QueryToObject());
            if (string.IsNullOrEmpty(queryDefinitionValue))
            {
                return;
            }

            this.ContainerMetaInfos.Add(containerMetaInfo);
            this.QueryParameters[$"QueryDef{this.NumberOfQueries}"] = queryDefinitionValue;
            if (!string.IsNullOrEmpty(linkRecordIdentification))
            {
                this.QueryParameters[$"LinkRecordIdentification%lu"] = linkRecordIdentification;
                if (linkId >= 0)
                {
                    this.QueryParameters[$"LinkId{this.NumberOfQueries}"] = $"{linkId}";
                }
            }

            if (!string.IsNullOrEmpty(recordIdentification))
            {
                this.QueryParameters[$"RecordIdentification{this.NumberOfQueries}"] = recordIdentification;
            }

            if (containerMetaInfo.MaxResults > 0)
            {
                this.QueryParameters[$"MaxResults{this.NumberOfQueries}"] = $"{containerMetaInfo.MaxResults + 1}";
            }

            this.QueryParameters[$"CountOnly{this.NumberOfQueries}"] = "1";
            this.NumberOfQueries++;
        }

        /// <summary>
        /// Handles the operation cancel.
        /// </summary>
        public override void HandleOperationCancel()
        {
            this.SearchOperationHandler = null;
        }

        /// <summary>
        /// Processes the error with remote data.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessErrorWithRemoteData(Exception error, RemoteData remoteData)
        {
            this.SearchOperationHandler?.SearchOperationDidFailWithError(this, error);
        }

        /// <summary>
        /// Processes the json response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessJsonResponse(Dictionary<string, object> json, RemoteData remoteData)
        {
            if (this.SupportsMultipleQueries)
            {
                this.ProcessMultipleQueriesResponse(json);
            }
            else
            {
                this.ProcessSingleQueryResponse(json);
            }
        }

        /// <summary>
        /// Processes the multiple queries response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        public override void ProcessMultipleQueriesResponse(Dictionary<string, object> json)
        {
            var serviceInfo = ServerSession.CurrentSession.ServiceInfoForServiceName("RecordData");
            var counts = new List<int?>(this.NumberOfQueries);
            if (serviceInfo.IsAtLeastVersion("1.1"))
            {
                var allLocalResultRows = new Dictionary<string, ICrmDataSourceRow>();
                for (var i = 0; i < this.ContainerMetaInfos.Count; i++)
                {
                    var containerMetaInfo = this.ContainerMetaInfos.ElementAt(i);
                    var localResult = containerMetaInfo.Find();
                    for (var j = 0; j < localResult.RowCount; j++)
                    {
                        var localResultRow = localResult.ResultRowAtIndex(j);
                        allLocalResultRows[localResultRow.RootRecordId] = localResultRow;
                    }
                }

                for (var i = 0; i < this.NumberOfQueries; i++)
                {
                    var serverResultAsDictionary = json[$"Count{i}"] as Dictionary<string, object>;
                    if (serverResultAsDictionary != null)
                    {
                        var result = serverResultAsDictionary.ValueOrDefault("CountRows") as List<object>;
                        if (result != null && result.Count > 0)
                        {
                            var count = int.Parse((string)result[0]);
                            counts.Add(count);
                        }
                    }
                    else
                    {
                        counts.Add(null);
                    }
                }
            }
            else
            {
                var results = this.CreateResultForMultipleQueriesResponse(json);
                for (var i = 0; i < this.NumberOfQueries; i++)
                {
                    var count = 0;
                    if (i < results.Count)
                    {
                        count = results[i].RowCount;
                    }

                    counts.Add(count);
                }
            }

            this.SearchOperationHandler?.SearchOperationDidFinishWithCounts(this, counts);
        }

        /// <summary>
        /// Processes the single query response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        public override void ProcessSingleQueryResponse(Dictionary<string, object> json)
        {
            var serviceInfo = ServerSession.CurrentSession.ServiceInfoForServiceName("RecordData");
            var count = 0;
            if (serviceInfo?.IsAtLeastVersion("1.1") ?? false)
            {
                var result = json["CountRows"] as List<object>;
                if (result != null && result.Count > 0)
                {
                    count = int.Parse((string)result[0]);
                }
            }
            else
            {
                var result = this.CreateResultForSingeQueryResponse(json);
                count = result.RowCount;
            }

            this.SearchOperationHandler?.SearchOperationDidFinishWithCount(this, count);
        }
    }
}
