// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPOSRow.cs" company="Aurea Software Gmbh">
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
//   Serial Entry POS Row
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Serial Entry POS Row
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSERow" />
    public class UPSEPOSRow : UPSERow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERow"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="listing">The listing.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSEPOSRow(UPCRMResultRow resultRow, UPSEListing listing, UPSerialEntry serialEntry)
            : base(resultRow, listing, serialEntry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERow"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="tableIndex">Index of the table.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <param name="serialEntry">The serial entry.</param>
        /// <exception cref="Exception">RowRecordId is null</exception>
        public UPSEPOSRow(UPCRMResultRow resultRow, int tableIndex, int sourceFieldOffset, UPSerialEntry serialEntry)
            : base(resultRow, tableIndex, sourceFieldOffset, serialEntry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERow"/> class.
        /// </summary>
        /// <param name="sourceRow">The source row.</param>
        public UPSEPOSRow(UPSERow sourceRow)
            : base(sourceRow)
        {
        }
    }
}
