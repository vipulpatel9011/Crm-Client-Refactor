// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncManagerMessage.cs" company="Aurea Software Gmbh">
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
//   Keep track of message keys used by Sync manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// Keep track of message keys used by Sync manager
    /// </summary>
    public static class SyncManagerMessageKey
    {
        /// <summary>
        /// The did cancel full synchronize message key
        /// </summary>
        public const string DidCancelFullSync = "DidCancelFullSyncNotification";

        /// <summary>
        /// The did change configuration message key
        /// </summary>
        public const string DidChangeConfiguration = "DidChangeConfigurationNotification";

        /// <summary>
        /// The did fail full synchronize message key
        /// </summary>
        public const string DidFailFullSync = "DidFailFullSyncNotification";

        /// <summary>
        /// The did fail incremental synchronize message key
        /// </summary>
        public const string DidFailIncrementalSync = "DidFailIncrementalSyncNotification";

        /// <summary>
        /// The did fail metadata synchronize message key
        /// </summary>
        public const string DidFailMetadataSync = "DidFailMetadataSyncNotification";

        /// <summary>
        /// The did fail single record synchronize message key
        /// </summary>
        public const string DidFailSingleRecordSync = "DidFailSingleRecordSyncNotification";

        /// <summary>
        /// The did fail up synchronize message key
        /// </summary>
        public const string DidFailUpSync = "DidFailUpSyncNotification";

        /// <summary>
        /// The did finish full synchronize message key
        /// </summary>
        public const string DidFinishFullSync = "DidFinishFullSyncNotification";

        /// <summary>
        /// The did finish incremental synchronize message key
        /// </summary>
        public const string DidFinishIncrementalSync = "DidFinishIncrementalSyncNotification";

        /// <summary>
        /// The did finish metadata synchronize message key
        /// </summary>
        public const string DidFinishMetadataSync = "DidFinishMetadataSyncNotification";

        /// <summary>
        /// The did finish resources synchronize message key
        /// </summary>
        public const string DidFinishResourcesSync = "DidFinishResourcesSyncNotification";

        /// <summary>
        /// The did finish single record synchronize message key
        /// </summary>
        public const string DidFinishSingleRecordSync = "DidFinishSingleRecordSyncNotification";

        /// <summary>
        /// The did finish up synchronize message key
        /// </summary>
        public const string DidFinishUpSync = "DidFinishUpSyncNotification";

        /// <summary>
        /// The did progress synchronize message key
        /// </summary>
        public const string DidProgressSync = "DidProgressSyncNotification";

        /// <summary>
        /// The did provide synchronize status hint message key
        /// </summary>
        public const string DidProvideSyncStatusHint = "DidProvideSyncStatusHintNotification";

        /// <summary>
        /// The did Start full synchronize message key
        /// </summary>
        public const string DidStartFullSync = "DidStartFullSyncNotification";

        /// <summary>
        /// The did Start incremental synchronize message key
        /// </summary>
        public const string DidStartIncrementalSync = "DidStartIncrementalSyncNotification";

        /// <summary>
        /// The did Start metadata synchronize message key
        /// </summary>
        public const string DidStartMetadataSync = "DidStartMetadataSyncNotification";

        /// <summary>
        /// The did Start resources synchronize message key
        /// </summary>
        public const string DidStartResourcesSync = "DidStartResourcesSyncNotification";

        /// <summary>
        /// The did Start single record synchronize message key
        /// </summary>
        public const string DidStartSingleRecordSync = "DidStartSingleRecordSyncNotification";

        /// <summary>
        /// The did Start up synchronize message key
        /// </summary>
        public const string DidStartUpSync = "DidStartUpSyncNotification";

        /// <summary>
        /// The did update records message key
        /// </summary>
        public const string DidUpdateRecords = "DidUpdateRecordsNotification";

        /// <summary>
        /// The number of conflicts changed message key
        /// </summary>
        public const string NumberOfConflictsChanged = "NumberOfConflictsChangedNotification";

        /// <summary>
        /// The did finish downloading all resources notification
        /// </summary>
        public const string DidFinishDownloadingAllResourcesNotification = "DidFinishDownloadingAllResourcesNotification";

        /// <summary>
        /// The did start processing download queue notification
        /// </summary>
        public const string DidStartProcessingDownloadQueueNotification = "DidStartProcessingDownloadQueueNotification";

        /// <summary>
        /// The did stop processing download queue notification
        /// </summary>
        public const string DidStopProcessingDownloadQueueNotification = "DidStopProcessingDownloadQueueNotification";

        /// <summary>
        /// The did finish downloading resource notification
        /// </summary>
        public const string DidFinishDownloadingResourceNotification = "DidFinishDownloadingResourceNotification";

        /// <summary>
        /// The did fail downloading resource notification
        /// </summary>
        public const string DidFailDownloadingResourceNotification = "DidFailDownloadingResourceNotification";

        /// <summary>
        /// The will cancel running download notification
        /// </summary>
        public const string WillCancelRunningDownloadNotification = "WillCancelRunningDownloadNotification";

        /// <summary>
        /// The will cancel all downloads notification
        /// </summary>
        public const string WillCancelAllDownloadsNotification = "WillCancelAllDownloadsNotification";

        /// <summary>
        /// Full sync failed
        /// </summary>
        public const string DidFailFullSyncNotification = "DidFailFullSyncNotification";

        /// <summary>
        /// Offline requests number changed
        /// </summary>
        public const string DidOfflineRequestsNumberChanged = "DidOfflineRequestsNumberChanged";

        /// <summary>
        /// Keeps session alive
        /// </summary>
        public const string DidKeepSessionAlive = "DidKeepSessionAlive";
    }

    /// <summary>
    /// A message that will update the title of the content page
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class SyncManagerMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the message key.
        /// </summary>
        /// <value>
        /// The message key.
        /// </value>
        public string MessageKey { get; private set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public Dictionary<string, object> State { get; private set; }

        /// <summary>
        /// Creates the specified message key.
        /// </summary>
        /// <param name="messageKey">
        /// The message key.
        /// </param>
        /// <returns>
        /// The <see cref="SyncManagerMessage"/>.
        /// </returns>
        public static SyncManagerMessage Create(string messageKey)
        {
            return new SyncManagerMessage { MessageKey = messageKey };
        }

        /// <summary>
        /// Creates the specified message key.
        /// </summary>
        /// <param name="messageKey">
        /// The message key.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// The <see cref="SyncManagerMessage"/>.
        /// </returns>
        public static SyncManagerMessage Create(string messageKey, Dictionary<string, object> state)
        {
            return new SyncManagerMessage { MessageKey = messageKey, State = state };
        }
    }
}
