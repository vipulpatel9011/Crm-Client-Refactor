// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualInfoAreaCacheForInfoArea.cs" company="Aurea Software Gmbh">
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
//   Virtual info area cache for given info area
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Virtual info area cache for given info area
    /// </summary>
    public class UPVirtualInfoAreaCacheForInfoArea
    {
        /// <summary>
        /// The record to information area identifier
        /// </summary>
        private readonly Dictionary<string, string> recordToInfoAreaId;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPVirtualInfoAreaCacheForInfoArea"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public UPVirtualInfoAreaCacheForInfoArea(string infoAreaId)
        {
            this.InfoAreaId = infoAreaId;
            this.recordToInfoAreaId = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Adds the record identifier information area identifier.
        /// </summary>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <param name="virtualInfoAreaId">
        /// The virtual information area identifier.
        /// </param>
        public virtual void AddRecordIdInfoAreaId(string recordId, string virtualInfoAreaId)
        {
            if (!string.IsNullOrEmpty(recordId))
            {
                this.recordToInfoAreaId.SetObjectForKey(virtualInfoAreaId, recordId);
            }
        }

        /// <summary>
        /// Virtuals the information area identifier for record identifier.
        /// </summary>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string VirtualInfoAreaIdForRecordId(string recordId)
        {
            var virtualInfoAreaId = this.recordToInfoAreaId.ValueOrDefault(recordId);
            if (virtualInfoAreaId == null)
            {
                return this.InfoAreaId;
            }

            return UPCRMDataStore.DefaultStore.IsValidInfoArea(virtualInfoAreaId) ? virtualInfoAreaId : this.InfoAreaId;
        }
    }
}
