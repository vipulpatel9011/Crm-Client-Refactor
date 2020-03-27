// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmRecordWithParameter.cs" company="Aurea Software Gmbh">
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
//   CRM Record With Parameter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    /// <summary>
    /// CRM Record With Parameter
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.DataModel.UPCRMRecord" />
    public class UPCRMRecordWithParameter : UPCRMRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordWithParameter"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="recordId">The record identifier.</param>
        public UPCRMRecordWithParameter(string infoAreaId, string recordId)
            : base(infoAreaId, recordId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordWithParameter"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>        
        public UPCRMRecordWithParameter(string infoAreaId)
        {
            this.InfoAreaId = infoAreaId;
            this.RecordId = null;
            //this.RecordIdentification = null;
            //this.OriginalRecordIdentification = null;
            this.Mode = "New";
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has values; otherwise, <c>false</c>.
        /// </value>
        public bool HasValues { get; set; }
    }
}
