// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncDocumentDownloadFieldGroupUrlCache.cs" company="Aurea Software Gmbh">
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
//   Sync Document Download Field Group URL Cache
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Common
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Sync document URl cache
    /// </summary>
    public class UPSyncDocumentDownloadFieldGroupUrlCache
    {
        /// <summary>
        /// The CRM result
        /// </summary>
        private readonly UPCRMResult result;

        /// <summary>
        /// The result dictionary
        /// </summary>
        private readonly Dictionary<string, ICrmDataSourceRow> resultDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDocumentDownloadFieldGroupUrlCache"/> class.
        /// </summary>
        /// <param name="fieldGroupName">
        /// Name of the field group.
        /// </param>
        public UPSyncDocumentDownloadFieldGroupUrlCache(string fieldGroupName)
        {
            this.FieldControl = ConfigurationUnitStore.DefaultStore?.FieldControlByNameFromGroup("List", fieldGroupName);
            if (this.FieldControl != null)
            {
                var crmQuery = new UPContainerMetaInfo(this.FieldControl);
                this.result = crmQuery.Find();
                var count = this.result?.RowCount ?? 0;
                this.resultDictionary = new Dictionary<string, ICrmDataSourceRow>(count);
                for (var i = 0; i < count; i++)
                {
                    var row = this.result?.ResultRowAtIndex(i);
                    var rid = row?.RootRecordId;
                    if (!string.IsNullOrEmpty(rid))
                    {
                        this.resultDictionary[rid] = row;
                    }
                }
            }

            var fieldMapping = this.FieldControl?.FunctionNames();
            var field = fieldMapping?.ValueOrDefault("UpdDate");
            this.ServerModifyDateFieldIndex = field?.TabIndependentFieldIndex ?? -1;

            field = fieldMapping?.ValueOrDefault("UpdTime");
            this.ServerModifyTimeFieldIndex = field?.TabIndependentFieldIndex ?? -1;

            this.HasServerDateTime = this.ServerModifyDateFieldIndex >= 0 && this.ServerModifyTimeFieldIndex >= 0;
        }

        /// <summary>
        /// Gets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has server date time.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has server date time; otherwise, <c>false</c>.
        /// </value>
        public bool HasServerDateTime { get; private set; }

        /// <summary>
        /// Gets the index of the server modify date field.
        /// </summary>
        /// <value>
        /// The index of the server modify date field.
        /// </value>
        public int ServerModifyDateFieldIndex { get; private set; }

        /// <summary>
        /// Gets the index of the server modify time field.
        /// </summary>
        /// <value>
        /// The index of the server modify time field.
        /// </value>
        public int ServerModifyTimeFieldIndex { get; private set; }

        /// <summary>
        /// Rows for record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// the result row
        /// </returns>
        public UPCRMResultRow RowForRecordIdentification(string recordIdentification)
        {
            return this.resultDictionary?.ValueOrDefault(recordIdentification) as UPCRMResultRow;
        }
    }
}
