// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncManager.cs" company="Aurea Software Gmbh">
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
//   The constants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aurea.CRM.Core.Common;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.Networking;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.ResourceHandling;
    using Aurea.CRM.Core.Utilities;
    using GalaSoft.MvvmLight.Messaging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Full sync requirement status values
    /// </summary>
    public enum FullSyncRequirementStatus
    {
        /// <summary>
        /// The mandatory.
        /// </summary>
        Mandatory = 0,

        /// <summary>
        /// The mandatory for online access.
        /// </summary>
        MandatoryForOnlineAccess,

        /// <summary>
        /// The recommended.
        /// </summary>
        Recommended,

        /// <summary>
        /// The unnecessary.
        /// </summary>
        Unnecessary,

        /// <summary>
        /// The resumable.
        /// </summary>
        Resumable
    }

    /// <summary>
    /// Sync operation queue types
    /// </summary>
    public enum SyncOperationQueueType
    {
        /// <summary>
        /// The incremental sync.
        /// </summary>
        IncrementalSync = 0,

        /// <summary>
        /// The up sync.
        /// </summary>
        UpSync,

        /// <summary>
        /// The full sync.
        /// </summary>
        FullSync,

        /// <summary>
        /// The single record sync.
        /// </summary>
        SingleRecordSync,

        /// <summary>
        /// The meta data.
        /// </summary>
        MetaData,

        /// <summary>
        /// The resources sync.
        /// </summary>
        ResourcesSync,

        /// <summary>
        /// The start async full sync.
        /// </summary>
        StartAsyncFullSync,

        /// <summary>
        /// The load async full sync.
        /// </summary>
        LoadAsyncFullSync
    }

    /// <summary>
    /// The constants.
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The sync operation queue operation type.
        /// </summary>
        public const string SyncOperationQueueOperationType = "OperationType";

        /// <summary>
        /// The sync operation queue record information.
        /// </summary>
        public const string SyncOperationQueueRecordInformation = "RecordInformation";

        /// <summary>
        /// The sync operation queue resume sync.
        /// </summary>
        public const string SyncOperationQueueResumeSync = "ResumeSync";

        /// <summary>
        /// The kup sync manager current step number.
        /// </summary>
        public const string KUPSyncManagerCurrentStepNumber = "currentStepNumber";

        /// <summary>
        /// The kup sync manager modified record identifications.
        /// </summary>
        public const string KUPSyncManagerModifiedRecordIdentifications = "modifiedRecordIdentifications";

        /// <summary>
        /// The kup sync manager number of failed up sync requests.
        /// </summary>
        public const string KUPSyncManagerNumberOfFailedUpSyncRequests = "numberOfFailedUpSyncRequests";

        /// <summary>
        /// The kup sync manager single record identification.
        /// </summary>
        public const string KUPSyncManagerSingleRecordIdentification = "recordIdentification";

        /// <summary>
        /// The kup sync manager sync error.
        /// </summary>
        public const string KUPSyncManagerSyncError = "error";

        /// <summary>
        /// The kup sync manager sync status hint.
        /// </summary>
        public const string KUPSyncManagerSyncStatusHint = "syncStatusHint";

        /// <summary>
        /// The kup sync manager sync count of total resources.
        /// </summary>
        public const string KUPSyncManagerTotalResources = "totalResources";

        /// <summary>
        /// The kup sync manager total step number.
        /// </summary>
        public const string KUPSyncManagerTotalStepNumber = "totalStepNumber";

        /// <summary>
        /// The sync manager full sync requirement block interval days.
        /// </summary>
        public const string SyncManagerFullSyncRequirementBlockIntervalDays = "28";

        /// <summary>
        /// The sync manager full sync requirement interval days.
        /// </summary>
        public const string SyncManagerFullSyncRequirementIntervalDays = "21";

        /// <summary>
        /// The sync manager full sync requirement warn interval days.
        /// </summary>
        public const string SyncManagerFullSyncRequirementWarnIntervalDays = "7";

        /// <summary>
        /// The sync manager incremental sync interval minutes wlan.
        /// </summary>
        public const string SyncManagerIncrementalSyncIntervalMinutesWLAN = "15";

        /// <summary>
        /// The sync manager incremental sync interval minutes wwan.
        /// </summary>
        public const string SyncManagerIncrementalSyncIntervalMinutesWWAN = "60";
    }

    /// <summary>
    /// Sync manager implementation
    /// </summary>
    /// <seealso cref="IServerSessionSyncHandlerDelegate" />
    public class UPSyncManager : IServerSessionSyncHandlerDelegate, IUPSyncManager
    {
        /// <summary>
        /// The suspend incremental sync.
        /// </summary>
        private bool suspendIncrementalSync;

        /// <summary>
        /// The use timer for incremental sync.
        /// </summary>
        private bool useTimerForIncrementalSync;

        /// <summary>
        /// The sync operation queue.
        /// </summary>
        private readonly List<Dictionary<string, object>> syncOperationQueue;

        /// <summary>
        /// The server login.
        /// </summary>
        private bool serverLogin;

        /// <summary>
        /// The document downloads queued.
        /// </summary>
        private bool documentDownloadsQueued;

        /// <summary>
        /// The token source.
        /// </summary>
        private readonly CancellationTokenSource tokenSource;

        /// <summary>
        /// The current sync handler.
        /// </summary>
        private ServerSessionSyncHandler currentSyncHandler;

        /// <summary>
        /// The messenger.
        /// </summary>
        private readonly IMessenger messenger = Messenger.Default;

#if PORTING
        protected Timer nextIncrementalSyncTimer;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncManager"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public UPSyncManager(ServerSession session)
        {
            this.Session = session;
            this.serverLogin = false;
            this.syncOperationQueue = new List<Dictionary<string, object>>();
            this.tokenSource = new CancellationTokenSource();

            this.messenger.Register<ConnectionWatchDogMessage>(this, ConnectionWatchDogMessageKey.DidEstablishServerConnectivity, this.ConnectionWatchDogDidEstablishServerConnectivity);
            this.messenger.Register<ConnectionWatchDogMessage>(this, ConnectionWatchDogMessageKey.DidLooseServerConnectivity, this.ConnectionWatchDogDidLooseServerConnectivity);
            this.messenger.Register<ConnectionWatchDogMessage>(this, ConnectionWatchDogMessageKey.DidChangeConnectivityQuality, this.ConnectionWatchDogDidChangeConnectivityQuality);
        }

        /// <summary>
        /// Operations the type running.
        /// </summary>
        /// <param name="operationType">
        /// Type of the operation.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool OperationTypeRunning(SyncOperationQueueType operationType)
        {
            return
                this.syncOperationQueue.Select(
                    operation => operation.ValueOrDefault(Constants.SyncOperationQueueOperationType))
                    .Any(type => type != null && (SyncOperationQueueType)type == operationType);
        }

        /// <summary>
        /// Gets a value indicating whether [up synchronize running].
        /// </summary>
        /// <value>
        /// <c>true</c> if [up synchronize running]; otherwise, <c>false</c>.
        /// </value>
        public bool UpSyncRunning => this.OperationTypeRunning(SyncOperationQueueType.UpSync);

        /// <summary>
        /// Gets a value indicating whether [full synchronize running].
        /// </summary>
        /// <value>
        /// <c>true</c> if [full synchronize running]; otherwise, <c>false</c>.
        /// </value>
        public bool FullSyncRunning => this.OperationTypeRunning(SyncOperationQueueType.FullSync);

        /// <summary>
        /// Gets a value indicating whether [incremental synchronize running].
        /// </summary>
        /// <value>
        /// <c>true</c> if [incremental synchronize running]; otherwise, <c>false</c>.
        /// </value>
        public bool IncrementalSyncRunning => this.OperationTypeRunning(SyncOperationQueueType.IncrementalSync);

        /// <summary>
        /// Gets a value indicating whether [metadata synchronize running].
        /// </summary>
        /// <value>
        /// <c>true</c> if [metadata synchronize running]; otherwise, <c>false</c>.
        /// </value>
        public bool MetadataSyncRunning => this.OperationTypeRunning(SyncOperationQueueType.MetaData);

        /// <summary>
        /// Gets a value indicating whether [resources synchronize running].
        /// </summary>
        /// <value>
        /// <c>true</c> if [resources synchronize running]; otherwise, <c>false</c>.
        /// </value>
        public bool ResourcesSyncRunning => this.OperationTypeRunning(SyncOperationQueueType.ResourcesSync);

        /// <summary>
        /// Records the synchronize running for record with identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RecordSyncRunningForRecordWithIdentification(string recordIdentification)
        {
            return
                this.syncOperationQueue.Select(
                    operation =>
                    new
                    {
                        Type = operation?.ValueOrDefault(Constants.SyncOperationQueueOperationType),
                        RecordInfo = operation?.ValueOrDefault(Constants.SyncOperationQueueRecordInformation) as string
                    })
                    .Any(
                        op => op?.Type != null && (SyncOperationQueueType)op.Type == SyncOperationQueueType.SingleRecordSync
                              && Equals(op.RecordInfo, recordIdentification));
        }

        /// <summary>
        /// Gets the full synchronize requirement status.
        /// </summary>
        /// <value>
        /// The full synchronize requirement status.
        /// </value>
        public FullSyncRequirementStatus FullSyncRequirementStatus
        {
            get
            {
                if (this.Session.AllwaysFullsyncWhenLogin() || this.Session.CrmAccount.FullSyncDateForLanguage(this.Session.LanguageKey) == null)
                {
                    return FullSyncRequirementStatus.Mandatory;
                }

                if (ServerSessionSyncHandler.CanResumeSync)
                {
                    return FullSyncRequirementStatus.Resumable;
                }

                // The FullSync is counted for the whole day , no matter what time you synchronize.
                DateTime dateLastFullSync = this.Session.CrmAccount.FullSyncDateForLanguage(this.Session.LanguageKey).Value;
                dateLastFullSync = new DateTime(dateLastFullSync.Year, dateLastFullSync.Month, dateLastFullSync.Day, 23, 59, 59);
                TimeSpan timeIntervalSinceLastFullSync = dateLastFullSync.TimeIntervalSinceNow().Duration();
                TimeSpan timeIntervalToBlock = this.TimeIntervalFromConfigInDays("Sync.FullSyncBlockIntervalDays", Constants.SyncManagerFullSyncRequirementBlockIntervalDays);

                if (timeIntervalSinceLastFullSync > timeIntervalToBlock)
                {
                    return FullSyncRequirementStatus.Mandatory;
                }

                TimeSpan timeIntervalToBlockOnline = this.TimeIntervalFromConfigInDays("Sync.FullSyncIntervalDays", Constants.SyncManagerFullSyncRequirementIntervalDays);
                if (timeIntervalSinceLastFullSync > timeIntervalToBlockOnline)
                {
                    return FullSyncRequirementStatus.MandatoryForOnlineAccess;
                }

                TimeSpan timeIntervalToWarn = this.TimeIntervalFromConfigInDays("Sync.FullSyncWarnIntervalDays", Constants.SyncManagerFullSyncRequirementWarnIntervalDays);
                return timeIntervalSinceLastFullSync > timeIntervalToWarn ? FullSyncRequirementStatus.Recommended : FullSyncRequirementStatus.Unnecessary;
            }
        }

        /// <summary>
        /// Gets the last synchronize date.
        /// </summary>
        /// <value>
        /// The last synchronize date.
        /// </value>
        public DateTime? LastSyncDate
        {
            get
            {
                var lastIncrementalSyncDate = this.Session.CrmAccount.IncrementalSyncDateForLanguage(this.Session.LanguageKey);
                if (lastIncrementalSyncDate.HasValue)
                {
                    return lastIncrementalSyncDate;
                }

                var lastFullSyncDate = this.Session.CrmAccount.FullSyncDateForLanguage(this.Session.LanguageKey);

                return lastFullSyncDate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [inc synchronize suspended].
        /// </summary>
        /// <value>
        /// <c>true</c> if [inc synchronize suspended]; otherwise, <c>false</c>.
        /// </value>
        public bool IncSyncSuspended { get; private set; }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public ServerSession Session { get; set; }

        /// <summary>
        /// Gets the last synchronize status hint.
        /// </summary>
        /// <value>
        /// The last synchronize status hint.
        /// </value>
        public string LastSyncStatusHint { get; private set; }

        /// <summary>
        /// Gets the explicit session.
        /// </summary>
        /// <value>
        /// The explicit session.
        /// </value>
        public ServerSession ExplicitSession { get; private set; }

        /// <summary>
        /// Invalidates the incremental synchronize timer.
        /// </summary>
        public void InvalidateIncrementalSyncTimer()
        {
            //nextIncrementalSyncTimer?.Invalidate();
            //nextIncrementalSyncTimer = null;
        }

        /// <summary>
        /// Shoulds the download documents with current connection status.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ShouldDownloadDocumentsWithCurrentConnectionStatus()
        {
            var documentDownloadCondition =
                this.Session.ConfigUnitStore.ConfigValueDefaultValue("Sync.DocumentSync", null) ?? "WLAN";

            if (documentDownloadCondition == "OFF")
            {
                return false;
            }

            if (documentDownloadCondition == "2G" || documentDownloadCondition == "UMTS")
            {
                return this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.ReachableViaWWAN
                    || this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.ReachableViaWiFi;
            }

            if (documentDownloadCondition == "WLAN")
            {
                return this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.ReachableViaWiFi;
            }

            return false;
        }

        /// <summary>
        /// Queues the document downloads if appropriate.
        /// </summary>
        public void QueueDocumentDownloadsIfAppropriate()
        {
            if (this.ShouldDownloadDocumentsWithCurrentConnectionStatus())
            {
                if (this.documentDownloadsQueued == false)
                {
                    UPSyncDocumentDownloadUrlCache urlCache = null;
                    if (this.Session.CrmAccount.SyncDocuments.Count > 20)
                    {
                        urlCache = new UPSyncDocumentDownloadUrlCache();
                    }

                    foreach (SyncDocument syncDocument in this.Session.CrmAccount.SyncDocuments)
                    {
                        foreach (Uri url in syncDocument.DownloadUrlsForDocument(urlCache))
                        {
                            ResourceDownload download = SmartbookResourceManager.DefaultResourceManager.QueueLowPriorityDownloadForResourceAtUrl(url, syncDocument.FileNameForDocument(urlCache), syncDocument.ServerModificationDate(urlCache));
                            download.DownloadFinishedEvent += this.ResourceDownloadDidFinish;
                        }
                    }

                    this.messenger.Register<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishDownloadingResourceNotification, this.ResourceDownloadDidFinish);
                    this.messenger.Register<SyncManagerMessage>(this, SyncManagerMessageKey.DidFailDownloadingResourceNotification, this.ResourceDownloadDidFail);

                    this.documentDownloadsQueued = true;
                    SmartbookResourceManager.DefaultResourceManager.StartProcessingDownloadQueue();
                }
            }
            else if (this.documentDownloadsQueued)
            {
                this.documentDownloadsQueued = false;
                SmartbookResourceManager.DefaultResourceManager.StopProcessingDownloadQueue();
                SmartbookResourceManager.DefaultResourceManager.CancelAllDownloads();
                this.messenger.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFailDownloadingResourceNotification);
                this.messenger.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishDownloadingResourceNotification);
            }
        }

        /// <summary>
        /// Starts the syncing documents after server login.
        /// </summary>
        public void StartSyncingDocumentsAfterServerLogin()
        {
            this.serverLogin = true;
            this.QueueDocumentDownloadsIfAppropriate();
        }

        /// <summary>
        /// Setups the new incremental synchronize timer.
        /// </summary>
        public void SetupNewIncrementalSyncTimer()
        {
#if PORTING
            if (nextIncrementalSyncTimer)
            {
                InvalidateIncrementalSyncTimer();
            }
#endif
            if (this.useTimerForIncrementalSync == false)
            {
                return;
            }

            this.SetupNewIncrementalSyncFromDate(this.Session.CrmAccount.IncrementalSyncDateForLanguage(this.Session.LanguageKey));
        }

        /// <summary>
        /// Setups the new incremental synchronize from date.
        /// </summary>
        /// <param name="lastSyncDate">
        /// The last synchronize date.
        /// </param>
        public void SetupNewIncrementalSyncFromDate(DateTime? lastSyncDate)
        {
            this.InvalidateIncrementalSyncTimer();

            if (this.useTimerForIncrementalSync == false || !lastSyncDate.HasValue)
            {
                return;
            }

            var timeIntervalForTimer = TimeSpan.Zero;
            var timeIntervalSinceLastIncrementalSync = DateTime.UtcNow - lastSyncDate.Value;
            TimeSpan? timeIntervalUntilNextTimerTrigger = TimeSpan.Zero;
            var configStore = this.Session.ConfigUnitStore;

            UPSyncIntervalComputation syncIntervalComputation = null;
            if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.NotReachable)
            {
                return;
            }

            if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.ReachableViaWWAN)
            {
                var syncConfig = configStore.ConfigValue("Sync.ConfigUMTS");
                if (string.IsNullOrEmpty(syncConfig))
                {
                    syncConfig = configStore.ConfigValue("Sync.Config");
                }

                if (!string.IsNullOrEmpty(syncConfig))
                {
                    syncIntervalComputation = UPSyncIntervalComputation.Create(syncConfig);
                }

                if (syncIntervalComputation == null && configStore.ConfigValueDefaultValue("Sync.IntervalMinutesUMTS", null) == "0")
                {
                    return;
                }

                timeIntervalForTimer =
                    this.TimeIntervalFromConfigInMinutesWithDefaultValue(
                        "Sync.IntervalMinutesUMTS",
                        Constants.SyncManagerIncrementalSyncIntervalMinutesWWAN);
            }
            else if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.ReachableViaWiFi)
            {
                var syncConfig = configStore.ConfigValue("Sync.ConfigWLAN");
                if (string.IsNullOrEmpty(syncConfig))
                {
                    syncConfig = configStore.ConfigValue("Sync.Config");
                }

                if (!string.IsNullOrEmpty(syncConfig))
                {
                    syncIntervalComputation = UPSyncIntervalComputation.Create(syncConfig);
                }

                if (syncIntervalComputation == null && configStore.ConfigValueDefaultValue("Sync.IntervalMinutesWLAN", null) == "0")
                {
                    return;
                }

                timeIntervalForTimer =
                    this.TimeIntervalFromConfigInMinutesWithDefaultValue(
                        "Sync.IntervalMinutesWLAN",
                        Constants.SyncManagerIncrementalSyncIntervalMinutesWLAN);
            }

            if (syncIntervalComputation != null)
            {
                timeIntervalUntilNextTimerTrigger = syncIntervalComputation.NextIncrementalSyncFor(lastSyncDate.GetValueOrDefault());
                if (timeIntervalUntilNextTimerTrigger == null)
                {
                    syncIntervalComputation = null;
                }
            }

            if (syncIntervalComputation == null)
            {
                if (timeIntervalForTimer == TimeSpan.Zero)
                {
                    return;
                }

                timeIntervalUntilNextTimerTrigger = timeIntervalForTimer - timeIntervalSinceLastIncrementalSync;
            }

            if (timeIntervalUntilNextTimerTrigger.Value > TimeSpan.Zero)
            {
                // nextIncrementalSyncTimer = NSTimer.ScheduledTimerWithTimeIntervalTargetSelectorUserInfoRepeats(timeIntervalUntilNextTimerTrigger, this, @selector(triggerIncSync), null, false);
                Task.Run(
                    async () =>
                    {
                        await Task.Delay(timeIntervalUntilNextTimerTrigger.Value, this.tokenSource.Token);
                        this.PerformIncrementalSync();
                    },
                    this.tokenSource.Token);
            }
            else
            {
                this.PerformIncrementalSync();
            }
        }

        /// <summary>
        /// Suspends the incremental synchronize if full synchronize required.
        /// </summary>
        /// <param name="suspend">
        /// if set to <c>true</c> [suspend].
        /// </param>
        public void SuspendIncrementalSyncIfFullSyncRequired(bool suspend)
        {
            if (suspend)
            {
                if (this.FullSyncRequirementStatus != FullSyncRequirementStatus.Unnecessary)
                {
                    this.IncSyncSuspended = true;
                }
            }
            else
            {
                if (this.IncSyncSuspended)
                {
                    this.IncSyncSuspended = false;
                    this.SetUpIncrementalSyncTimer();
                }
            }
        }

        /// <summary>
        /// Sets up incremental synchronize timer.
        /// </summary>
        public void SetUpIncrementalSyncTimer()
        {
            if (this.IncSyncSuspended)
            {
                return;
            }

            this.useTimerForIncrementalSync = true;
            this.SetupNewIncrementalSyncTimer();
        }

        /// <summary>
        /// Tears down synchronize manager.
        /// </summary>
        public void TearDownSyncManager()
        {
            this.documentDownloadsQueued = false;
            SmartbookResourceManager.DefaultResourceManager.StopProcessingDownloadQueue();
            SmartbookResourceManager.DefaultResourceManager.CancelAllDownloads();

            this.messenger.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFailDownloadingResourceNotification);
            this.messenger.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishDownloadingResourceNotification);

            this.InvalidateIncrementalSyncTimer();
        }

        private void ConnectionWatchDogDidEstablishServerConnectivity(object notification)
        {
            this.SetupNewIncrementalSyncTimer();
        }

        private void ConnectionWatchDogDidLooseServerConnectivity(object notification)
        {
            this.serverLogin = false;
            this.documentDownloadsQueued = false;
            SmartbookResourceManager.DefaultResourceManager.StopProcessingDownloadQueue();
            SmartbookResourceManager.DefaultResourceManager.CancelAllDownloads();

            this.messenger.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFailDownloadingResourceNotification);
            this.messenger.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishDownloadingResourceNotification);

            this.InvalidateIncrementalSyncTimer();
        }

        private void ConnectionWatchDogDidChangeConnectivityQuality(object notification)
        {
            if (this.serverLogin)
            {
                this.QueueDocumentDownloadsIfAppropriate();
            }

            this.SetupNewIncrementalSyncTimer();
        }

        void Dealloc()
        {
            this.messenger.Unregister<ConnectionWatchDogMessage>(this, ConnectionWatchDogMessageKey.DidEstablishServerConnectivity);
            this.messenger.Unregister<ConnectionWatchDogMessage>(this, ConnectionWatchDogMessageKey.DidLooseServerConnectivity);
            this.messenger.Unregister<ConnectionWatchDogMessage>(this, ConnectionWatchDogMessageKey.DidChangeConnectivityQuality);
        }

        /// <summary>
        /// Starts the next synchronize operation.
        /// </summary>
        private void StartNextSyncOperation()
        {
            ServerSessionSyncHandler newSyncHandler = null;
            string newRecordIdentification = null;

            if (this.syncOperationQueue.Count == 0 || this.currentSyncHandler != null)
            {
                return;
            }

            var syncOperationDictionary = this.syncOperationQueue[0];
            var syncOperationType = (SyncOperationQueueType)syncOperationDictionary.ValueOrDefault(Constants.SyncOperationQueueOperationType);

            SyncOption? syncOptions;
            switch (syncOperationType)
            {
                case SyncOperationQueueType.FullSync:
                    var resumeSync = (bool)syncOperationDictionary.ValueOrDefault(Constants.SyncOperationQueueResumeSync);
                    if (resumeSync)
                    {
                        newSyncHandler = new ServerSessionSyncHandler(this.Session, this);
                    }
                    else
                    {
                        syncOptions = SyncOption.DataModel
                                      | SyncOption.Catalogs
                                      | SyncOption.Configuration
                                      | SyncOption.DataSetDefinition
                                      | SyncOption.RecordData
                                      | SyncOption.FullSync
                                      | SyncOption.DeleteDatabases
                                      | SyncOption.Documents;

                        newSyncHandler = new ServerSessionSyncHandler(this.Session, syncOptions.Value, this);
                    }

                    break;

                // Commented out in xcode.... not sure why.
                //case SyncOperationQueueType.StartAsyncFullSync:
                //    syncOptions = SyncOption.StartAsync;
                //    newSyncHandler = new ServerSessionSyncHandler(this.Session, syncOptions.Value, this);
                //    break;

                //case SyncOperationQueueType.LoadAsyncFullSync:
                //    syncOptions = SyncOption.RecordDataIncremental | SyncOption.CatalogsIncremental
                //                  | SyncOption.Documents | SyncOption.LoadAsync | SyncOption.DeleteDatabases;

                //    newSyncHandler = new ServerSessionSyncHandler(this.Session, syncOptions.Value, this);
                //    break;

                case SyncOperationQueueType.IncrementalSync:
                    syncOptions = SyncOption.RecordDataIncremental | SyncOption.CatalogsIncremental | SyncOption.Documents;

                    if (ConfigurationUnitStore.DefaultStore != null && !ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Sync.DisableSeparateSession"))
                    {
                        syncOptions |= SyncOption.UseSeparateSession;
                    }

                    newSyncHandler = new ServerSessionSyncHandler(this.Session, syncOptions.Value, this);
                    break;

                case SyncOperationQueueType.UpSync:
                    syncOptions = SyncOption.UncommittedData;
                    newSyncHandler = new ServerSessionSyncHandler(this.Session, syncOptions.Value, this);
                    break;

                case SyncOperationQueueType.SingleRecordSync:
                    var recordIdentification = syncOperationDictionary.ValueOrDefault(Constants.SyncOperationQueueRecordInformation) as string;
                    newRecordIdentification = recordIdentification;
                    newSyncHandler = new ServerSessionSyncHandler(this.Session, recordIdentification, this);
                    break;

                case SyncOperationQueueType.MetaData:
                    syncOptions = SyncOption.Configuration;
                    newSyncHandler = new ServerSessionSyncHandler(this.Session, syncOptions.Value, this);
                    break;

                case SyncOperationQueueType.ResourcesSync:
                    syncOptions = SyncOption.Resources;
                    newSyncHandler = new ServerSessionSyncHandler(this.Session, syncOptions.Value, this);
                    break;
                default:
                    break;
            }

            SyncOperationQueueType newSyncOperationType = syncOperationType;
            this.currentSyncHandler = newSyncHandler;

            if (newSyncHandler == null || !this.syncOperationQueue.Any())
            {
                this.currentSyncHandler = null;
                return;
            }

            this.InvalidateIncrementalSyncTimer();

            switch (newSyncOperationType)
            {
                case SyncOperationQueueType.FullSync:
                case SyncOperationQueueType.LoadAsyncFullSync:
                case SyncOperationQueueType.StartAsyncFullSync:
                    this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidStartFullSync));
                    break;

                case SyncOperationQueueType.IncrementalSync:
                    this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidStartIncrementalSync));
                    break;

                case SyncOperationQueueType.UpSync:
                    this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidStartUpSync));
                    break;

                case SyncOperationQueueType.SingleRecordSync:
                    this.messenger?.Send(
                        SyncManagerMessage.Create(
                            SyncManagerMessageKey.DidStartSingleRecordSync,
                            new Dictionary<string, object> { { Constants.KUPSyncManagerSingleRecordIdentification, newRecordIdentification } }));
                    break;

                case SyncOperationQueueType.MetaData:
                    this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidStartMetadataSync));
                    break;

                case SyncOperationQueueType.ResourcesSync:
                    this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidStartResourcesSync));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            newSyncHandler.Start();
        }

        /// <summary>
        /// Performs the incremental synchronize.
        /// </summary>
        public async void PerformIncrementalSync()
        {
            if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.NotReachable)
            {
                return;
            }

            this.PerformUpSync();

            Task t = Task.Run(
                () =>
                    {
                        if (this.IncrementalSyncRunning == false)
                        {
                            this.syncOperationQueue.Add(
                                new Dictionary<string, object>
                                    {
                                        { Constants.SyncOperationQueueOperationType, SyncOperationQueueType.IncrementalSync }
                                    });
                        }
                    });
            t.Wait();

            this.StartNextSyncOperation();
        }

        /// <summary>
        /// Performs the Start asynchronous full synchronize.
        /// </summary>
        public async void PerformStartAsyncFullSync()
        {
            if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.NotReachable)
            {
                return;
            }

            await Task.Run(
                () =>
                    {
                        if (this.FullSyncRunning == false)
                        {
                            this.syncOperationQueue.Add(
                                new Dictionary<string, object>
                                    {
                                        { Constants.SyncOperationQueueOperationType, SyncOperationQueueType.StartAsyncFullSync }
                                    });
                        }
                    });

            this.StartNextSyncOperation();
        }

        /// <summary>
        /// Performs the load asynchronous full synchronize.
        /// </summary>
        public async void PerformLoadAsyncFullSync()
        {
            if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.NotReachable)
            {
                return;
            }

            await Task.Run(
                () =>
                    {
                        if (this.MetadataSyncRunning == false)
                        {
                            this.syncOperationQueue.Add(
                                new Dictionary<string, object>
                                    {
                                        { Constants.SyncOperationQueueOperationType, SyncOperationQueueType.LoadAsyncFullSync }
                                    });
                        }
                    });

            this.StartNextSyncOperation();
        }

        /// <summary>
        /// Performs the full synchronize.
        /// </summary>
        public void PerformFullSync()
        {
            this.PerformFullSyncWithResumeSync(false);
        }

        /// <summary>
        /// Performs the full synchronize with resume synchronize.
        /// </summary>
        /// <param name="resume">
        /// if set to <c>true</c> [resume].
        /// </param>
        public void PerformFullSyncWithResumeSync(bool resume)
        {
            if (this.Session.ConnectionWatchDog?.LastServerReachabilityStatus == ReachabilityStatus.NotReachable)
            {
                this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidCancelFullSync));
                return;
            }

            SmartbookResourceManager.DefaultResourceManager?.CancelAllDownloads();

            this.messenger.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFailDownloadingResourceNotification);
            this.messenger.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishDownloadingResourceNotification);

            if (!resume && this.Session.CrmAccount.FullSyncDateForLanguage(this.Session.LanguageKey) != null)
            {
                this.PerformUpSync();
            }

            if (this.FullSyncRunning == false)
            {
                this.syncOperationQueue.Add(
                    new Dictionary<string, object>
                        {
                            { Constants.SyncOperationQueueOperationType, SyncOperationQueueType.FullSync },
                            { Constants.SyncOperationQueueResumeSync, resume }
                        });
            }

            this.StartNextSyncOperation();
        }

        /// <summary>
        /// Performs up synchronize.
        /// </summary>
        public void PerformUpSync()
        {
            if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.NotReachable)
            {
                return;
            }

            if (this.Session.OfflineStorage.NumberOfUncommittedRequests() > 0)
            {
                var upSyncQueued = false;
                lock (this)
                {
                    if (this.UpSyncRunning == false)
                    {
                        this.syncOperationQueue.Add(new Dictionary<string, object> { { Constants.SyncOperationQueueOperationType, SyncOperationQueueType.UpSync } });
                        upSyncQueued = true;
                    }
                }

                if (upSyncQueued)
                {
                    this.StartNextSyncOperation();
                }
            }
            else
            {
                lock (this)
                {
                    if (this.UpSyncRunning == false)
                    {
                        this.Session.OfflineStorage.BlockOnlineRecordRequest = false;
                    }
                }
            }
        }

        /// <summary>
        /// Performs the metadata synchronize.
        /// </summary>
        public async void PerformMetadataSync()
        {
            if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.NotReachable)
            {
                return;
            }

            await Task.Run(
                () =>
                    {
                        if (this.MetadataSyncRunning == false)
                        {
                            this.syncOperationQueue.Add(
                                new Dictionary<string, object>
                                    {
                                        { Constants.SyncOperationQueueOperationType, SyncOperationQueueType.MetaData }
                                    });
                        }
                    });

            this.StartNextSyncOperation();
        }

        /// <summary>
        /// Performs the resources sync.
        /// </summary>
        public async void PerformResourcesSync()
        {
            if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.NotReachable)
            {
                return;
            }

            await Task.Run(
                () =>
                    {
                        if (this.ResourcesSyncRunning == false)
                        {
                            this.syncOperationQueue.Add(
                                new Dictionary<string, object>
                                    {
                                        { Constants.SyncOperationQueueOperationType, SyncOperationQueueType.ResourcesSync }
                                    });
                        }
                    });

            this.StartNextSyncOperation();
        }

        /// <summary>
        /// Performs the synchronize for record with identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        public async void PerformSyncForRecord(string recordIdentification)
        {
            if (this.Session.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.NotReachable)
            {
                return;
            }

            await Task.Run(
                () =>
                    {
                        if (!this.RecordSyncRunningForRecordWithIdentification(recordIdentification))
                        {
                            this.syncOperationQueue.Add(
                                new Dictionary<string, object>
                                    {
                                        { Constants.SyncOperationQueueOperationType, SyncOperationQueueType.SingleRecordSync },
                                        { Constants.SyncOperationQueueRecordInformation, recordIdentification }
                                    });
                        }
                    });

            this.StartNextSyncOperation();
        }

        /// <summary>
        /// Stops the sync operation
        /// </summary>
        public void StopFullSync()
        {
            this.Session?.RemoteSession?.ServerOperationManager?.Stop();
            this.Session?.ResourceManager?.CancelAllDownloads();

            if (!this.syncOperationQueue.Any())
            {
                return;
            }

            if (this.currentSyncHandler != null)
            {
                this.currentSyncHandler.CancelCurrentSync();
            }

            if (!this.syncOperationQueue.Any())
            {
                return;
            }

            this.currentSyncHandler = null;
            var syncOperationDictionary = this.syncOperationQueue[0];
            var syncOperationType = (SyncOperationQueueType)syncOperationDictionary.ValueOrDefault(Constants.SyncOperationQueueOperationType);
            if (syncOperationType == SyncOperationQueueType.FullSync)
            {
                this.syncOperationQueue.RemoveAt(0);

                this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidCancelFullSync));
            }
        }

        /// <summary>
        /// Times the interval from configuration value in minutes with name default value.
        /// </summary>
        /// <param name="configValueName">
        /// Name of the configuration value.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        public TimeSpan TimeIntervalFromConfigInMinutesWithDefaultValue(string configValueName, string defaultValue)
        {
            var configValue = this.Session.ConfigUnitStore.ConfigValueDefaultValue(configValueName, defaultValue);
            var configValueFloat = float.Parse(configValue);
            if (configValueFloat <= 0.00001)
            {
                configValueFloat = float.Parse(defaultValue);
            }

            return TimeSpan.FromSeconds(configValueFloat * 60);
        }

        /// <summary>
        /// Times the interval from configuration value in days.
        /// </summary>
        /// <param name="configValueName">
        /// Name of the configuration value.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        public TimeSpan TimeIntervalFromConfigInDays(string configValueName, string defaultValue)
        {
            var configValue = this.Session.ConfigUnitStore != null
                ? this.Session.ConfigUnitStore.ConfigValueDefaultValue(configValueName, defaultValue)
                : defaultValue;
            var configValueFloat = float.Parse(configValue);
            if (configValueFloat <= 0.00001)
            {
                configValueFloat = float.Parse(defaultValue);
            }

            return TimeSpan.FromSeconds(configValueFloat * 24 * 60 * 60);
        }

        /// <summary>
        /// Times the interval from now until block.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        public TimeSpan TimeIntervalFromNowUntilBlock()
        {
            var date = this.Session.CrmAccount.FullSyncDateForLanguage(this.Session.LanguageKey);
            var timeIntervalSinceLastFullSync = date.HasValue ? DateTime.UtcNow - date.Value : TimeSpan.Zero;

            var timeIntervalToBlock = this.TimeIntervalFromConfigInDays(
                "Sync.FullSyncBlockIntervalDays",
                Constants.SyncManagerFullSyncRequirementBlockIntervalDays);

            return timeIntervalToBlock - timeIntervalSinceLastFullSync;
        }

        /// <summary>
        /// Times the interval from now until offline online.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        public TimeSpan TimeIntervalFromNowUntilOfflineOnline()
        {
            var date = this.Session.CrmAccount.FullSyncDateForLanguage(this.Session.LanguageKey);
            var timeIntervalSinceLastFullSync = date.HasValue ? DateTime.UtcNow - date.Value : TimeSpan.Zero;

            var timeIntervalToBlockOnline = this.TimeIntervalFromConfigInDays(
                "Sync.FullSyncIntervalDays",
                Constants.SyncManagerFullSyncRequirementIntervalDays);

            return timeIntervalToBlockOnline - timeIntervalSinceLastFullSync;
        }

        /// <summary>
        /// Checks for document updates remove all others.
        /// </summary>
        /// <param name="syncDocuments">The synchronize documents.</param>
        /// <param name="removeAllOthers">if set to <c>true</c> [remove all others].</param>
        public void CheckForDocumentUpdatesRemoveAllOthers(List<SyncDocument> syncDocuments, bool removeAllOthers)
        {
            ResourceManager resourceManager = SmartbookResourceManager.DefaultResourceManager;
            IStorageProvider storageProvider = SimpleIoc.Default.GetInstance<IStorageProvider>();

            List<string> transientResourcesFileNames = storageProvider.ContentsOfDirectory(resourceManager.LocalPathForTransientResources);
            UPSyncDocumentDownloadUrlCache urlCache = null;
            if (syncDocuments != null)
            {
                if (syncDocuments.Count > 20)
                {
                    urlCache = new UPSyncDocumentDownloadUrlCache();
                }

                foreach (SyncDocument document in syncDocuments)
                {
                    foreach (Uri url in document.DownloadUrlsForDocument(urlCache))
                    {
                        Resource resource = resourceManager.ResourceForUrl(url, document.FileNameForDocument(urlCache));
                        if (resource != null)
                        {
                            transientResourcesFileNames.Remove(Path.GetFileName(resource.LocalUrl));
                        }
                    }
                }
            }

            if (removeAllOthers)
            {
                foreach (string transientResource in transientResourcesFileNames)
                {
                    Exception error;
                    storageProvider.TryDelete(Path.Combine(resourceManager.LocalPathForTransientResources, transientResource), out error);
                }
            }

            List<SyncDocument> syncDocumentsToDownload = new List<SyncDocument>();
            if (syncDocuments != null)
            {
                foreach (SyncDocument document in syncDocuments)
                {
                    foreach (Uri url in document.DownloadUrlsForDocument(urlCache))
                    {
                        Resource resource = resourceManager.ResourceForUrl(url, document.FileNameForDocument(urlCache));
                        if (resource == null)
                        {
                            syncDocumentsToDownload.Add(document);
                        }

                        if (resource != null)
                        {
                            DateTime? serverModificationDate = document.ServerModificationDate(urlCache);
                            if (serverModificationDate != null && resource.LastModificationDate < serverModificationDate)
                            {
                                resource.DeleteResource();
                                syncDocumentsToDownload.Add(document);
                            }
                        }
                    }
                }
            }

            this.documentDownloadsQueued = false;
            this.Session.CrmAccount.UpdateSyncDocuments(syncDocumentsToDownload);
            this.QueueDocumentDownloadsIfAppropriate();
        }

        /// <summary>
        /// Servers the session synchronize handler status hint.
        /// </summary>
        /// <param name="syncHandler">
        /// The synchronize handler.
        /// </param>
        /// <param name="statusHint">
        /// The status hint.
        /// </param>
        public void ServerSessionSyncHandlerStatusHint(ServerSessionSyncHandler syncHandler, string statusHint)
        {
            this.LastSyncStatusHint = statusHint;
            var state = new Dictionary<string, object> { { Constants.KUPSyncManagerSyncStatusHint, this.LastSyncStatusHint } };
            this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidProvideSyncStatusHint, state));
        }

        /// <summary>
        /// Servers the session synchronize handler did proceed to step number of steps.
        /// </summary>
        /// <param name="syncHandler">
        /// The synchronize handler.
        /// </param>
        /// <param name="currentStepNumber">
        /// The current step number.
        /// </param>
        /// <param name="totalStepNumber">
        /// The total step number.
        /// </param>
        public void ServerSessionSyncHandlerDidProceedToStepNumberOfSteps(
            ServerSessionSyncHandler syncHandler,
            int currentStepNumber,
            int totalStepNumber)
        {
            var state = new Dictionary<string, object>
                            {
                                { Constants.KUPSyncManagerCurrentStepNumber, currentStepNumber },
                                { Constants.KUPSyncManagerTotalStepNumber, totalStepNumber }
                            };

            this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidProgressSync, state));
        }

        /// <summary>
        /// Servers the session synchronize handler did finish.
        /// </summary>
        /// <param name="syncHandler">The synchronize handler.</param>
        /// <param name="changedRecordIdentifications">The changed record identifications.</param>
        /// <param name="syncDocuments">The synchronize documents.</param>
        public void ServerSessionSyncHandlerDidFinish(
            ServerSessionSyncHandler syncHandler,
            List<string> changedRecordIdentifications,
            List<SyncDocument> syncDocuments)
        {
            string notificationName = null;
            string recordIdentification = null;
            var postConfigChange = false;
            this.ExplicitSession = null;

            this.currentSyncHandler = null;
            this.Session.OfflineStorage.SetSyncIsActive(false);

            if (!this.syncOperationQueue.Any())
            {
                return;
            }

            var syncOperationDictionary = this.syncOperationQueue[0];
            var syncOperationType = (SyncOperationQueueType)syncOperationDictionary.ValueOrDefault(Constants.SyncOperationQueueOperationType);
            this.syncOperationQueue.RemoveAt(0);

            switch (syncOperationType)
            {
                case SyncOperationQueueType.FullSync:
                case SyncOperationQueueType.LoadAsyncFullSync:
                    notificationName = SyncManagerMessageKey.DidFinishFullSync;
                    postConfigChange = true;
                    this.Session.CrmAccount.UpdateFullSyncDate(this.Session.LanguageKey);
                    this.Session.CrmAccount.UpdateIncrementalSyncDate(this.Session.LanguageKey);
                    this.SuspendIncrementalSyncIfFullSyncRequired(false);
                    break;

                case SyncOperationQueueType.IncrementalSync:
                    notificationName = SyncManagerMessageKey.DidFinishIncrementalSync;
                    this.Session.CrmAccount.UpdateIncrementalSyncDate(this.Session.LanguageKey);
                    break;

                case SyncOperationQueueType.UpSync:
                    this.Session.OfflineStorage.ClearCachedRequestNumbers();
                    notificationName = SyncManagerMessageKey.DidFinishUpSync;
                    break;

                case SyncOperationQueueType.SingleRecordSync:
                    notificationName = SyncManagerMessageKey.DidFinishSingleRecordSync;
                    recordIdentification = syncOperationDictionary.ValueOrDefault(Constants.SyncOperationQueueRecordInformation) as string;
                    break;

                case SyncOperationQueueType.MetaData:
                    notificationName = SyncManagerMessageKey.DidFinishMetadataSync;
                    this.Session.CrmAccount.UpdateIncrementalSyncDate(this.Session.LanguageKey);
                    postConfigChange = true;
                    break;

                case SyncOperationQueueType.ResourcesSync:
                    notificationName = SyncManagerMessageKey.DidFinishResourcesSync;
                    postConfigChange = true;
                    break;

                case SyncOperationQueueType.StartAsyncFullSync:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrEmpty(notificationName))
            {
                var userInfo = new Dictionary<string, object>();
                if (changedRecordIdentifications?.Count > 0 && notificationName != SyncManagerMessageKey.DidFinishFullSync)
                {
                    userInfo[Constants.KUPSyncManagerModifiedRecordIdentifications] = changedRecordIdentifications;
                }

                if (notificationName == SyncManagerMessageKey.DidFinishSingleRecordSync)
                {
                    userInfo[Constants.KUPSyncManagerSingleRecordIdentification] = recordIdentification;
                }
                else if (notificationName == SyncManagerMessageKey.DidFinishUpSync)
                {
                    int numberOfRequestsWithError = this.Session.OfflineStorage.NumberOfRequestsWithErrors();
                    if (numberOfRequestsWithError > 0)
                    {
                        userInfo[Constants.KUPSyncManagerNumberOfFailedUpSyncRequests] = numberOfRequestsWithError;
                    }
                }

                this.messenger?.Send(SyncManagerMessage.Create(notificationName, userInfo));

                if (changedRecordIdentifications?.Count > 0)
                {
                    if (changedRecordIdentifications.Count > 10)
                    {
                        changedRecordIdentifications = new List<string> { changedRecordIdentifications[0] };
                    }

                    var state = new Dictionary<string, object>
                                    {
                                        { Constants.KUPSyncManagerModifiedRecordIdentifications, changedRecordIdentifications }
                                    };

                    this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidUpdateRecords, state));
                }
            }

            if (notificationName == SyncManagerMessageKey.DidFinishIncrementalSync)
            {
                this.SetupNewIncrementalSyncTimer();
                this.serverLogin = true;
                if (syncDocuments?.Count > 0)
                {
                    SmartbookResourceManager.DefaultResourceManager.CancelAllDownloads();
                    this.Session.CrmAccount.AddSyncDocuments(syncDocuments);
                    this.CheckForDocumentUpdatesRemoveAllOthers(this.Session.CrmAccount.SyncDocuments, false);
                }
            }
            else if (notificationName == SyncManagerMessageKey.DidFinishFullSync)
            {
                this.SetupNewIncrementalSyncTimer();
                this.serverLogin = true;
                SmartbookResourceManager.DefaultResourceManager.CancelAllDownloads();
                this.CheckForDocumentUpdatesRemoveAllOthers(syncDocuments, true);
            }
            else
            {
                this.Session.CrmAccount.AddSyncDocuments(syncDocuments);
            }

            if (postConfigChange)
            {
                this.messenger?.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidChangeConfiguration), SyncManagerMessageKey.DidChangeConfiguration);
            }

            this.StartNextSyncOperation();
        }

        /// <summary>
        /// Servers the session synchronize handler did finish with error.
        /// </summary>
        /// <param name="syncHandler">
        /// The synchronize handler.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void ServerSessionSyncHandlerDidFinishWithError(ServerSessionSyncHandler syncHandler, Exception error)
        {
            string notificationName = null;
            string recordIdentification = null;
            if (error != null)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError(error.Message);
            }

            this.ExplicitSession = null;
            this.currentSyncHandler = null;

            if (this.syncOperationQueue.Any())
            {
                var syncOperationDictionary = this.syncOperationQueue[0];
                var syncOperationType = (SyncOperationQueueType)syncOperationDictionary.ValueOrDefault(Constants.SyncOperationQueueOperationType);
                this.syncOperationQueue.RemoveAt(0);

                switch (syncOperationType)
                {
                    case SyncOperationQueueType.FullSync:
                        notificationName = SyncManagerMessageKey.DidFailFullSync;
                        break;

                    case SyncOperationQueueType.IncrementalSync:
                        notificationName = SyncManagerMessageKey.DidFailIncrementalSync;
                        break;

                    case SyncOperationQueueType.UpSync:
                        notificationName = SyncManagerMessageKey.DidFailUpSync;
                        break;

                    case SyncOperationQueueType.SingleRecordSync:
                        notificationName = SyncManagerMessageKey.DidFailSingleRecordSync;
                        recordIdentification =
                            syncOperationDictionary.ValueOrDefault(Constants.SyncOperationQueueRecordInformation) as string;
                        break;

                    case SyncOperationQueueType.MetaData:
                        notificationName = SyncManagerMessageKey.DidFailMetadataSync;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(notificationName))
            {
                var userInfoDictionary = new Dictionary<string, object>();
                if (error != null)
                {
                    userInfoDictionary[Constants.KUPSyncManagerSyncError] = error;
                }

                if (notificationName == SyncManagerMessageKey.DidFailSingleRecordSync)
                {
                    userInfoDictionary[Constants.KUPSyncManagerSingleRecordIdentification] = recordIdentification;
                }
                else if (notificationName == SyncManagerMessageKey.DidFailUpSync)
                {
                    int numberOfRequestsWithError = this.Session.OfflineStorage.NumberOfRequestsWithErrors();
                    if (numberOfRequestsWithError > 0)
                    {
                        userInfoDictionary[Constants.KUPSyncManagerNumberOfFailedUpSyncRequests] = numberOfRequestsWithError;
                    }
                }

                this.messenger?.Send(SyncManagerMessage.Create(notificationName, userInfoDictionary));
            }

            this.SetupNewIncrementalSyncFromDate(DateTime.UtcNow);
            this.StartNextSyncOperation();
        }

        private void ResourceDownloadDidFinish(object objDownload, EventArgs args)
        {
            ResourceDownload download = (ResourceDownload)objDownload;
            this.Session.CrmAccount.RemoveSyncDocumentFromDownloadQueue(download.DownloadUrl);
        }

        private void ResourceDownloadDidFinish(SyncManagerMessage notification)
        {
            ResourceDownload download = (ResourceDownload)notification.State.ValueOrDefault("TheObject");
            this.Session.CrmAccount.RemoveSyncDocumentFromDownloadQueue(download.DownloadUrl);
        }

        private void ResourceDownloadDidFail(SyncManagerMessage notification)
        {
            Exception error = notification.State.ValueOrDefault(Constants.KUPSyncManagerSyncError) as Exception;
            ResourceDownload download = (ResourceDownload)notification.State.ValueOrDefault("TheObject");

            Exception deleteError;
            SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.TryDelete(download.LocalUrl, out deleteError);

            if ((error != null && error.Message == "NotAuthenticatedError") || this.serverLogin == false)
            {
                this.serverLogin = false;
                this.documentDownloadsQueued = false;
                SmartbookResourceManager.DefaultResourceManager.StopProcessingDownloadQueue();
                SmartbookResourceManager.DefaultResourceManager.CancelAllDownloads();
            }
            else
            {
                this.Session.CrmAccount.RemoveSyncDocumentFromDownloadQueue(download.DownloadUrl);
            }
        }
    }
}
