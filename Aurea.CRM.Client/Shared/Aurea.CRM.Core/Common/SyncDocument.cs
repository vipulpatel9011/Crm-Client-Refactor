// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncDocument.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   The Sync Document class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Common
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.ResourceHandling;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Document sync related implementation
    /// </summary>
    public class SyncDocument
    {
        /// <summary>
        /// The download urls
        /// </summary>
        private List<Uri> downloadUrls;

        /// <summary>
        /// The download ur ls intitialized
        /// </summary>
        private bool downloadUrLsIntitialized;

        /// <summary>
        /// Gets the name of the field group.
        /// </summary>
        /// <value>
        /// The name of the field group.
        /// </value>
        private string FieldGroupName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [document from fields].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [document from fields]; otherwise, <c>false</c>.
        /// </value>
        private bool DocumentFromFields { get; set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        private string RecordIdentification { get; }

        /// <summary>
        /// Gets the document manager
        /// </summary>
        private DocumentManager DocumentManager { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncDocument"/> class.
        /// </summary>
        /// <param name="recordIdentification">Record identification</param>
        /// <param name="fieldGroupName">Field group name</param>
        /// <param name="documentManager">Document manager</param>
        public SyncDocument(string recordIdentification, string fieldGroupName, DocumentManager documentManager)
        {
            this.FieldGroupName = fieldGroupName;
            this.RecordIdentification = recordIdentification;
            this.DocumentManager = documentManager;
            var infoAreaId = this.RecordIdentification.InfoAreaId();
            this.DocumentFromFields = !(Equals(infoAreaId, "D3") || Equals(infoAreaId, "D1") || Equals(infoAreaId, "D2"));
        }

        /// <summary>
        /// Downloads the ur ls for document.
        /// </summary>
        /// <param name="urlCache">The URL cache.</param>
        /// <returns></returns>
        public List<Uri> DownloadUrlsForDocument(UPSyncDocumentDownloadUrlCache urlCache)
        {
            if (this.downloadUrLsIntitialized)
            {
                return this.downloadUrls;
            }

            this.downloadUrLsIntitialized = true;
            UPCRMResultRow row;
            FieldControl fieldControl;
            if (urlCache != null)
            {
                var fieldGroupCache = urlCache.FieldGroupUrlCacheForFieldGroup(this.FieldGroupName);
                fieldControl = fieldGroupCache.FieldControl;
                row = fieldGroupCache.RowForRecordIdentification(this.RecordIdentification);
            }
            else
            {
                fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", this.FieldGroupName);
                if (fieldControl == null)
                {
                    return null;
                }

                var crmQuery = new UPContainerMetaInfo(fieldControl);
                var result = crmQuery.ReadRecord(this.RecordIdentification);
                if (result.RowCount == 0)
                {
                    return null;
                }

                row = (UPCRMResultRow)result.ResultRowAtIndex(0);
            }

            if (this.DocumentFromFields)
            {
                var documentDownloadURLs = new List<Uri>();
                foreach (var tab in fieldControl.Tabs)
                {
                    foreach (var field in tab.Fields)
                    {
                        string value = row.ValueAtIndex(field.TabIndependentFieldIndex);
                        if (!string.IsNullOrEmpty(value))
                        {
                            var documentData = this.DocumentManager.DocumentForKey(value);
                            Uri downloadURL = documentData != null ?
                                ServerSession.CurrentSession.DocumentRequestUrlForRecordIdentification(documentData.RecordIdentification, documentData.Title) :
                                ServerSession.CurrentSession.DocumentRequestUrlForDocumentKey(value);

                            if (SmartbookResourceManager.DefaultResourceManager.ResourceForUrl(downloadURL, null) == null)
                            {
                                documentDownloadURLs.Add(downloadURL);
                            }
                        }
                    }
                }

                this.downloadUrls = documentDownloadURLs;
                return documentDownloadURLs;
            }

            Uri downloadUrl = ServerSession.CurrentSession.DocumentRequestUrlForRecordIdentification(this.RecordIdentification, row.ValueAtIndex(1));
            if (SmartbookResourceManager.DefaultResourceManager.ResourceForUrl(downloadUrl, null) == null)
            {
                this.downloadUrls = new List<Uri> { downloadUrl };
                return this.downloadUrls;
            }

            return null;
        }

        /// <summary>
        /// Files the name for document.
        /// </summary>
        /// <param name="urlCache">
        /// The URL cache.
        /// </param>
        /// <returns>
        /// file name
        /// </returns>
        public string FileNameForDocument(UPSyncDocumentDownloadUrlCache urlCache)
        {
            UPCRMResultRow row;
            if (urlCache != null)
            {
                var fieldGroupCache = urlCache.FieldGroupUrlCacheForFieldGroup(this.FieldGroupName);
                row = fieldGroupCache.RowForRecordIdentification(this.RecordIdentification);
            }
            else
            {
                FieldControl fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", this.FieldGroupName);
                if (fieldControl == null)
                {
                    return null;
                }

                var crmQuery = new UPContainerMetaInfo(fieldControl);
                var result = crmQuery.ReadRecord(this.RecordIdentification);
                if (result.RowCount == 0)
                {
                    return null;
                }

                row = (UPCRMResultRow)result.ResultRowAtIndex(0);
            }

            // FUNCTION NAME "Title"
            return row.ValueAtIndex(1);
        }

        /// <summary>
        /// Starts the download.
        /// </summary>
        /// <returns>downloaded file counf</returns>
        public int StartDownload()
        {
            var fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup(
                "List",
                this.FieldGroupName);
            if (fieldControl == null)
            {
                return 0;
            }

            var crmQuery = new UPContainerMetaInfo(fieldControl);
            var result = crmQuery.ReadRecord(this.RecordIdentification);
            if (result.RowCount == 0)
            {
                return 0;
            }

            var resourceManager = SmartbookResourceManager.DefaultResourceManager;
            var currentSession = ServerSession.CurrentSession;
            var row = result.ResultRowAtIndex(0);

            if (this.DocumentFromFields)
            {
                var count = 0;
                foreach (var tab in fieldControl.Tabs)
                {
                    foreach (var field in tab.Fields)
                    {
                        string value = row.ValueAtIndex(field.TabIndependentFieldIndex);
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }

                        ++count;
                        var docData = this.DocumentManager.DocumentForKey(value);
                        if (docData != null)
                        {
                            resourceManager.QueueLowPriorityDownloadForResourceAtUrl(
                                currentSession.DocumentRequestUrlForRecordIdentification(this.RecordIdentification, docData.Title), docData.ServerUpdateDate);
                        }
                        else
                        {
                            resourceManager.QueueLowPriorityDownloadForResourceAtUrl(currentSession.DocumentRequestUrlForDocumentKey(value), null);
                        }
                    }
                }

                return count;
            }

            var documentInfoAreaManager = new DocumentInfoAreaManager(fieldControl.InfoAreaId, fieldControl, null);
            var documentData = documentInfoAreaManager.DocumentDataForResultRow(row as UPCRMResultRow);
            resourceManager.QueueLowPriorityDownloadForResourceAtUrl(
                currentSession.DocumentRequestUrlForRecordIdentification(this.RecordIdentification, documentData.Title), documentData.ServerUpdateDate);

            return 1;
        }

        /// <summary>
        /// Servers the modification date.
        /// </summary>
        /// <param name="urlCache">
        /// The URL cache.
        /// </param>
        /// <returns>
        /// when the record last upated
        /// </returns>
        public DateTime? ServerModificationDate(UPSyncDocumentDownloadUrlCache urlCache)
        {
            if (this.DocumentFromFields)
            {
                return null;
            }

            if (urlCache != null)
            {
                var fieldGroupCache = urlCache.FieldGroupUrlCacheForFieldGroup(this.FieldGroupName);
                var row = fieldGroupCache.RowForRecordIdentification(this.RecordIdentification);
                if (fieldGroupCache.HasServerDateTime && row != null)
                {
                    string updateDateString = row.RawValueAtIndex(fieldGroupCache.ServerModifyDateFieldIndex);
                    string updateTimeString = row.RawValueAtIndex(fieldGroupCache.ServerModifyTimeFieldIndex);
                    return !string.IsNullOrEmpty(updateDateString)
                        ? (DateTime?)UPCRMTimeZone.Current.DateFromClientDataMMDateStringTimeString(updateDateString, updateTimeString)
                        : null;
                }
            }

            var documentData = this.DocumentManager.DocumentDataForRecordIdentification(this.RecordIdentification);
            return documentData?.ServerUpdateDate;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.FieldGroupName}: {this.RecordIdentification}";
        }
    }
}
