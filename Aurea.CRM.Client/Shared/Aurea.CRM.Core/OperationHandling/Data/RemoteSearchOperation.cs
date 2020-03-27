// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteSearchOperation.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;
    using Newtonsoft.Json;

    /// <summary>
    /// Search operation performed on remote server (i.e. Online)
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.JsonResponseServerOperation" />
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.Data.ISearchOperation" />
    public class RemoteSearchOperation : JsonResponseServerOperation, ISearchOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSearchOperation"/> class.
        /// </summary>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public RemoteSearchOperation(ISearchOperationHandler handler)
        {
            this.QueryParameters = new Dictionary<string, string>();
            this.SearchOperationHandler = handler;
            this.SupportsMultipleQueries = true;
            this.ContainerMetaInfos = new List<UPContainerMetaInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSearchOperation"/> class.
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
        /// <param name="handler">
        /// The handler.
        /// </param>
        public RemoteSearchOperation(
            UPContainerMetaInfo containerMetaInfo,
            string recordIdentification,
            string linkRecordIdentification,
            int linkId,
            ISearchOperationHandler handler)
        {
            this.ContainerMetaInfo = containerMetaInfo;
            this.QueryParameters = new Dictionary<string, string>();
            this.SearchOperationHandler = handler;

            this.QueryParameters["QueryDef"] = JsonConvert.SerializeObject(containerMetaInfo?.QueryToObject());
            if (!string.IsNullOrEmpty(linkRecordIdentification))
            {
                this.QueryParameters["LinkRecordIdentification"] = linkRecordIdentification;
                if (linkId >= 0)
                {
                    this.QueryParameters["LinkId"] = $" {linkId}";
                }
            }

            if (!string.IsNullOrEmpty(recordIdentification))
            {
                this.RecordIdentification = recordIdentification;
                this.SkipLocalMerge = true;
                this.QueryParameters["RecordIdentification"] = recordIdentification;
            }

            if (containerMetaInfo?.MaxResults > 0)
            {
                this.QueryParameters["MaxResults"] = $" {containerMetaInfo.MaxResults}"; // it was  {containerMetaInfo.MaxResults + 1}. I can't find the logic in this +1 here.
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSearchOperation"/> class.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public RemoteSearchOperation(
            UPContainerMetaInfo containerMetaInfo,
            string recordIdentification,
            ISearchOperationHandler handler)
            : this(containerMetaInfo, recordIdentification, null, -1, handler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSearchOperation"/> class.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="linkRecordIdentification">
        /// The link record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public RemoteSearchOperation(
            UPContainerMetaInfo containerMetaInfo,
            string linkRecordIdentification,
            int linkId,
            ISearchOperationHandler handler)
            : this(containerMetaInfo, null, linkRecordIdentification, linkId, handler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSearchOperation"/> class.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="theHandler">
        /// The handler.
        /// </param>
        public RemoteSearchOperation(UPContainerMetaInfo containerMetaInfo, ISearchOperationHandler theHandler)
            : this(containerMetaInfo, null, null, -1, theHandler)
        {
        }

        /// <summary>
        /// Gets a value indicating whether [always perform].
        /// </summary>
        /// <value>
        /// <c>true</c> if [always perform]; otherwise, <c>false</c>.
        /// </value>
        public override bool AlwaysPerform => true;

        /// <summary>
        /// Gets a value indicating whether [blocked by pending up synchronize].
        /// </summary>
        /// <value>
        /// <c>true</c> if [blocked by pending up synchronize]; otherwise, <c>false</c>.
        /// </value>
        public override bool BlockedByPendingUpSync => true;

        /// <summary>
        /// Gets the container meta information.
        /// </summary>
        /// <value>
        /// The container meta information.
        /// </value>
        public UPContainerMetaInfo ContainerMetaInfo { get; private set; }

        /// <summary>
        /// Gets or sets the local merge result.
        /// </summary>
        /// <value>
        /// The local merge result.
        /// </value>
        public UPCRMResult LocalMergeResult { get; set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

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
                    dictionary["QueryCount"] = $"{this.NumberOfQueries}";
                }

                return dictionary;
            }
        }

        /// <summary>
        /// Gets or sets the search operation handler.
        /// werden auch von der remoteCountOperation verwendet.
        /// </summary>
        /// <value>
        /// The search operation handler.
        /// </value>
        public ISearchOperationHandler SearchOperationHandler { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [skip local merge].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [skip local merge]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SkipLocalMerge { get; set; }

        /// <summary>
        /// Gets the container meta infos.
        /// </summary>
        /// <value>
        /// The container meta infos.
        /// </value>
        protected List<UPContainerMetaInfo> ContainerMetaInfos { get; private set; }

        /// <summary>
        /// Gets or sets the number of queries.
        /// </summary>
        /// <value>
        /// The number of queries.
        /// </value>
        protected int NumberOfQueries { get; set; }

        /// <summary>
        /// Gets the query parameters.
        /// </summary>
        /// <value>
        /// The query parameters.
        /// </value>
        protected Dictionary<string, string> QueryParameters { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [supports multiple queries].
        /// </summary>
        /// <value>
        /// <c>true</c> if [supports multiple queries]; otherwise, <c>false</c>.
        /// </value>
        protected bool SupportsMultipleQueries { get; private set; }

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
        public virtual void AddContainerMetaInfo(
            UPContainerMetaInfo containerMetaInfo,
            string recordIdentification,
            string linkRecordIdentification,
            int linkId)
        {
            if (this.SupportsMultipleQueries == false)
            {
                return;
            }

            if (containerMetaInfo == null)
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
                this.QueryParameters[$"LinkRecordIdentification{this.NumberOfQueries}"] = linkRecordIdentification;
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

            this.NumberOfQueries++;
        }

        /// <summary>
        /// Adds the container meta information.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        public virtual void AddContainerMetaInfo(UPContainerMetaInfo containerMetaInfo)
        {
            this.AddContainerMetaInfo(containerMetaInfo, null, null, -1);
        }

        /// <summary>
        /// Adds the container meta information.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        public virtual void AddContainerMetaInfo(UPContainerMetaInfo containerMetaInfo, string recordIdentification)
        {
            this.AddContainerMetaInfo(containerMetaInfo, recordIdentification, null, -1);
        }

        /// <summary>
        /// Adds the container meta information.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        /// <param name="linkRecordIdentification">
        /// The link record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public virtual void AddContainerMetaInfo(
            UPContainerMetaInfo containerMetaInfo,
            string linkRecordIdentification,
            int linkId)
        {
            this.AddContainerMetaInfo(containerMetaInfo, null, linkRecordIdentification, linkId);
        }

        /// <summary>
        /// Creates the result for multiple queries response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<UPCRMResult> CreateResultForMultipleQueriesResponse(Dictionary<string, object> json)
        {
            var allLocalResultRows = new Dictionary<string, object>();
            foreach (var containerMetaInfo in this.ContainerMetaInfos)
            {
                var localResult = containerMetaInfo.Find();
                for (var j = 0; localResult != null && j < localResult.RowCount; j++)
                {
                    var localResultRow = localResult.ResultRowAtIndex(j);
                    allLocalResultRows[localResultRow.RootRecordId] = localResultRow;
                }
            }

            var results = new List<UPCRMResult>(this.NumberOfQueries);
            var dataStore = UPCRMDataStore.DefaultStore;
            for (var i = 0; i < this.NumberOfQueries; i++)
            {
                var serverResultAsDictionary = json[$"Result{i}"];
                if (serverResultAsDictionary != null)
                {
                    var serverResult = new UPCRMResult(this.ContainerMetaInfos[i], serverResultAsDictionary);
                    dataStore.VirtualInfoAreaCache.ApplyResult(serverResult);
                    var mergedResult = new UPCRMMergedResult(this.ContainerMetaInfos[i]);
                    mergedResult.MergeServerResultWithLocalResults(serverResult, allLocalResultRows);
                    results.Add(mergedResult);
                }
                else
                {
                    results.Add(null);
                }
            }

            return results;
        }

        /// <summary>
        /// Creates the result for singe query response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMResult"/>.
        /// </returns>
        public virtual UPCRMResult CreateResultForSingeQueryResponse(Dictionary<string, object> json)
        {
            var serverResult = new UPCRMResult(this.ContainerMetaInfo, json);
            serverResult.Log();

            UPCRMDataStore.DefaultStore.VirtualInfoAreaCache.ApplyResult(serverResult);

            if (this.SkipLocalMerge || serverResult.RowCount == 0)
            {
                return serverResult;
            }

            UPCRMResult localResult;
            if (this.LocalMergeResult != null)
            {
                localResult = this.LocalMergeResult;
            }
            else if (!string.IsNullOrEmpty(this.RecordIdentification))
            {
                localResult = this.ContainerMetaInfo.ReadRecord(this.RecordIdentification.RecordId());
            }
            else
            {
                localResult = this.ContainerMetaInfo.Find();
            }

            if (localResult == null || localResult.RowCount == 0)
            {
                return serverResult;
            }

            var allLocalResultRows = new Dictionary<string, object>();
            for (var j = 0; j < localResult.RowCount; j++)
            {
                var localResultRow = localResult.ResultRowAtIndex(j);
                allLocalResultRows[localResultRow.RootRecordId] = localResultRow;
            }

            var mergedResult = new UPCRMMergedResult(this.ContainerMetaInfo);
            mergedResult.MergeServerResultWithLocalResults(serverResult, allLocalResultRows);
            return mergedResult;
        }

        /// <summary>
        /// Handles the operation cancel.
        /// </summary>
        public virtual void HandleOperationCancel()
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
            if (this.LocalMergeResult != null && error.IsConnectionOfflineError() && !this.SupportsMultipleQueries)
            {
                this.SearchOperationHandler.SearchOperationDidFinishWithResult(this, this.LocalMergeResult);
                return;
            }

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
        /// Processes the json response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessJsonSyncObject(DataModelSyncDeserializer json, RemoteData remoteData)
        {
            //this.Delegate?.OnFinishWithObjectResponse(this, json);
        }

        /// <summary>
        /// Processes the multiple queries response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        public virtual void ProcessMultipleQueriesResponse(Dictionary<string, object> json)
        {
            var results = this.CreateResultForMultipleQueriesResponse(json);
            this.SearchOperationHandler.SearchOperationDidFinishWithResults(this, results);
        }

        /// <summary>
        /// Processes the remote data.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessRemoteData(RemoteData remoteData)
        {
        }

        /// <summary>
        /// Processes the single query response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        public virtual void ProcessSingleQueryResponse(Dictionary<string, object> json)
        {
            var mergedResult = this.CreateResultForSingeQueryResponse(json);
            this.SearchOperationHandler?.SearchOperationDidFinishWithResult(this, mergedResult);
        }
    }
}
