// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntryInfoRow.cs" company="Aurea Software Gmbh">
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
//   UPSerialEntryInfoRow
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// UPSerialEntryInfoRow
    /// </summary>
    public class UPSerialEntryInfoRow
    {
        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public List<string> Cells { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryInfoRow"/> class.
        /// </summary>
        /// <param name="cells">The cells.</param>
        /// <param name="result">The result.</param>
        public UPSerialEntryInfoRow(List<string> cells, UPSerialEntryInfoResult result)
        {
            this.Cells = cells;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryInfoRow"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        protected UPSerialEntryInfoRow(UPSerialEntryInfoResult result)
        {
        }
    }

    /// <summary>
    /// UPSerialEntryInfoRowFromCRMResultRow
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryInfoRow" />
    public class UPSerialEntryInfoRowFromCRMResultRow : UPSerialEntryInfoRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryInfoRowFromCRMResultRow"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="result">The result.</param>
        public UPSerialEntryInfoRowFromCRMResultRow(UPCRMResultRow row, UPSerialEntryInfoResult result)
            : base(result)
        {
            int cellCount = result.Info.ColumnNames.Count;
            List<string> cells = new List<string>(result.Info.ColumnNames.Count);
            for (int i = 0; i < cellCount; i++)
            {
                cells.Add(row.ValueAtIndex(i));
            }

            this.Cells = cells;
        }
    }
}
