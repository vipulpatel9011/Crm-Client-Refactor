// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmParentRecordReference.cs" company="Aurea Software Gmbh">
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
//   CRM parent record reference class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    /// <summary>
    /// CRM parent record reference
    /// </summary>
    public class UPCRMParentRecordReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMParentRecordReference"/> class.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        public UPCRMParentRecordReference(UPCRMRecordWithHierarchy record)
        {
            this.Record = record;
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public UPCRMRecordWithHierarchy Record { get; private set; }
    }
}
