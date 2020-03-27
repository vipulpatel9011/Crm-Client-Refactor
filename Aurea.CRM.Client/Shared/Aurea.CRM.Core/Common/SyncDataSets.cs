// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncDataSets.cs" company="Aurea Software Gmbh">
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
//   The Sync data set delegate interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The Sync data set delegate interface
    /// </summary>
    public interface ISyncDataSetsDelegate
    {
        /// <summary>
        /// Synchronizing the data sets did fail.
        /// </summary>
        /// <param name="sets">
        /// The sets.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void SyncDataSetsDidFail(UPSyncDataSets sets, Exception error);

        /// <summary>
        /// Synchronizing the data sets did finish.
        /// </summary>
        /// <param name="sets">
        /// The sets.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="changedRecordIdentifications">
        /// The changed record identifications.
        /// </param>
        void SyncDataSetsDidFinishSync(
            UPSyncDataSets sets,
            Dictionary<string, object> json,
            List<string> changedRecordIdentifications);

        /// <summary>
        /// Synchronizing the data sets did finish.
        /// </summary>
        /// <param name="sets">
        /// The sets.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="changedRecordIdentifications">
        /// The changed record identifications.
        /// </param>
        void SyncDataSetsDidFinishSyncWithObject(
            UPSyncDataSets sets,
            DataModelSyncDeserializer json,
            List<string> changedRecordIdentifications);
        
    }

    /// <summary>
    /// Dandles data sets
    /// </summary>
    /// <seealso cref="ISyncRequestDelegate" />
    public class UPSyncDataSets : ISyncRequestDelegate
    {
        /// <summary>
        /// The cancelled flag
        /// </summary>
        private bool cancelled;

        /// <summary>
        /// The server request
        /// </summary>
        private SyncRequestServerOperation serverRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDataSets"/> class.
        /// </summary>
        /// <param name="dataSetNames">
        /// The data set names.
        /// </param>
        /// <param name="incremental">
        /// if set to <c>true</c> [incremental].
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="crmDataStore">
        /// The CRM data store.
        /// </param>
        /// <param name="configStore">
        /// The configuration store.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public UPSyncDataSets(
            List<string> dataSetNames,
            bool incremental,
            IServerSession session,
            ICRMDataStore crmDataStore,
            IConfigurationUnitStore configStore,
            ISyncDataSetsDelegate theDelegate)
        {
            this.Delegate = theDelegate;
            this.Session = session;
            this.DataStore = crmDataStore;
            this.ConfigStore = configStore;

            var dataSetArray = dataSetNames == null
                                   ? new List<UPSyncDataSet>()
                                   : new List<UPSyncDataSet>(dataSetNames.Count);
            var dataSets = dataSetNames?.Select(dataSetName => new UPSyncDataSet(dataSetName, incremental, crmDataStore));
            if (dataSets != null && dataSets.Any())
            {
                dataSetArray.AddRange(dataSets);
            }

            this.DataSets = dataSetArray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDataSets"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="incremental">
        /// if set to <c>true</c> [incremental].
        /// </param>
        /// <param name="crmDataStore">
        /// The CRM data store.
        /// </param>
        /// <param name="configStore">
        /// The configuration store.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public UPSyncDataSets(
            IServerSession session,
            bool incremental,
            ICRMDataStore crmDataStore,
            IConfigurationUnitStore configStore,
            ISyncDataSetsDelegate theDelegate)
            : this(configStore?.AllDataSetNamesSorted(), incremental, session, crmDataStore, configStore, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDataSets"/> class.
        /// </summary>
        /// <param name="dataSetNames">
        /// The data set names.
        /// </param>
        /// <param name="linkRecordIdentification">
        /// The link record identification.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="crmDataStore">
        /// The CRM data store.
        /// </param>
        /// <param name="configStore">
        /// The configuration store.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public UPSyncDataSets(
            List<string> dataSetNames,
            string linkRecordIdentification,
            IServerSession session,
            ICRMDataStore crmDataStore,
            IConfigurationUnitStore configStore,
            ISyncDataSetsDelegate theDelegate)
        {
            var dataSetArray = dataSetNames != null
                                   ? new List<UPSyncDataSet>(dataSetNames.Count)
                                   : new List<UPSyncDataSet>();

            if (dataSetNames != null)
            {
                dataSetArray.AddRange(dataSetNames.Select(dataSetName => new UPSyncDataSet(dataSetName)));
            }

            this.DataSets = dataSetArray;
            this.Delegate = theDelegate;
            this.LinkRecordIdentification = linkRecordIdentification;
            this.Session = session;
            this.DataStore = crmDataStore;
            this.ConfigStore = configStore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDataSets"/> class.
        /// </summary>
        /// <param name="dataSetNames">
        /// The data set names.
        /// </param>
        /// <param name="linkRecordIdentification">
        /// The link record identification.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public UPSyncDataSets(
            List<string> dataSetNames,
            string linkRecordIdentification,
            ISyncDataSetsDelegate theDelegate)
            : this(
                dataSetNames,
                linkRecordIdentification,
                ServerSession.CurrentSession,
                UPCRMDataStore.DefaultStore,
                ConfigurationUnitStore.DefaultStore,
                theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDataSets"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="crmDataStore">
        /// The CRM data store.
        /// </param>
        /// <param name="configStore">
        /// The configuration store.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public UPSyncDataSets(
            IServerSession session,
            ICRMDataStore crmDataStore,
            IConfigurationUnitStore configStore,
            ISyncDataSetsDelegate theDelegate)
        {
            this.Session = session;
            this.Delegate = theDelegate;
            this.DataStore = crmDataStore;
            this.ConfigStore = configStore;
            this.ContinueRequest = true;
        }

        /// <summary>
        /// Gets the documents to synchronize.
        /// </summary>
        /// <value>
        /// The documents to synchronize.
        /// </value>
        public List<SyncDocument> DocumentsToSync { get; private set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public ISyncDataSetsDelegate Delegate { get; set; }

        /// <summary>
        /// Gets the data sets.
        /// </summary>
        /// <value>
        /// The data sets.
        /// </value>
        public List<UPSyncDataSet> DataSets { get; private set; }

        /// <summary>
        /// Gets the link record identification.
        /// </summary>
        /// <value>
        /// The link record identification.
        /// </value>
        public string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [synchronize running].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [synchronize running]; otherwise, <c>false</c>.
        /// </value>
        public bool SyncRunning { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [stop on first error].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [stop on first error]; otherwise, <c>false</c>.
        /// </value>
        public bool StopOnFirstError { get; private set; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public IServerSession Session { get; private set; }

        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        public IConfigurationUnitStore ConfigStore { get; private set; }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public ICRMDataStore DataStore { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [always perform operation].
        /// </summary>
        /// <value>
        /// <c>true</c> if [always perform operation]; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysPerformOperation { get; set; }

        /// <summary>
        /// Gets a value indicating whether [continue request].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [continue request]; otherwise, <c>false</c>.
        /// </value>
        public bool ContinueRequest { get; private set; }

        /// <summary>
        /// Gets or sets the tracking delegate.
        /// </summary>
        /// <value>
        /// The tracking delegate.
        /// </value>
        public UPSyncReport TrackingDelegate { get; set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.StartInSession(this.Session?.RemoteSession);
        }

        /// <summary>
        /// Stars the dt in session.
        /// </summary>
        /// <param name="remoteSession">
        /// The remote session.
        /// </param>
        public void StartInSession(RemoteSession remoteSession)
        {
            this.serverRequest = new SyncRequestServerOperation(this.DataSets, this)
            {
                AlwaysPerform = this.AlwaysPerformOperation,
                ContinueRequest = this.ContinueRequest,
                TrackingDelegate = this.TrackingDelegate
            };

            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                this.serverRequest.RecordIdentification = this.LinkRecordIdentification;
            }

            this.Session?.ExecuteRequestInSession(this.serverRequest, remoteSession);
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            this.cancelled = true;

            if (this.serverRequest != null)
            {
                this.serverRequest.Delegate = null;
                this.serverRequest.Cancel();
                this.serverRequest = null;
            }

            this.Delegate = null;
        }

        /// <summary>
        /// Called when [finish with response].
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        public void OnFinishWithResponse(SyncRequestServerOperation request, Dictionary<string, object> json)
        {
            this.serverRequest = null;
            if (this.cancelled)
            {
                return;
            }

            var sync = new UPSynchronization(this.DataStore, this.ConfigStore);
            var ret = sync.SyncWithDataDictionary(json);
            var recordCount = 0;
            var changedRecordIdentifications = new List<string>();
            var recordSyncArray1 = (json.ValueOrDefault("records") as JArray)?.ToObject<List<object>>();
            if (recordSyncArray1 != null)
            {
                foreach (var recordSyncObject in recordSyncArray1)
                {
                    var recordSync = (recordSyncObject as JObject)?.ToObject<Dictionary<string, object>>();

                    var dataSetName = recordSync?.ValueOrDefault("datasetName") as string;
                    if (string.IsNullOrEmpty(dataSetName))
                    {
                        continue;
                    }

                    var dataSet = this.ConfigStore.DataSetByName(dataSetName);
                    if (dataSet == null)
                    {
                        continue;
                    }

                    var rowArray = recordSync.ValueOrDefault("rows") as JArray;
                    if (rowArray == null)
                    {
                        continue;
                    }

                    recordCount += rowArray.Count;
                    foreach (JArray row in rowArray)
                    {
                        var rowinfo = row.First() as JArray;
                        var recordIdentification = StringExtensions.InfoAreaIdRecordId((string)rowinfo[0], (string)rowinfo[1]);
                        changedRecordIdentifications.Add(recordIdentification);
                    }
                }
            }

            if (ret > 0)
            {
                var errortext = $"storing records failed with errorcode #{ret}";

                // NSError.ErrorWithDomainCodeUserInfo(errortext, ret, null);
                var error = new Exception(errortext);
                this.Delegate?.SyncDataSetsDidFail(this, error);
            }
            else
            {
                var recordSyncArray = json.ValueOrDefault("records") as JArray;
                if (recordSyncArray != null)
                {
                    foreach (var recordSyncObject in recordSyncArray)
                    {
                        var recordSync = recordSyncObject?.ToObject<Dictionary<string, object>>();

                        var dataSetName = recordSync?.ValueOrDefault("datasetName") as string;
                        if (string.IsNullOrEmpty(dataSetName))
                        {
                            continue;
                        }

                        var dataSet = this.ConfigStore.DataSetByName(dataSetName);
                        if (string.IsNullOrEmpty(dataSet?.SyncDocumentFieldGroupName))
                        {
                            continue;
                        }

                        var documentManager = new DocumentManager();
                        var rowArray = recordSync.ValueOrDefault("rows") as List<object>;
                        if (rowArray == null)
                        {
                            continue;
                        }

                        foreach (List<object> row in rowArray)
                        {
                            var rowinfo = row[0] as List<object>;
                            var recordIdentification = StringExtensions.InfoAreaIdRecordId((string)rowinfo[0], (string)rowinfo[1]);
                            var syncDocument = new SyncDocument(recordIdentification, dataSet.SyncDocumentFieldGroupName, documentManager);
                            if (this.DocumentsToSync == null)
                            {
                                this.DocumentsToSync = new List<SyncDocument> { syncDocument };
                            }
                            else
                            {
                                this.DocumentsToSync.Add(syncDocument);
                            }
                        }
                    }
                }

                this.TrackingDelegate.ClientFinishedWithRecordCount(recordCount);
                this.Delegate?.SyncDataSetsDidFinishSync(this, json, changedRecordIdentifications);
            }
        }

        /// <summary>
        /// Called when [finish with response].
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        public void OnFinishWithObjectResponse(SyncRequestServerOperation request, DataModelSyncDeserializer json)
        {
            this.serverRequest = null;
            if (this.cancelled)
            {
                return;
            }

            var sync = new UPSynchronization(this.DataStore, this.ConfigStore);
            var ret = sync.SyncWithDataObject(json);
            var recordCount = 0;
            var changedRecordIdentifications = new List<string>();
            var recordSyncArray1 = json.records;
            if (recordSyncArray1 != null)
            {
                foreach (var recordSyncObject in recordSyncArray1)
                {
                    var recordSync = recordSyncObject;

                    var dataSetName = recordSyncObject.datasetName;
                    if (string.IsNullOrEmpty(dataSetName))
                    {
                        continue;
                    }

                    var dataSet = this.ConfigStore.DataSetByName(dataSetName);
                    if (dataSet == null)
                    {
                        continue;
                    }

                    var rowArray = recordSync.rows;
                    if (rowArray == null)
                    {
                        continue;
                    }

                    recordCount += rowArray.Count;
                    foreach (JArray row in rowArray)
                    {
                        var rowinfo = row.First() as JArray;
                        var recordIdentification = StringExtensions.InfoAreaIdRecordId((string)rowinfo[0], (string)rowinfo[1]);
                        changedRecordIdentifications.Add(recordIdentification);
                    }
                }
            }

            if (ret > 0)
            {
                var errortext = $"storing records failed with errorcode #{ret}";

                // NSError.ErrorWithDomainCodeUserInfo(errortext, ret, null);
                var error = new Exception(errortext);
                this.Delegate?.SyncDataSetsDidFail(this, error);
            }
            else
            {
                var recordSyncArray = json.records;
                if (recordSyncArray != null)
                {
                    foreach (var recordSyncObject in recordSyncArray)
                    {
                        //var recordSync = recordSyncObject?.ToObject<Dictionary<string, object>>();

                        var dataSetName = recordSyncObject.datasetName;
                        if (string.IsNullOrEmpty(dataSetName))
                        {
                            continue;
                        }

                        var dataSet = this.ConfigStore.DataSetByName(dataSetName);
                        if (string.IsNullOrEmpty(dataSet?.SyncDocumentFieldGroupName))
                        {
                            continue;
                        }

                        var documentManager = new DocumentManager();
                        var rowArray = recordSyncObject.rows;
                        if (rowArray == null)
                        {
                            continue;
                        }

                        foreach (JArray row in rowArray)
                        {
                            var rowinfo = row[0];
                            var recordIdentification = StringExtensions.InfoAreaIdRecordId((string)rowinfo[0], (string)rowinfo[1]);
                            var syncDocument = new SyncDocument(recordIdentification, dataSet.SyncDocumentFieldGroupName, documentManager);
                            if (this.DocumentsToSync == null)
                            {
                                this.DocumentsToSync = new List<SyncDocument> { syncDocument };
                            }
                            else
                            {
                                this.DocumentsToSync.Add(syncDocument);
                            }
                        }
                    }
                }

                this.TrackingDelegate.ClientFinishedWithRecordCount(recordCount);
                this.Delegate?.SyncDataSetsDidFinishSyncWithObject(this, json, changedRecordIdentifications);
            }
        }


        /// <summary>
        /// Called when [fai dl with error].
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnFailWithError(SyncRequestServerOperation request, Exception error)
        {
            this.serverRequest = null;
            if (this.cancelled)
            {
                return;
            }

            this.Delegate?.SyncDataSetsDidFail(this, error);
        }
    }
}
