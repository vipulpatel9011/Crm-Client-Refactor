// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncRequestServerOperation.cs" company="Aurea Software Gmbh">
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
//   Sync request delegate interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Common;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Sync request delegate interface
    /// </summary>
    public interface ISyncRequestDelegate
    {
        /// <summary>
        /// Called when [fail with error].
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void OnFailWithError(SyncRequestServerOperation operation, Exception error);

        /// <summary>
        /// Called when [finish with response].
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        void OnFinishWithResponse(SyncRequestServerOperation operation, Dictionary<string, object> json);

        /// <summary>
        /// Called when [finish with response].
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        void OnFinishWithObjectResponse(SyncRequestServerOperation operation, DataModelSyncDeserializer json);
    }

    /// <summary>
    /// Flags for differet sync options
    /// </summary>
    [Flags]
    public enum SyncOption
    {
        /// <summary>
        /// The data model.
        /// </summary>
        DataModel = 1 << 0,

        /// <summary>
        /// The configuration.
        /// </summary>
        Configuration = 1 << 1,

        /// <summary>
        /// The catalogs.
        /// </summary>
        Catalogs = 1 << 2,

        /// <summary>
        /// The record data.
        /// </summary>
        RecordData = 1 << 3,

        /// <summary>
        /// The data set definition.
        /// </summary>
        DataSetDefinition = 1 << 4,

        /// <summary>
        /// The full sync.
        /// </summary>
        FullSync = 1 << 5,

        /// <summary>
        /// The delete databases.
        /// </summary>
        DeleteDatabases = 1 << 6,

        /// <summary>
        /// The invalidate cache.
        /// </summary>
        InvalidateCache = 1 << 7,

        /// <summary>
        /// The uncommitted data.
        /// </summary>
        UncommittedData = 1 << 8,

        /// <summary>
        /// The resources.
        /// </summary>
        Resources = 1 << 9,

        /// <summary>
        /// The documents.
        /// </summary>
        Documents = 1 << 10,

        /// <summary>
        /// The record data incremental.
        /// </summary>
        RecordDataIncremental = 1 << 11,

        /// <summary>
        /// The catalogs incremental.
        /// </summary>
        CatalogsIncremental = 1 << 12,

        /// <summary>
        /// The use separate session.
        /// </summary>
        UseSeparateSession = 1 << 13,

        /// <summary>
        /// The start async.
        /// </summary>
        StartAsync = 1 << 14,

        /// <summary>
        /// The load async.
        /// </summary>
        LoadAsync = 1 << 15
    }

    /// <summary>
    /// A server operation to perfrom a sync request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.JsonResponseServerOperation" />
    public class SyncRequestServerOperation : JsonResponseServerOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncRequestServerOperation"/> class.
        /// </summary>
        /// <param name="syncOptions">
        /// The synchronize options.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public SyncRequestServerOperation(SyncOption syncOptions, ISyncRequestDelegate theDelegate)
        {
            this.SyncOptions = syncOptions;
            this.Delegate = theDelegate;
            this.DataSets = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncRequestServerOperation"/> class.
        /// </summary>
        /// <param name="instanceKey">
        /// The instance key.
        /// </param>
        /// <param name="syncKey">
        /// The synchronize key.
        /// </param>
        /// <param name="syncOptions">
        /// The synchronize options.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public SyncRequestServerOperation(
            string instanceKey,
            string syncKey,
            SyncOption syncOptions,
            ISyncRequestDelegate theDelegate)
            : this(syncOptions, theDelegate)
        {
            this.SyncKey = syncKey;
            this.InstanceKey = instanceKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncRequestServerOperation"/> class.
        /// </summary>
        /// <param name="instanceKey">
        /// The instance key.
        /// </param>
        /// <param name="syncKey">
        /// The synchronize key.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public SyncRequestServerOperation(string instanceKey, string syncKey, ISyncRequestDelegate theDelegate)
            : this(instanceKey, syncKey, -1, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncRequestServerOperation"/> class.
        /// </summary>
        /// <param name="instanceKey">
        /// The instance key.
        /// </param>
        /// <param name="syncKey">
        /// The synchronize key.
        /// </param>
        /// <param name="syncKeyChildIndex">
        /// Index of the synchronize key child.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public SyncRequestServerOperation(
            string instanceKey,
            string syncKey,
            int syncKeyChildIndex,
            ISyncRequestDelegate theDelegate)
            : this(SyncOption.LoadAsync, theDelegate)
        {
            this.SyncKey = syncKey;
            this.InstanceKey = instanceKey;
            this.SyncKeyChildIndex = syncKeyChildIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncRequestServerOperation"/> class.
        /// </summary>
        /// <param name="dataSets">
        /// The data sets.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public SyncRequestServerOperation(List<UPSyncDataSet> dataSets, ISyncRequestDelegate theDelegate)
            : this(SyncOption.RecordData, theDelegate)
        {
            this.DataSets = dataSets;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [continue request].
        /// </summary>
        /// <value>
        /// <c>true</c> if [continue request]; otherwise, <c>false</c>.
        /// </value>
        public bool ContinueRequest { get; set; }

        /// <summary>
        /// Gets the data sets.
        /// </summary>
        /// <value>
        /// The data sets.
        /// </value>
        public List<UPSyncDataSet> DataSets { get; private set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public ISyncRequestDelegate Delegate { get; set; }

        /// <summary>
        /// Gets the instance key.
        /// </summary>
        /// <value>
        /// The instance key.
        /// </value>
        public string InstanceKey { get; private set; }

        /// <summary>
        /// Gets or sets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; set; }

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
                var parameterDictionary = new Dictionary<string, string> { { "Service", "Synchronization" }, };

                if (this.ContinueRequest)
                {
                    parameterDictionary["ContinueSyncRecordDataRequest"] = "true";

                    // DDLogCRequest("SyncRequest with parameters:\n%@", parameterDictionary);
                    Logger.LogDebug($"SyncRequest with parameters:{parameterDictionary.StringFormat()}", LogFlag.LogRequests);
                    return parameterDictionary;
                }

                // Commented out in xcode.... not sure why.
                //if (this.SyncOptions.HasFlag(SyncOption.StartAsync) && !string.IsNullOrEmpty(this.SyncKey))
                //{
                //    parameterDictionary["SyncFullBatch"] = "true";
                //    parameterDictionary["ResponseTarget"] = this.SyncKey;
                //    this.SyncOptions |= SyncOption.InvalidateCache;
                //}
                //else if (this.SyncOptions.HasFlag(SyncOption.LoadAsync) && !string.IsNullOrEmpty(this.SyncKey))
                //{
                //    if (this.SyncKeyChildIndex < 0)
                //    {
                //        parameterDictionary["ResponseFromTarget"] = $"{this.InstanceKey};{this.SyncKey}";
                //    }
                //    else
                //    {
                //        parameterDictionary["ResponseFromTarget"] = $"{this.InstanceKey};Child_{this.SyncKey}_{this.SyncKeyChildIndex}";
                //    }
                //}

                if (this.SyncOptions.HasFlag(SyncOption.DataModel))
                {
                    parameterDictionary["SyncDataModel"] = "true";
                    this.SyncOptions |= SyncOption.InvalidateCache;
                }

                if (this.SyncOptions.HasFlag(SyncOption.Configuration))
                {
                    parameterDictionary["SyncConfiguration"] = "true";
                    this.SyncOptions |= SyncOption.InvalidateCache;
                }

                if (this.SyncOptions.HasFlag(SyncOption.Catalogs))
                {
                    parameterDictionary["SyncCatalogs"] = "true";
                }

                if (this.SyncOptions.HasFlag(SyncOption.CatalogsIncremental))
                {
                    parameterDictionary["SyncCatalogsIncremental"] = "true";

                    var timestamp = UPCRMDataStore.DefaultStore.LastSyncOfDataset("VariableCatalogs");
                    if (!string.IsNullOrEmpty(timestamp))
                    {
                        parameterDictionary["SyncCatalogsSince"] = timestamp;
                    }
                }

                if (this.SyncOptions.HasFlag(SyncOption.RecordData))
                {
                    parameterDictionary["SyncRecordData"] = "true";
                }

                if (this.SyncOptions.HasFlag(SyncOption.DataSetDefinition))
                {
                    parameterDictionary["SyncDataSetDefinition"] = "true";
                }

                if (this.SyncOptions.HasFlag(SyncOption.InvalidateCache))
                {
                    parameterDictionary["InvalidateCache"] = "true";
                }

                if (!string.IsNullOrEmpty(this.RecordIdentification))
                {
                    parameterDictionary["RecordIdentification"] = this.RecordIdentification;
                }

                if (!string.IsNullOrEmpty(this.Since))
                {
                    parameterDictionary["Since"] = this.Since;
                }

                if (this.DataSets?.Count > 0)
                {
                    parameterDictionary["DataSetNameCount"] = $"{this.DataSets.Count}";
                    for (var i = 0; i < this.DataSets.Count; i++)
                    {
                        var dataSet = this.DataSets[i];

                        parameterDictionary[$"DataSetName{i}"] = dataSet.DataSetName;
                        if (!dataSet.FullSync && !string.IsNullOrEmpty(dataSet.Timestamp))
                        {
                            parameterDictionary[$"Since{i}"] = dataSet.Timestamp;
                        }
                    }
                }

                // DDLogCRequest("SyncRequest with parameters:\n%@", parameterDictionary);
                Logger.LogDebug($"SyncRequest with parameters: {parameterDictionary.StringFormat()}", LogFlag.LogRequests);
                return parameterDictionary;
            }
        }

        /// <summary>
        /// Gets or sets the since.
        /// </summary>
        /// <value>
        /// The since.
        /// </value>
        public string Since { get; set; } //= "20160304 032815";

        /// <summary>
        /// Gets the synchronize key.
        /// </summary>
        /// <value>
        /// The synchronize key.
        /// </value>
        public string SyncKey { get; private set; }

        /// <summary>
        /// Gets the index of the synchronize key child.
        /// </summary>
        /// <value>
        /// The index of the synchronize key child.
        /// </value>
        public int SyncKeyChildIndex { get; private set; }

        /// <summary>
        /// Gets the synchronize options.
        /// </summary>
        /// <value>
        /// The synchronize options.
        /// </value>
        public SyncOption SyncOptions { get; private set; }

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
            this.Delegate?.OnFailWithError(this, error);
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
            this.Delegate?.OnFinishWithResponse(this, json);
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
            this.Delegate?.OnFinishWithObjectResponse(this, json);
        }

        /// <summary>
        /// Processes the remote data.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessRemoteData(RemoteData remoteData)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return null;
        }
    }
}
