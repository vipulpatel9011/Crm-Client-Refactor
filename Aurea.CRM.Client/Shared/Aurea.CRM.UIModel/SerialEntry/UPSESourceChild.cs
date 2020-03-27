// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSESourceChild.cs" company="Aurea Software Gmbh">
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
//   UPSESourceChild
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// UPSESourceChild
    /// </summary>
    public class UPSESourceChild
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSESourceChild"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        public UPSESourceChild(UPCRMResultRow row)
        {
            this.Record = new UPCRMRecord(row.RootRecordIdentification);
            this.FieldValues = row.Values();
            this.RawFieldValues = row.RawValues();
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public UPCRMRecord Record { get; private set; }

        /// <summary>
        /// Gets the field values.
        /// </summary>
        /// <value>
        /// The field values.
        /// </value>
        public List<string> FieldValues { get; private set; }

        /// <summary>
        /// Gets the raw field values.
        /// </summary>
        /// <value>
        /// The raw field values.
        /// </value>
        public List<string> RawFieldValues { get; private set; }
    }
}
