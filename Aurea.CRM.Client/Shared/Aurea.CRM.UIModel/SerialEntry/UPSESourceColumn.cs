// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSESourceColumn.cs" company="Aurea Software Gmbh">
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
//   UPSESourceColumn
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// UPSESourceColumn
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEColumn" />
    public class UPSESourceColumn : UPSEColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSESourceColumn"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="index">The index.</param>
        /// <param name="positionInControl">The position in control.</param>
        public UPSESourceColumn(UPConfigFieldControlField fieldConfig, int index, int positionInControl)
            : base(fieldConfig, index, positionInControl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSESourceColumn"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="column">The column.</param>
        public UPSESourceColumn(UPConfigFieldControlField fieldConfig, UPSEColumn column)
            : base(fieldConfig, column)
        {
        }

        /// <summary>
        /// Gets the column from.
        /// </summary>
        /// <value>
        /// The column from.
        /// </value>
        public override UPSEColumnFrom ColumnFrom => UPSEColumnFrom.Source;
    }
}
