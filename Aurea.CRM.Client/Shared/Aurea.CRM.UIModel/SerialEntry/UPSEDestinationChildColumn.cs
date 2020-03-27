// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEDestinationChildColumn.cs" company="Aurea Software Gmbh">
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
//   UPSEDestinationChildColumn
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// UPSEDestinationChildColumn
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEDestinationColumnBase" />
    public class UPSEDestinationChildColumn : UPSEDestinationColumnBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEDestinationChildColumn"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="index">The index.</param>
        /// <param name="parentColumnIndex">Index of the parent column.</param>
        /// <param name="positionInControl">The position in control.</param>
        /// <param name="childIndex">Index of the child.</param>
        /// <param name="destinationInfoAreaId">The destination information area identifier.</param>
        public UPSEDestinationChildColumn(UPConfigFieldControlField fieldConfig, int index, int parentColumnIndex,
            int positionInControl, int childIndex, string destinationInfoAreaId)
            : base(fieldConfig, index, parentColumnIndex, positionInControl, destinationInfoAreaId)
        {
            this.ChildIndex = childIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEDestinationChildColumn"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="column">The column.</param>
        public UPSEDestinationChildColumn(UPConfigFieldControlField fieldConfig, UPSEDestinationChildColumn column)
            : base(fieldConfig, column)
        {
            this.ChildIndex = column.ChildIndex;
        }

        /// <summary>
        /// Gets the index of the child.
        /// </summary>
        /// <value>
        /// The index of the child.
        /// </value>
        public int ChildIndex { get; private set; }

        /// <summary>
        /// Gets the column from.
        /// </summary>
        /// <value>
        /// The column from.
        /// </value>
        public override UPSEColumnFrom ColumnFrom => UPSEColumnFrom.DestChild;
    }
}
