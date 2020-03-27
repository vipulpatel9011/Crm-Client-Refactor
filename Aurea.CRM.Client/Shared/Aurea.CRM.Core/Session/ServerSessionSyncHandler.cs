// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerSessionSyncHandler.cs" company="Aurea Software Gmbh">
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
//   Server session sync handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using Aurea.CRM.Core.Common;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.ResourceHandling;
    using GalaSoft.MvvmLight.Messaging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Request Cache policy
    /// </summary>
    public enum RequestCachePolicy
    {
        /// <summary>
        /// The use protocol cache poliy
        /// </summary>
        UseProtocolCachePoliy = 0,

        /// <summary>
        /// The reload ignoring local cache data
        /// </summary>
        ReloadIgnoringLocalCacheData = 1,

        /// <summary>
        /// The reload ignoring local and remote cache data
        /// </summary>
        ReloadIgnoringLocalAndRemoteCacheData = 4,  // Unimplemented

        /// <summary>
        /// The reload ignoring cache data
        /// </summary>
        ReloadIgnoringCacheData = ReloadIgnoringLocalAndRemoteCacheData,

        /// <summary>
        /// The return cache data else load
        /// </summary>
        ReturnCacheDataElseLoad = 2,

        /// <summary>
        /// The return cache data dont load
        /// </summary>
        ReturnCacheDataDontLoad = 3,

        /// <summary>
        /// The reload revalidating cache data
        /// </summary>
        ReloadRevalidatingCacheData = 5 // Unimplemented
    }

    /// <summary>
    /// Error check option
    /// </summary>
    public enum ErrorCheckOption
    {
        /// <summary>
        /// No check
        /// </summary>
        None = 0,

        /// <summary>
        /// Is png
        /// </summary>
        IsPng = 1,

        /// <summary>
        /// Is retina
        /// </summary>
        IsRetina = 2,

        /// <summary>
        /// Is png retina
        /// </summary>
        IsPngRetina = 3
    }

    /// <summary>
    /// Server session sync handler
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Session.IRemoteSessionDelegate" />
    /// <seealso cref="ISyncRequestDelegate" />
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.IRemoteDataDelegate" />
    /// <seealso cref="Aurea.CRM.Core.Common.ISyncDataSetsDelegate" />
    public class ServerSessionSyncHandler : IRemoteSessionDelegate,
                                            ISyncRequestDelegate,
                                            IRemoteDataDelegate,
                                            ISyncDataSetsDelegate,
                                            UPOfflineStorageSyncDelegate
    {

        /// <summary>
        /// The always perform operations.
        /// </summary>
        private readonly bool alwaysPerformOperations;

        /// <summary>
        /// The all record sets at once.
        /// </summary>
        private readonly bool allRecordSetsAtOnce;

        /// <summary>
        /// The delete databases.
        /// </summary>
        private readonly bool deleteDatabases;
        private readonly SessionBackupService sessionBackupService;

        /// <summary>
        /// The record identification.
        /// </summary>
        private readonly string recordIdentification;

        /// <summary>
        /// The store state.
        /// </summary>
        private bool storeState;

        /// <summary>
        /// The all resources.
        /// </summary>
        private List<ConfigUnit> allResources;

        /// <summary>
        /// The async request child count.
        /// </summary>
        private int asyncRequestChildCount;

        /// <summary>
        /// The async request key.
        /// </summary>
        private string asyncRequestKey;

        /// <summary>
        /// The auto sync.
        /// </summary>
        private bool autoSync;

        /// <summary>
        /// The base directory path.
        /// </summary>
        private string baseDirectoryPath;

        /// <summary>
        /// The changed record identifications.
        /// </summary>
        private HashSet<string> changedRecordIdentifications;

        /// <summary>
        /// The close remote session.
        /// </summary>
        private bool closeRemoteSession;

        /// <summary>
        /// The current request child index.
        /// </summary>
        private int currentRequestChildIndex;

        /// <summary>
        /// The current sync options.
        /// </summary>
        private SyncOption currentSyncOptions;

        /// <summary>
        /// The data set names.
        /// </summary>
        private List<string> dataSetNames;

        // State variables
        // If you add variables here, do not forget to serialize them in encodeWithCoder/decodeWithCoder!

        /// <summary>
        /// The document download ur ls.
        /// </summary>
        private List<object> documentDownloadURLs;

        /// <summary>
        /// The documents to sync.
        /// </summary>
        private List<SyncDocument> documentsToSync;

        /// <summary>
        /// The instance key.
        /// </summary>
        private string instanceKey;

        private ILogger logger = SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// The meta data was synced.
        /// </summary>
        private bool metaDataWasSynced;

        /// <summary>
        /// The next data set index.
        /// </summary>
        private int nextDataSetIndex;

        /// <summary>
        /// The next resource index.
        /// </summary>
        private int nextResourceIndex;

        /// <summary>
        /// The remote session.
        /// </summary>
        private RemoteSession remoteSession;

        private RequestCachePolicy resourceCachePolicy;

        /// <summary>
        /// The resource error check option.
        /// </summary>
        private ErrorCheckOption resourceErrorCheckOption;

        private byte[] resourceErrorCheckSequence;

        /// <summary>
        /// The resource file name.
        /// </summary>
        private string resourceFileName;

        /// <summary>
        /// The resources 2 x.
        /// </summary>
        private Dictionary<string, object> resources2X;

        /// <summary>
        /// The resource server url path.
        /// </summary>
        private Uri resourceServerUrlPath;

        /// <summary>
        /// The retry with base file name.
        /// </summary>
        private bool retryWithBaseFileName;

        /// <summary>
        /// The successful.
        /// </summary>
        private bool successful;

        /// <summary>
        /// The sync data set request.
        /// </summary>
        private UPSyncDataSets syncDataSetRequest;

        /// <summary>
        /// The sync options.
        /// </summary>
        private SyncOption syncOptions;

        /// <summary>
        /// Sync report
        /// </summary>
        private UPSyncReport syncReport;

        /// <summary>
        /// The sync request.
        /// </summary>
        private SyncRequestServerOperation syncRequest;

        /// <summary>
        /// The use 2 x resources.
        /// </summary>
        private bool use2XResources;

        /// <summary>
        /// Set to true when session has been canceled
        /// </summary>
        private bool isCancelled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSessionSyncHandler"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public ServerSessionSyncHandler(ServerSession session, SyncOption options, IServerSessionSyncHandlerDelegate theDelegate)
        {
            this.Session = session;
            this.remoteSession = this.Session.RemoteSession;
            this.Delegate = theDelegate;
            this.changedRecordIdentifications = new HashSet<string>();
            this.documentDownloadURLs = new List<object>();
            this.deleteDatabases = options.HasFlag(SyncOption.DeleteDatabases);
            this.sessionBackupService = new SessionBackupService(this.Session.SessionSpecificCachesPath, SyncStatePath, Platform?.StorageProvider);

            this.logger.LogInfo("New server session sync created");

            if (this.deleteDatabases)
            {
                options &= ~SyncOption.DeleteDatabases;
            }

            this.syncOptions = options;
            if (!this.syncOptions.HasFlag(SyncOption.StartAsync))
            {
                this.syncOptions |= SyncOption.UncommittedData;
            }

            if (this.syncOptions.HasFlag(SyncOption.Configuration))
            {
                this.syncOptions |= SyncOption.Resources;
            }

            if (this.syncOptions.HasFlag(SyncOption.LoadAsync) || this.syncOptions.HasFlag(SyncOption.StartAsync)
                || this.syncOptions.HasFlag(SyncOption.FullSync) || this.syncOptions.HasFlag(SyncOption.UncommittedData))
            {
                this.alwaysPerformOperations = true;
            }

            if (this.syncOptions.HasFlag(SyncOption.FullSync))
            {
                this.storeState = true;
            }

            this.logger.LogInfo($"Server session sync options {options}");
            this.CrmStore = this.Session.CrmDataStore;
            this.ConfigStore = this.Session.ConfigUnitStore;
            if (this.deleteDatabases)
            {
                this.CloseDatabases();
                this.Session.ConfigUnitStore = new ConfigurationUnitStore(
                    this.Session.ConfigUnitStore.BaseDirectoryPath,
                    ConfigurationUnitStore.DefaultDatabaseName,
                    true);
                this.ConfigStore = this.Session.ConfigUnitStore;

                this.Session.CrmDataStore = new UPCRMDataStore(
                    this.Session.CrmDataStore.BaseDirectoryPath,
                    UPCRMDataStore.DefaultDatabaseName,
                    true,
                    this.Session.IsUpdateCrm,
                    this.ConfigStore);
                this.CrmStore = this.Session.CrmDataStore;

                this.CanCancelSync = true;
                this.deleteDatabases = false;
            }

            this.logger.LogInfo($"Can cancel sync {this.CanCancelSync}");
            this.logger.LogInfo($"Crm store to use {this.CrmStore?.DatabaseInstance?.DatabasePath}");
            this.logger.LogInfo($"Config store to use {this.ConfigStore?.BaseDirectoryPath}\\{this.ConfigStore?.DatabaseFilename}");

            this.successful = true;
            this.currentSyncOptions = 0;
            this.allRecordSetsAtOnce = false;
            this.nextDataSetIndex = 0;
            this.nextResourceIndex = 0;
            this.metaDataWasSynced = false;

            this.logger.LogInfo($"Next data set to sync index {this.nextDataSetIndex}");
            this.logger.LogInfo($"Next resource to sync index {this.nextResourceIndex}");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSessionSyncHandler"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="identification">
        /// The identification.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public ServerSessionSyncHandler(
            ServerSession session,
            string identification,
            IServerSessionSyncHandlerDelegate theDelegate)
            : this(session, SyncOption.RecordData, theDelegate)
        {
            this.recordIdentification = identification;
            this.allRecordSetsAtOnce = true;
            this.metaDataWasSynced = false;

            this.logger.LogInfo($"Sync all record sets at once {this.allRecordSetsAtOnce}");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSessionSyncHandler"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public ServerSessionSyncHandler(ServerSession session, IServerSessionSyncHandlerDelegate theDelegate)
        {
            this.Session = session;
            this.remoteSession = this.Session.RemoteSession;
            this.Delegate = theDelegate;
            this.sessionBackupService = new SessionBackupService(this.Session.SessionSpecificCachesPath, SyncStatePath, Platform?.StorageProvider);

            this.logger.LogInfo("New server session sync created");

            // re-store state
#if PORTING
            NSData data = NSData.DataWithContentsOfFile(ServerSessionSyncHandler.SyncStatePath());
            NSKeyedUnarchiver unarchiver = new NSKeyedUnarchiver(data);
            DecodeWithCoder(unarchiver);
            unarchiver.FinishDecoding();
#endif
            this.storeState = true;

            // re-open data stores
            if (this.deleteDatabases || this.CanCancelSync)
            {
                this.Session.ConfigUnitStore = new ConfigurationUnitStore(
                    this.Session.ConfigUnitStore.BaseDirectoryPath,
                    "configDataSync.sql",
                    false);
                this.ConfigStore = this.Session.ConfigUnitStore;

                this.Session.CrmDataStore = new UPCRMDataStore(
                    this.Session.CrmDataStore.BaseDirectoryPath,
                    "crmDataStoreSync.sql",
                    false,
                    this.Session.IsUpdateCrm,
                    this.ConfigStore);
                this.CrmStore = this.Session.CrmDataStore;
            }
            else
            {
                this.CrmStore = this.Session.CrmDataStore;
                this.ConfigStore = this.Session.ConfigUnitStore;
            }

            this.logger.LogInfo($"Crm store to use {this.CrmStore?.DatabaseInstance?.DatabasePath}");
            this.logger.LogInfo($"Config store to use {this.ConfigStore?.BaseDirectoryPath}\\{this.ConfigStore?.DatabaseFilename}");

            this.currentSyncOptions = 0;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can resume synchronize.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can resume synchronize; otherwise, <c>false</c>.
        /// </value>
        public static bool CanResumeSync => Platform?.StorageProvider.FileExists(SyncStatePath) ?? false;

        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>
        /// The platform.
        /// </value>
        public static IPlatformService Platform => SimpleIoc.Default.GetInstance<IPlatformService>();

        /// <summary>
        /// Gets a value indicating whether this instance can cancel synchronize.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can cancel synchronize; otherwise, <c>false</c>.
        /// </value>
        public bool CanCancelSync { get; private set; }

        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        public IConfigurationUnitStore ConfigStore { get; private set; }

        /// <summary>
        /// Gets the CRM store.
        /// </summary>
        /// <value>
        /// The CRM store.
        /// </value>
        public ICRMDataStore CrmStore { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [databases replaced].
        /// </summary>
        /// <value>
        /// <c>true</c> if [databases replaced]; otherwise, <c>false</c>.
        /// </value>
        public bool DatabasesReplaced { get; private set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IServerSessionSyncHandlerDelegate Delegate { get; set; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public ServerSession Session { get; private set; }

        /// <summary>
        /// Gets the synchronize state path.
        /// </summary>
        /// <value>
        /// The synchronize state path.
        /// </value>
        private static string SyncStatePath
            => Path.Combine(ServerSession.CurrentSession?.SessionSpecificPath ?? string.Empty, "syncState");

        /// <summary>
        /// Applies the synchronize package.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ApplySyncPackage(Dictionary<string, object> json)
        {
            var syncer = new UPSynchronization(this.CrmStore, this.ConfigStore);
            return syncer.SyncWithDataDictionary(json);
        }

        /// <summary>
        /// Cancels the current synchronization
        /// </summary>
        public void CancelCurrentSync()
        {
            this.logger.LogInfo($"Current sync was cancelled without database delete");

            if (this.CanCancelSync && this.syncDataSetRequest != null)
            {
                this.syncDataSetRequest.Cancel();
            }

            this.isCancelled = true;
            this.RestoreSavedState();
            this.RemoveSavedState();
        }

        /// <summary>
        /// Creates the record indices.
        /// </summary>
        public void CreateRecordIndices()
        {
            var logger = this.logger;

            // create indices for quicksearch
            var quickSearchStore = this.ConfigStore.StoreWithType("QuickSearch");

            logger.LogInfo("Creating indices for quicksearch.");

            var allQuickSearches = quickSearchStore?.AllUnits();
            if (allQuickSearches != null)
            {
                foreach (QuickSearch quickSearch in allQuickSearches)
                {
                    for (var currentEntry = 0; currentEntry < quickSearch.NumberOfEntries; currentEntry++)
                    {
                        var quickSearchEntry = quickSearch.EntryAtIndex(currentEntry);
                        var searchField = quickSearchEntry.CrmField;
                        this.CrmStore.CreateIndexFor(searchField, "QUICKSEARCH");
                    }
                }
            }

            // create custom indices
            var customIndicesValue = this.ConfigStore.WebConfigValueByName("CustomIndices");
            if (customIndicesValue != null)
            {
                logger.LogInfo("Creating custom indices.");
                var customIndices = this.GetCustomIndexDefinitionForString(customIndicesValue.Value);
                if (customIndices?.Count > 0)
                {
                    foreach (Dictionary<string, object> customIndex in customIndices)
                    {
                        foreach (var key in customIndex.Keys)
                        {
                            this.CrmStore.CreateIndexFor(
                                key,
                                customIndex.ValueOrDefault(key) as List<object>,
                                "CustomIndex");
                        }
                    }
                }
                else
                {
                    logger.LogError("Custom indices format error.");
                }
            }
            else
            {
                logger.LogInfo("No custom indices defined.");
            }
        }

        /// <summary>
        /// Fetches the next package.
        /// </summary>
        public void FetchNextPackage()
        {
            this.syncDataSetRequest = new UPSyncDataSets(this.Session, this.CrmStore, this.ConfigStore, this)
            {
                AlwaysPerformOperation = this.alwaysPerformOperations
            };

            this.syncDataSetRequest.StartInSession(this.remoteSession);
        }

        /// <summary>
        /// Finishes the synchronize successful.
        /// </summary>
        public void FinishSyncSuccessful()
        {
            this.logger.LogInfo($"Sync finished successfuly");

            this.RemoveSavedState();

            // Don't care for old records if more than 100 changes
            if (this.recordIdentification == null && this.changedRecordIdentifications?.Count > 100)
            {
                this.changedRecordIdentifications = null;
            }

            this.Session.ResetRecordBasedSessionVariables();
            this.Delegate?.ServerSessionSyncHandlerDidFinish(
                this,
                this.changedRecordIdentifications?.ToList(),
                this.documentsToSync);
        }

        /// <summary>
        /// Finishes the synchronize with error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void FinishSyncWithError(Exception error)
        {
            if (error != null)
            {
                this.logger.LogInfo($"Sync finished with error {error.Message}, {error.StackTrace}");
            }
            else
            {
                this.logger.LogInfo($"Sync finished with error");
            }

            this.RemoveSavedState();
            this.successful = false;
            if (error != null)
            {
                this.Delegate?.ServerSessionSyncHandlerDidFinishWithError(this, error);
            }
            else
            {
                this.Delegate?.ServerSessionSyncHandlerDidFinish(
                    this,
                    this.changedRecordIdentifications.ToList(),
                    this.documentsToSync);
            }
        }

        /// <summary>
        /// Gets the custom index definition for string.
        /// </summary>
        /// <param name="webConfigString">
        /// The web configuration string.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<object> GetCustomIndexDefinitionForString(string webConfigString)
        {
            var customIndices = new List<object>();
            if (string.IsNullOrEmpty(webConfigString))
            {
                return customIndices;
            }

            // json string eq. [{"table": "FI", "columns": [ "F2", "F5" ] }, {"table": "KP", "columns": ["F9", "F10"] }]
            var jsonIndices = webConfigString.ParseAsJson();
            if (jsonIndices != null)
            {
                if (jsonIndices is List<object>)
                {
                    var indicesArray = (List<object>)jsonIndices;
                    foreach (var idIndex in indicesArray)
                    {
                        if (!(idIndex is Dictionary<string, object>))
                        {
                            continue;
                        }

                        var indexDictionary = (Dictionary<string, object>)idIndex;
                        var table = indexDictionary.ValueOrDefault("table") as string;
                        var fields = indexDictionary.ValueOrDefault("columns") as List<object>;
                        var customIndex = new Dictionary<string, object> { [table] = fields };
                        customIndices.Add(customIndex);
                    }
                }
                else if (jsonIndices is Dictionary<string, object>)
                {
                    var indexDictionary = (Dictionary<string, object>)jsonIndices;
                    var table = indexDictionary.ValueOrDefault("table") as string;
                    var fields = indexDictionary.ValueOrDefault("columns") as List<object>;
                    var customIndex = new Dictionary<string, object> { [table] = fields };
                    customIndices.Add(customIndex);
                }
                else
                {
                    this.logger.LogError($"Webconfigparameter {jsonIndices} is not a valid json dictionary or array.");
                }
            }
            else
            {
                var indicesDefinitionArray = webConfigString.Split(';');
                if (!(indicesDefinitionArray?.Length > 0))
                {
                    return customIndices;
                }

                foreach (var indexDefinition in indicesDefinitionArray)
                {
                    var indexComponents = indexDefinition.Split(':');
                    if (indexComponents?.Length != 2)
                    {
                        continue;
                    }

                    var indexInfoAreaId = indexComponents[0];
                    var indexFields = indexComponents[1];
                    var indicesArray = indexFields.Split('+');
                    if (!(indicesArray?.Length > 0))
                    {
                        continue;
                    }

                    var customIndex = new Dictionary<string, object> { [indexInfoAreaId] = indicesArray };
                    customIndices.Add(customIndex);
                }
            }

            return customIndices;
        }

        /// <summary>
        /// Is2s the x resource.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Is2XResource(string fileName)
        {
            return this.resources2X?.ValueOrDefault(Path.GetExtension(fileName).ToLower()) != null;
        }

        /// <summary>
        /// Nexts the file resource.
        /// </summary>
        /// <param name="alternate">
        /// if set to <c>true</c> [alternate].
        /// </param>
        public void NextFileResource(bool alternate)
        {
            while (this.nextResourceIndex < this.allResources?.Count)
            {
                if (this.isCancelled)
                {
                    return;
                }

                var currentResource = (UPConfigResource)this.allResources[this.nextResourceIndex++];
                if (this.use2XResources && !alternate && this.Is2XResource(currentResource.FileName))
                {
                    this.resourceFileName = currentResource.FileName2X;
                    this.retryWithBaseFileName = true;
                }
                else
                {
                    this.resourceFileName = currentResource.FileName;
                    this.retryWithBaseFileName = false;
                }

                if (string.IsNullOrEmpty(this.resourceFileName))
                {
                    continue;
                }

                this.logger.LogInfo($"Sync resource: {this.resourceFileName} on path {this.resourceServerUrlPath}");

                Uri serverFilePath;
                var serverFilePathAsString = $"{this.resourceServerUrlPath}/{this.resourceFileName}";
                try
                {
                    serverFilePath = new Uri(serverFilePathAsString);
                }
                catch (Exception)
                {
                    this.logger.LogError($"Sync resource url:{serverFilePathAsString} is not valid");
                    continue;
                }

                var remoteData = new RemoteData(serverFilePath, this.remoteSession, this)
                {
                    Credentials = ServerSession.CurrentSession.SessionCredentials,
                    CacheResponseOnDisk = true
                };

                if (SimpleIoc.Default.GetInstance<ILogSettings>().LogNetwork)
                {
                    remoteData.NetworkLog = true;
                }

                if (this.resourceCachePolicy != 0)
                {
                    remoteData.CachePolicy = this.resourceCachePolicy;
                }

                remoteData.Load();
                return;
            }

            Messenger.Default.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidFinishResourcesSync));
            this.syncOptions &= ~SyncOption.Resources;
            this.Start();
        }

        /// <summary>
        /// Nexts the record synchronize.
        /// </summary>
        public void NextRecordSync()
        {
            this.logger.LogInfo($"Sync next data record");
            this.logger.LogInfo($"Number of datasets to sync {this.dataSetNames?.Count}, next dataset to sync {this.nextDataSetIndex}");

            if (this.dataSetNames?.Count > this.nextDataSetIndex)
            {
                var dataSetName = this.dataSetNames[this.nextDataSetIndex++];
                var dataSet = this.ConfigStore.DataSetByName(dataSetName);

                this.logger.LogInfo($"Syncing {dataSetName} dataset");

                this.syncDataSetRequest = new UPSyncDataSets(
                    new List<string> { dataSet.UnitName },
                    false,
                    this.Session,
                    this.CrmStore,
                    this.ConfigStore,
                    this)
                {
                    AlwaysPerformOperation = this.alwaysPerformOperations
                };

                this.syncReport = new UPSyncReport("Record", dataSet.UnitName);
                this.syncDataSetRequest.TrackingDelegate = this.syncReport;

                string displayText;
                if (!string.IsNullOrEmpty(dataSet.Label))
                {
                    displayText = dataSet.Label;
                }
                else
                {
                    var infoAreaConfig = this.ConfigStore.InfoAreaConfigById(dataSet.InfoAreaId);
                    if (!string.IsNullOrEmpty(infoAreaConfig?.SingularName))
                    {
                        displayText = infoAreaConfig.SingularName;
                    }
                    else
                    {
                        displayText = this.CrmStore.TableInfoForInfoArea(dataSet.InfoAreaId)?.Label;
                        if (string.IsNullOrEmpty(displayText))
                        {
                            displayText = dataSet.InfoAreaId;
                        }
                    }
                }

                string syncPlaceHolderText = LocalizedString.TextSyncPlaceHolder;

                this.logger.LogInfo($"Syncing {displayText} dataset");

                this.Delegate?.ServerSessionSyncHandlerStatusHint(
                    this,
                    syncPlaceHolderText.StringByReplacingOccurrencesOfParameterWithIndexWithString(0, displayText));

                this.Delegate?.ServerSessionSyncHandlerDidProceedToStepNumberOfSteps(
                    this,
                    this.nextDataSetIndex,
                    this.dataSetNames.Count);

                this.logger.LogInfo($"Starting sync session for {displayText} dataset");

                this.syncDataSetRequest.StartInSession(this.remoteSession);
            }
            else
            {
                if (!this.syncOptions.HasFlag(SyncOption.RecordDataIncremental)
                    && this.ConfigStore.ConfigValueIsSetDefaultValue("System.AnalyzeCacheDB", true))
                {
                    this.CrmStore.AnalyzeDB();
                }

                this.syncOptions &= ~SyncOption.RecordData & ~SyncOption.RecordDataIncremental;
                this.Start();
            }
        }

        /// <summary>
        /// Ups the offline storage did fail with error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OfflineStorageDidFailWithError(UPOfflineStorage sender, Exception error)
        {
            // for the moment - ignore upload errors
            this.syncOptions &= ~SyncOption.UncommittedData;
            this.Start();
        }

        /// <summary>
        /// Ups the offline storage did finish with result.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        public void OfflineStorageDidFinishWithResult(UPOfflineStorage sender, object result)
        {
            this.syncOptions &= ~SyncOption.UncommittedData;
            this.Start();
        }

        /// <summary>
        /// Ups the offline storage did proceed to step number of steps.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="currentStepNumber">
        /// The current step number.
        /// </param>
        /// <param name="totalStepNumber">
        /// The total step number.
        /// </param>
        public void OfflineStorageDidProceedToStepNumberOfSteps(
            UPOfflineStorage sender,
            int currentStepNumber,
            int totalStepNumber)
        {
            this.Delegate?.ServerSessionSyncHandlerDidProceedToStepNumberOfSteps(
                this,
                currentStepNumber,
                totalStepNumber);
        }

        /// <summary>
        /// Called when [fail with error].
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnFailWithError(SyncRequestServerOperation request, Exception error)
        {
            this.syncRequest = null;
            var temp = error != null ? new Dictionary<string, object> { { "error", error } } : null;
            this.FinishSyncWithError(error);
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
            this.logger.LogInfo($"Finished server operation");

            if (request != null && request.RequestParameters != null)
            {
                this.logger.LogInfo($"Finished server operation request identification {request.RecordIdentification}");

                var commaSeparatedRequestParams = string.Join(",", request.RequestParameters?.Select(k => $"{k.Key} - {k.Value}"));

                if (!string.IsNullOrEmpty(commaSeparatedRequestParams))
                {
                    this.logger.LogInfo($"Finished server operation request params {commaSeparatedRequestParams}");
                }
            }

            if (this.currentSyncOptions.HasFlag(SyncOption.StartAsync))
            {
                this.syncOptions &= ~this.currentSyncOptions;
                this.Start();
                return;
            }

            var syncReturnCode = this.ApplySyncPackage(json);
            if (syncReturnCode > 0)
            {
                this.FinishSyncWithError(new Exception("ServerSession:" + syncReturnCode));
            }

            if (this.currentSyncOptions.HasFlag(SyncOption.LoadAsync))
            {
                if (this.currentRequestChildIndex < 0)
                {
                    this.asyncRequestChildCount = (int)json.ValueOrDefault("ChildRequestCount");
                }

                if (++this.currentRequestChildIndex < this.asyncRequestChildCount)
                {
                    this.Delegate.ServerSessionSyncHandlerStatusHint(
                        this,
                        $"requesting {this.currentRequestChildIndex} of {this.asyncRequestChildCount}");
                    this.syncRequest = new SyncRequestServerOperation(
                        this.instanceKey,
                        this.asyncRequestKey,
                        this.currentRequestChildIndex,
                        this)
                    {
                        AlwaysPerform = this.alwaysPerformOperations
                    };
                    this.Session.ExecuteRequestInSession(this.syncRequest, this.remoteSession);
                    return;
                }
            }

            this.syncOptions &= ~this.currentSyncOptions;
            this.Start();
        }

        /// <summary>
        /// Called when [finish with response].
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        public void OnFinishWithObjectResponse(SyncRequestServerOperation operation, DataModelSyncDeserializer json)
        {
        }

        /// <summary>
        /// Remotes the data did fail loading with error.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void RemoteDataDidFailLoadingWithError(RemoteData remoteData, Exception error)
        {
            if (this.retryWithBaseFileName)
            {
                --this.nextResourceIndex;
                this.NextFileResource(true);
            }
            else
            {
                var state = new Dictionary<string, object>
                {
                    [Constants.KUPSyncManagerCurrentStepNumber] = this.nextResourceIndex,
                    [Constants.KUPSyncManagerTotalResources] = this.allResources?.Count ?? 0
                };
                Messenger.Default.Send(
                    SyncManagerMessage.Create(
                        SyncManagerMessageKey.DidFinishDownloadingResourceNotification, state));
                this.NextFileResource(false);
            }
        }

        /// <summary>
        /// Remotes the data did finish loading.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public void RemoteDataDidFinishLoading(RemoteData remoteData)
        {
            var check = false;
            switch (this.resourceErrorCheckOption)
            {
                case ErrorCheckOption.IsPng:
                    check = this.resourceFileName.EndsWith(".png");
                    break;
                case ErrorCheckOption.IsRetina:
                    check = this.resourceFileName.Contains("@2x");
                    break;
                case ErrorCheckOption.IsPngRetina:
                    check = this.resourceFileName.EndsWith("@2x.png");
                    break;
            }

            if (check)
            {
                // Checking if remote data contains error check sequence
                if (remoteData.Data.IndexOf(this.resourceErrorCheckSequence).Count() > 0)
                {
                    this.RemoteDataDidFailLoadingWithError(remoteData, new Exception("ras error page"));
                    return;
                }
            }

            if (remoteData != null && !string.IsNullOrEmpty(this.resourceFileName))
            {
                this.logger.LogInfo($"Remote data finished loading {this.resourceFileName}");

                var fileName = Path.Combine(this.baseDirectoryPath, this.resourceFileName);
                var storageProvider = SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider;
                storageProvider.CreateFile(fileName, remoteData?.Data);

                if (this.resourceFileName.EndsWith(".zip"))
                {
                    var zipStream = storageProvider.GetFileStreamForRead(fileName).Result;
                    using (var archive = new ZipArchive(zipStream))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            using (var stream = entry.Open())
                            {
                                var fullFileName = Path.Combine(this.baseDirectoryPath, entry.FullName);
                                using (var fileStream = storageProvider.GetFileStreamForWrite(fullFileName).Result)
                                {
                                    stream.CopyTo(fileStream);
                                }
                            }
                        }
                    }
                }
            }

            Messenger.Default.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidFinishDownloadingResourceNotification));
            this.NextFileResource(false);
        }

        /// <inheritdoc/>
        public void RemoteDataProgressChanged(ulong progress, ulong total)
        {
        }

        /// <summary>
        /// Servers the operation manager did fail server login.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        public void ServerOperationManagerDidFailServerLogin(
            ServerOperationManager serverOperationManager,
            Exception error,
            PasswordChangeResult passwordChanged)
        {
            this.FinishSyncWithError(error);
        }

        /// <summary>
        /// Servers the operation manager did fail with internet connection error.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        public void ServerOperationManagerDidFailWithInternetConnectionError(
            ServerOperationManager serverOperationManager,
            Exception error,
            PasswordChangeResult passwordChanged)
        {
            this.FinishSyncWithError(error);
        }

        /// <summary>
        /// Servers the operation manager did perform server login.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="availableLanguages">
        /// The available languages.
        /// </param>
        /// <param name="sessionAttributes">
        /// The session attributes.
        /// </param>
        /// <param name="serverInfo">
        /// The server information.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        public void ServerOperationManagerDidPerformServerLogin(
            ServerOperationManager serverOperationManager,
            List<ServerLanguage> availableLanguages,
            Dictionary<string, object> sessionAttributes,
            List<object> serverInfo,
            PasswordChangeResult passwordChanged)
        {
            // continue sync
            this.Start();
        }

        /// <summary>
        /// Servers the operation manager password change requested.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="isRequested">
        /// if set to <c>true</c> [is requested].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ServerOperationManagerPasswordChangeRequested(
            ServerOperationManager serverOperationManager,
            bool isRequested)
        {
            // this should never happen!
            return false;
        }

        /// <summary>
        /// Servers the operation manager requires language for session.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="availableLanguages">
        /// The available languages.
        /// </param>
        /// <param name="sessionAttributes">
        /// The session attributes.
        /// </param>
        /// <param name="serverInfo">
        /// The server information.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        public void ServerOperationManagerRequiresLanguageForSession(
            ServerOperationManager serverOperationManager,
            List<ServerLanguage> availableLanguages,
            Dictionary<string, object> sessionAttributes,
            List<object> serverInfo,
            PasswordChangeResult passwordChanged)
        {
            // this should never happen!
            this.FinishSyncWithError(new Exception("Sync requiresLanguageForSessionWithAvailableLanguages"));
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.logger.LogInfo($"Starting sync session processing");

            if (this.isCancelled)
            {
                this.logger.LogError("Sync session processing was cancelled");
                return;
            }

            if (!this.SaveState())
            {
                this.logger.LogError("Error dumping sync state.");
            }

            if (this.syncOptions.HasFlag(SyncOption.UseSeparateSession))
            {
                if (this.StartRemoteSession())
                {
                    return;
                }

                // could not start in separate session -> start in main session
                this.remoteSession = this.Session.RemoteSession;
            }

            if (this.syncOptions.HasFlag(SyncOption.UncommittedData))
            {
                this.logger.LogInfo($"Start uncommited data sync");
                this.StartUncommittedDataSync();
                return;
            }

            if (this.syncOptions.HasFlag(SyncOption.StartAsync))
            {
                this.logger.LogInfo($"Start full sync async");
                this.StartAsyncFullSync();
                return;
            }

            if (this.syncOptions.HasFlag(SyncOption.LoadAsync))
            {
                this.logger.LogInfo($"Start load full sync async");
                this.StartLoadAsyncFullSync();
                return;
            }

            if (this.syncOptions.HasFlag(SyncOption.Configuration) || this.syncOptions.HasFlag(SyncOption.DataModel)
                || this.syncOptions.HasFlag(SyncOption.Catalogs))
            {
                this.logger.LogInfo($"Start metadata sync");
                this.metaDataWasSynced = true;
                this.StartMetaDataSync();
                return;
            }

            if (this.metaDataWasSynced)
            {
                this.logger.LogInfo($"Metadata was synced");
                this.CreateRecordIndices();
                this.metaDataWasSynced = false;
            }

            if (this.syncOptions.HasFlag(SyncOption.CatalogsIncremental))
            {
                this.logger.LogInfo($"Start incremental catalog sync");
                this.StartIncrementalCatalogSync();
                return;
            }

            if (this.syncOptions.HasFlag(SyncOption.RecordDataIncremental))
            {
                this.logger.LogInfo($"Start incremental record data sync");
                this.StartIncrementalRecordSync();
                return;
            }

            if (this.syncOptions.HasFlag(SyncOption.RecordData))
            {
                this.logger.LogInfo($"Start record data sync");
                this.StartRecordSync();
                return;
            }

            if (this.CanCancelSync)
            {
                this.logger.LogInfo($"Can cancel current sync {this.CanCancelSync}");

                // These two method calls are currently disabled, because they're breaking the sync process.
                // this.ConfigStore = this.Session.ReplaceConfigStoreWithStore(this.ConfigStore);
                // this.CrmStore = this.Session.ReplaceCrmDataStoreWithStore(this.CrmStore);
                this.CanCancelSync = false;
                this.DatabasesReplaced = true;
                this.changedRecordIdentifications = null;
            }

            if (this.syncOptions.HasFlag(SyncOption.Resources))
            {
                this.logger.LogInfo($"Start file resource sync");
                this.StartFileResourceSync();
                return;
            }

            if (this.syncOptions.HasFlag(SyncOption.Documents))
            {
                this.logger.LogInfo($"Start document sync");
                this.StartDocumentSync();
                return;
            }

            if (this.closeRemoteSession)
            {
                this.logger.LogInfo($"Close remote session");
                this.Session.CloseRemoteSession(this.remoteSession);
            }

            this.FinishSyncSuccessful();

            this.logger.LogInfo($"Finished sync session processing");
        }

        /// <summary>
        /// Starts the asynchronous full synchronize.
        /// </summary>
        public void StartAsyncFullSync()
        {
            this.currentSyncOptions = this.syncOptions & (SyncOption.StartAsync | SyncOption.InvalidateCache);
            this.syncRequest = new SyncRequestServerOperation("demo", "Sync;Full", this.currentSyncOptions, this)
            {
                AlwaysPerform = this.alwaysPerformOperations
            };

            this.Session?.ExecuteRequestInSession(this.syncRequest, this.remoteSession);
        }

        /// <summary>
        /// Starts the document synchronize.
        /// </summary>
        public void StartDocumentSync()
        {
            this.syncOptions &= ~SyncOption.Documents;
            this.Start();
        }

        /// <summary>
        /// Starts the file resource synchronize.
        /// </summary>
        public void StartFileResourceSync()
        {
            if (this.isCancelled)
            {
                return;
            }

            this.Delegate?.ServerSessionSyncHandlerStatusHint(this, LocalizedString.TextSyncResources);

            Exception error = null;
            var serverPath = this.Session.CrmServer.ServerUrl;
            double scale = 1;
            this.use2XResources = scale == 2.0 && !this.Session.ValueIsSet("Resource.Ignore2X");
            if (this.use2XResources)
            {
                string extensionsFor2XResources = this.Session.ValueForKeyDefaultValue("Resource.2XExtensions", "png,gif,jpg,jpeg");
                var extensionArray = extensionsFor2XResources.Split(',');
                this.resources2X = new Dictionary<string, object>();
                foreach (var item in extensionArray)
                {
                    this.resources2X[item] = item;
                }
            }

            string resourceCachePolicyString = this.ConfigStore.ConfigValue("Resource.CachePolicy");
            if (!string.IsNullOrEmpty(resourceCachePolicyString))
            {
                int i;
                if (resourceCachePolicyString == "true")
                {
                    this.resourceCachePolicy = RequestCachePolicy.ReloadRevalidatingCacheData;
                }
                else if (int.TryParse(resourceCachePolicyString, out i))
                {
                    if (i > 0 && i < 6)
                    {
                        this.resourceCachePolicy = (RequestCachePolicy)i;
                    }
                }
            }

            string resourcePath = this.ConfigStore.ConfigValue("ResourcePath");
            if (string.IsNullOrEmpty(resourcePath))
            {
                resourcePath = this.ConfigStore.ConfigValue("Resource.Path");
            }

            if (resourcePath == string.Empty)
            {
                error = new Exception("resource path undefined");
            }

            if (error == null)
            {
                if (serverPath != null)
                {
                    this.resourceServerUrlPath = new Uri($"{serverPath.AbsoluteUri}/{resourcePath}");
                    if (!this.resourceServerUrlPath.IsWellFormedOriginalString())
                    {
                        error = new Exception("invalid server resource path");
                    }
                }
            }

            if (error == null)
            {
                this.allResources = this.ConfigStore.AllFileResourcesPerConfigId();
                this.nextResourceIndex = 0;

                this.baseDirectoryPath = Path.Combine(this.Session.CrmDataStore.BaseDirectoryPath, this.Session.FileStore.DefaultFolderName);

                IStorageProvider fileManager = SimpleIoc.Default.GetInstance<IStorageProvider>();
                fileManager.CreateDirectory(this.baseDirectoryPath);

                List<string> files = fileManager.ContentsOfDirectory(this.baseDirectoryPath);
                foreach (string file in files)
                {
                    fileManager.TryDelete(Path.Combine(this.baseDirectoryPath, file), out error);
                }
            }

            var errorCheckOption = Convert.ToInt32(this.ConfigStore.ConfigValueDefaultValue("Sync.ResourcePNGCheck", "2"));
            if (errorCheckOption > 0)
            {
                this.resourceErrorCheckOption = (ErrorCheckOption)errorCheckOption;

                string configResourceCheckSequence = this.ConfigStore.ConfigValueDefaultValue("Sync.ResourcePNGCheckSequence", "Error");
                if (!string.IsNullOrEmpty(configResourceCheckSequence))
                {
                    this.resourceErrorCheckSequence = Encoding.UTF8.GetBytes(configResourceCheckSequence);
                }
                else
                {
                    this.resourceErrorCheckOption = ErrorCheckOption.None;
                }
            }
            else
            {
                this.resourceErrorCheckOption = ErrorCheckOption.None;
            }

            Messenger.Default.Send(SyncManagerMessage.Create(
                SyncManagerMessageKey.DidStartResourcesSync, new Dictionary<string, object> { { "totalResources", this.allResources?.Count ?? 0 } }));

            this.NextFileResource(false);
        }

        /// <summary>
        /// Starts the incremental catalog synchronize.
        /// </summary>
        public void StartIncrementalCatalogSync()
        {
            this.autoSync = false;
            var message = LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncCatalogs, this.ConfigStore);
            if (message.IsNotLocalized())
            {
                message = LocalizedString.TextSyncCatalogs;
            }

            this.Delegate?.ServerSessionSyncHandlerStatusHint(this, message);

            this.currentSyncOptions = this.syncOptions & SyncOption.CatalogsIncremental;
            this.syncRequest = new SyncRequestServerOperation(this.currentSyncOptions, this)
            {
                AlwaysPerform = this.alwaysPerformOperations
            };

            this.syncReport = new UPSyncReport("CatalogIncremental");
            this.syncRequest.TrackingDelegate = this.syncReport;
            this.Session.ExecuteRequestInSession(this.syncRequest, this.remoteSession);
        }

        /// <summary>
        /// Starts the incremental record synchronize.
        /// </summary>
        public void StartIncrementalRecordSync()
        {
            this.syncDataSetRequest = new UPSyncDataSets(this.Session, true, this.CrmStore, this.ConfigStore, this);
            this.syncDataSetRequest.AlwaysPerformOperation = this.alwaysPerformOperations;
            this.dataSetNames = null;
            this.syncReport = new UPSyncReport("RecordIncremental");
            this.syncDataSetRequest.TrackingDelegate = this.syncReport;
            this.syncDataSetRequest.StartInSession(this.remoteSession);
        }

        /// <summary>
        /// Starts the load asynchronous full synchronize.
        /// </summary>
        public void StartLoadAsyncFullSync()
        {
            this.currentSyncOptions = this.syncOptions & SyncOption.LoadAsync;
            this.instanceKey = "demo";
            this.asyncRequestKey = "Sync;Full";
            this.currentRequestChildIndex = -1;
            this.syncRequest = new SyncRequestServerOperation(this.instanceKey, this.asyncRequestKey, this)
            {
                AlwaysPerform = this.alwaysPerformOperations
            };

            this.Session?.ExecuteRequestInSession(this.syncRequest, this.remoteSession);
        }

        /// <summary>
        /// Starts the meta data synchronize.
        /// </summary>
        public void StartMetaDataSync()
        {
            this.autoSync = false;
            var message = string.Empty;
            message = LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncCore, this.ConfigStore);
            if (message.IsNotLocalized())
            {
                message = LocalizedString.TextSyncCore;
            }

            this.Delegate?.ServerSessionSyncHandlerStatusHint(this, message);

            this.currentSyncOptions = this.syncOptions & (SyncOption.Configuration | SyncOption.DataModel | SyncOption.Catalogs);
            this.syncRequest = new SyncRequestServerOperation(this.currentSyncOptions, this)
            {
                AlwaysPerform = this.alwaysPerformOperations
            };

            this.syncReport = new UPSyncReport("MetaData");
            this.syncRequest.TrackingDelegate = this.syncReport;

            this.Session?.ExecuteRequestInSession(this.syncRequest, this.remoteSession);
        }

        /// <summary>
        /// Starts the record synchronize.
        /// </summary>
        public void StartRecordSync()
        {
            this.syncRequest = null;
            if (this.allRecordSetsAtOnce)
            {
                if (!string.IsNullOrEmpty(this.recordIdentification))
                {
                    this.syncDataSetRequest = new UPSyncDataSets(
                        this.ConfigStore.AllDataSetNamesSorted(),
                        this.recordIdentification,
                        this.Session,
                        this.CrmStore,
                        this.ConfigStore,
                        this);

                    this.syncReport = new UPSyncReport("Record", this.recordIdentification);
                }
                else
                {
                    this.syncDataSetRequest = new UPSyncDataSets(
                        this.Session,
                        false,
                        this.CrmStore,
                        this.ConfigStore,
                        this);

                    this.syncReport = new UPSyncReport("Record");
                }

                this.syncDataSetRequest.AlwaysPerformOperation = this.alwaysPerformOperations;
                this.syncDataSetRequest.TrackingDelegate = this.syncReport;
                this.syncDataSetRequest.StartInSession(this.remoteSession);
            }
            else
            {
                this.dataSetNames = this.ConfigStore.AllDataSetNamesSorted();
                this.NextRecordSync();
            }
        }

        /// <summary>
        /// Starts the remote session.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool StartRemoteSession()
        {
            this.syncOptions &= ~SyncOption.UseSeparateSession;
            this.remoteSession = this.Session.CreateRemoteSessionWithDelegate(this);
            this.closeRemoteSession = true;
            return true;
        }

        /// <summary>
        /// Starts the uncommitted data synchronize.
        /// </summary>
        public void StartUncommittedDataSync()
        {
            var offlineStorage = UPOfflineStorage.DefaultStorage;
            if (offlineStorage == null)
            {
                return;
            }
            if (offlineStorage.NumberOfUncommittedRequests() > 0)
            {
                offlineStorage.Sync(this);
            }
            else
            {
                offlineStorage.BlockOnlineRecordRequest = false;
                this.syncOptions &= ~SyncOption.UncommittedData;
                this.Start();
            }
        }

        /// <summary>
        /// Synchronizing the data sets did fail.
        /// </summary>
        /// <param name="sets">
        /// The sets.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void SyncDataSetsDidFail(UPSyncDataSets sets, Exception error)
        {
            this.FinishSyncWithError(error);
        }

        /// <summary>
        /// Synchronizes the data sets did finish synchronize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="recordIdentificationsArray">
        /// The record identifications array.
        /// </param>
        public void SyncDataSetsDidFinishSync(
            UPSyncDataSets sender,
            Dictionary<string, object> json,
            List<string> recordIdentificationsArray)
        {
            if (recordIdentificationsArray?.Count > 0)
            {
                foreach (var id in recordIdentificationsArray)
                {
                    this.changedRecordIdentifications.Add(id);
                }
            }

            if (sender.DocumentsToSync != null)
            {
                if (this.documentsToSync == null)
                {
                    this.documentsToSync = new List<SyncDocument>(sender.DocumentsToSync);
                }
                else
                {
                    this.documentsToSync.AddRange(sender.DocumentsToSync);
                }
            }

            int? remainingRecordCount = null;
            var records = json.ValueOrDefault("records") as JArray;
            if (records?.Count == 1)
            {
                var recordSet = records[0]?.ToObject<Dictionary<string, object>>();
                if (recordSet != null)
                {
                    remainingRecordCount = recordSet.ValueOrDefault("RemainingRecordCount")?.ToInt();
                }
            }

            if (remainingRecordCount.HasValue)
            {
                this.FetchNextPackage();
            }
            else
            {
                this.SaveState();
                this.NextRecordSync();
            }
        }

        /// <summary>
        /// Synchronizes the data sets did finish synchronize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="recordIdentificationsArray">
        /// The record identifications array.
        /// </param>
        public void SyncDataSetsDidFinishSyncWithObject(
            UPSyncDataSets sender,
            DataModelSyncDeserializer json,
            List<string> recordIdentificationsArray)
        {
            if (recordIdentificationsArray?.Count > 0)
            {
                foreach (var id in recordIdentificationsArray)
                {
                    this.changedRecordIdentifications.Add(id);
                }
            }

            if (sender.DocumentsToSync != null)
            {
                if (this.documentsToSync == null)
                {
                    this.documentsToSync = new List<SyncDocument>(sender.DocumentsToSync);
                }
                else
                {
                    this.documentsToSync.AddRange(sender.DocumentsToSync);
                }
            }

            int? remainingRecordCount = null;
            var records = json.records;
            if (records?.Count == 1)
            {
                var recordSet = records[0];
                if (recordSet != null)
                {
                    remainingRecordCount = recordSet.RemainingRecordCount;
                }
            }

            if (remainingRecordCount.HasValue && remainingRecordCount > 0)
            {
                this.FetchNextPackage();
            }
            else
            {
                this.SaveState();
                this.NextRecordSync();
            }
        }

        /// <summary>
        /// Saves the state.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool SaveState()
        {
            if (!this.storeState)
            {
                return true;
            }

            this.storeState = false;

            try
            {
                // Remove old backup
                this.RemoveSavedState();

                // Close dbs before taking backup to avoid copying file when any operation is running on db
                this.CloseDatabases();
                this.sessionBackupService.BackupDatabase(ConfigurationUnitStore.DefaultDatabaseName);
                this.sessionBackupService.BackupDatabase(UPCRMDataStore.DefaultDatabaseName);
                this.InitializeStores();

                this.sessionBackupService.BackupResources(this.Session.FileStore.DefaultFolderName);
            }
            catch (Exception backupException)
            {
                this.logger.LogError(backupException);
                this.RemoveSavedState();

                return false;
            }

            return true;
        }

        /// <summary>
        /// Restore saved state
        /// </summary>
        private void RestoreSavedState()
        {
            try
            {
                // Close dbs before restoring to avoid files being locked
                this.CloseDatabases();
                this.sessionBackupService.RestoreDatabase(ConfigurationUnitStore.DefaultDatabaseName);
                this.sessionBackupService.RestoreDatabase(UPCRMDataStore.DefaultDatabaseName);
                this.InitializeStores();

                this.sessionBackupService.RestoreResources(this.Session.FileStore.DefaultFolderName);

                this.RemoveSavedState();
            }
            catch (Exception backupException)
            {
                this.logger.LogError(backupException);
            }
        }

        /// <summary>
        /// Removes the state.
        /// </summary>
        private void RemoveSavedState()
        {
            try
            {
                this.sessionBackupService.RemoveBackupFolder();
            }
            catch (Exception removeStateException)
            {
                this.logger.LogError(removeStateException);
            }
        }

        /// <summary>
        /// Close crm and config databases
        /// </summary>
        private void CloseDatabases()
        {
            this.ConfigStore?.DatabaseInstance?.Close();
            this.CrmStore?.DatabaseInstance?.Close();
        }

        /// <summary>
        /// Initialize config and crm store
        /// </summary>
        private void InitializeStores()
        {
            this.Session.ConfigUnitStore = new ConfigurationUnitStore(
                this.Session.ConfigUnitStore.BaseDirectoryPath,
                ConfigurationUnitStore.DefaultDatabaseName,
                false);
            this.ConfigStore = this.Session.ConfigUnitStore;

            this.Session.CrmDataStore = new UPCRMDataStore(
                this.Session.CrmDataStore.BaseDirectoryPath,
                UPCRMDataStore.DefaultDatabaseName,
                false,
                this.Session.IsUpdateCrm,
                this.ConfigStore);
            this.CrmStore = this.Session.CrmDataStore;
        }
    }
}
