// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntryInfoResult.cs" company="Aurea Software Gmbh">
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
//   Serial Entry Position Info Message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// UPSerialEntryInfoResult
    /// </summary>
    public class UPSerialEntryInfoResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryInfoResult"/> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="info">The information.</param>
        public UPSerialEntryInfoResult(List<UPSerialEntryInfoRowFromCRMResultRow> rows, UPSerialEntryInfo info)
        {
            this.Rows = rows;
            this.Info = info;
        }

        protected UPSerialEntryInfoResult(UPSerialEntryInfo info)
        {
            this.Info = info;
        }

        /// <summary>
        /// Gets the column names.
        /// </summary>
        /// <value>
        /// The column names.
        /// </value>
        public List<string> ColumnNames => this.Info.ColumnNames;

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public List<UPSerialEntryInfoRowFromCRMResultRow> Rows { get; protected set; }

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <value>
        /// The information.
        /// </value>
        public UPSerialEntryInfo Info { get; }
    }

    /// <summary>
    /// UPSerialEntryInfoResultFromCRMResult
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryInfoResult" />
    public class UPSerialEntryInfoResultFromCRMResult : UPSerialEntryInfoResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryInfoResultFromCRMResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="info">The information.</param>
        public UPSerialEntryInfoResultFromCRMResult(UPCRMResult result, UPSerialEntrySourceRowInfo info)
            : base(info)
        {
            List<UPSerialEntryInfoRowFromCRMResultRow> rowArray = new List<UPSerialEntryInfoRowFromCRMResultRow>();
            int count = result.RowCount;
            if (count > info.MaxResults)
            {
                count = info.MaxResults;
            }

            for (int i = 0; i < count; i++)
            {
                UPSerialEntryInfoRowFromCRMResultRow row = new UPSerialEntryInfoRowFromCRMResultRow((UPCRMResultRow)result.ResultRowAtIndex(i), this);
                rowArray.Add(row);
            }

            this.Rows = rowArray;
        }
    }
}
