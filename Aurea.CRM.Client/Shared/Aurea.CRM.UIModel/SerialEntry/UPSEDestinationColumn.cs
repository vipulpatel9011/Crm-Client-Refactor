// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEDestinationColumn.cs" company="Aurea Software Gmbh">
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
//   UPSEDestinationColumn
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// UPSEDestinationColumn
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEDestinationColumnBase" />
    public class UPSEDestinationColumn : UPSEDestinationColumnBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEDestinationColumn"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="index">The index.</param>
        /// <param name="parentColumnIndex">Index of the parent column.</param>
        /// <param name="positionInControl">The position in control.</param>
        /// <param name="destinationInfoAreaId">The destination information area identifier.</param>
        public UPSEDestinationColumn(UPConfigFieldControlField fieldConfig, int index, int parentColumnIndex, int positionInControl, string destinationInfoAreaId)
            : base(fieldConfig, index, parentColumnIndex, positionInControl, destinationInfoAreaId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEDestinationColumn"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="column">The column.</param>
        public UPSEDestinationColumn(UPConfigFieldControlField fieldConfig, UPSEDestinationColumnBase column)
            : base(fieldConfig, column)
        {
        }

        /// <summary>
        /// Gets the column from.
        /// </summary>
        /// <value>
        /// The column from.
        /// </value>
        public override UPSEColumnFrom ColumnFrom => UPSEColumnFrom.Dest;
    }
}
