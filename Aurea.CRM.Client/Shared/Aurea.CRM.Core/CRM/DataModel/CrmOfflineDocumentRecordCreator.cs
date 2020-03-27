// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmOfflineDocumentRecordCreator.cs" company="Aurea Software Gmbh">
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
//   The Offline Document Record Creator
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Offline Document Record Creator
    /// </summary>
    public class UPCRMOfflineDocumentRecordCreator
    {
        /// <summary>
        /// Gets the record link identifier.
        /// </summary>
        /// <value>
        /// The record link identifier.
        /// </value>
        public int RecordLinkId { get; private set; }

        /// <summary>
        /// Gets the document link link identifier.
        /// </summary>
        /// <value>
        /// The document link link identifier.
        /// </value>
        public int DocumentLinkLinkId { get; private set; }

        /// <summary>
        /// Gets the name of the document link template filter.
        /// </summary>
        /// <value>
        /// The name of the document link template filter.
        /// </value>
        public string DocumentLinkTemplateFilterName { get; private set; }

        /// <summary>
        /// Gets the name of the document template filter.
        /// </summary>
        /// <value>
        /// The name of the document template filter.
        /// </value>
        public string DocumentTemplateFilterName { get; private set; }

        /// <summary>
        /// Gets the template filter.
        /// </summary>
        /// <value>
        /// The template filter.
        /// </value>
        public UPConfigFilter TemplateFilter { get; private set; }

        /// <summary>
        /// Gets the link template filter.
        /// </summary>
        /// <value>
        /// The link template filter.
        /// </value>
        public UPConfigFilter LinkTemplateFilter { get; private set; }

        /// <summary>
        /// Gets the document link information area identifier.
        /// </summary>
        /// <value>
        /// The document link information area identifier.
        /// </value>
        public string DocumentLinkInfoAreaId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMOfflineDocumentRecordCreator"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="noD3Link">The no d3 link.</param>
        public UPCRMOfflineDocumentRecordCreator(ViewReference viewReference, string noD3Link)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string val = viewReference.ContextValueForKey("DocumentTemplateFilter");
            if (!string.IsNullOrEmpty(val))
            {
                this.TemplateFilter = configStore.FilterByName(val);
                if (this.TemplateFilter != null)
                {
                    this.TemplateFilter = this.TemplateFilter.FilterByApplyingDefaultReplacements();
                }
            }

            val = viewReference.ContextValueForKey("DocumentLinkTemplateFilter");
            if (!string.IsNullOrEmpty(val))
            {
                this.LinkTemplateFilter = configStore.FilterByName(val);
                if (this.LinkTemplateFilter != null)
                {
                    this.LinkTemplateFilter = this.LinkTemplateFilter.FilterByApplyingDefaultReplacements();
                }
            }

            val = viewReference.ContextValueForKey("RecordLinkId");
            this.RecordLinkId = !string.IsNullOrEmpty(val) ? Convert.ToInt32(val) : 126;

            val = viewReference.ContextValueForKey("DocumentLinkLinkId");
            if (!string.IsNullOrEmpty(val))
            {
                this.DocumentLinkLinkId = Convert.ToInt32(val);
            }
            else
            {
                this.DocumentLinkLinkId = -1;
            }

            if (string.IsNullOrEmpty(noD3Link))
            {
                this.DocumentLinkInfoAreaId = viewReference.ContextValueForKey("DocumentLinkInfoAreaId");
                if (!string.IsNullOrEmpty(this.DocumentLinkInfoAreaId))
                {
                    if (this.DocumentLinkInfoAreaId == "nolink")
                    {
                        this.DocumentLinkInfoAreaId = null;
                    }
                }
                else
                {
                    this.DocumentLinkInfoAreaId = "D3";
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMOfflineDocumentRecordCreator"/> class.
        /// </summary>
        /// <param name="noD3Link">The no d3 link.</param>
        /// <returns>A new instance of the <see cref="UPCRMOfflineDocumentRecordCreator"/> class.</returns>
        public static UPCRMOfflineDocumentRecordCreator Create(string noD3Link = null)
        {
            Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName("Configuration:OfflineDocumentCreation");
            if (menu?.ViewReference == null)
            {
                return null;
            }

            return new UPCRMOfflineDocumentRecordCreator(menu.ViewReference, noD3Link);
        }

        /// <summary>
        /// Parameterses for document.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="dateString">The date string.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public Dictionary<string, object> ParametersForDocument(string fileName, string mimeType, string dateString, int size)
        {
            return new Dictionary<string, object> { { "MimeType", mimeType }, { "Date", dateString }, { "FileName", fileName }, { "Size", size } };
        }

        /// <summary>
        /// CRMs the records for parameters record add root.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="record">The record.</param>
        /// <param name="addRoot">if set to <c>true</c> [add root].</param>
        /// <returns></returns>
        public List<UPCRMRecord> CrmRecordsForParametersRecordAddRoot(Dictionary<string, object> parameters, UPCRMRecord record, bool addRoot)
        {
            List<UPCRMRecord> recordArray = new List<UPCRMRecord>();
            UPCRMRecord documentRecord = null;
            if (this.TemplateFilter != null)
            {
                UPConfigFilter filter = this.TemplateFilter.FilterByApplyingValueDictionary(parameters);
                if (filter != null)
                {
                    documentRecord = UPCRMRecord.CreateNew(filter.InfoAreaId);
                    documentRecord.Mode = "NewOffline";
                    documentRecord.ApplyValuesFromTemplateFilter(filter);
                    recordArray.Add(documentRecord);
                }
            }

            if (!string.IsNullOrEmpty(this.DocumentLinkInfoAreaId) && documentRecord != null)
            {
                UPCRMRecord documentLinkRecord = UPCRMRecord.CreateNew(this.DocumentLinkInfoAreaId);
                documentLinkRecord.Mode = "NewOffline";
                UPConfigFilter filter = this.LinkTemplateFilter?.FilterByApplyingValueDictionary(parameters);
                if (filter != null)
                {
                    documentLinkRecord.ApplyValuesFromTemplateFilter(filter);
                }

                documentLinkRecord.AddLink(new UPCRMLink(record, this.RecordLinkId));
                documentLinkRecord.AddLink(new UPCRMLink(documentRecord, this.DocumentLinkLinkId));
                recordArray.Add(documentLinkRecord);
            }
            else if (string.IsNullOrEmpty(this.DocumentLinkInfoAreaId))
            {
                documentRecord?.AddLink(new UPCRMLink(record, this.RecordLinkId));
            }

            if (addRoot && recordArray.Count > 0)
            {
                recordArray.Add(record);
            }

            return recordArray.Count > 0 ? recordArray : null;
        }
    }
}
