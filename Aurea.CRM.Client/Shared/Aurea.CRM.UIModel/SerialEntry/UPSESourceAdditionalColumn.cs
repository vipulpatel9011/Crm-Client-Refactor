// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSESourceAdditionalColumn.cs" company="Aurea Software Gmbh">
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
//   UPSESourceAdditionalColumn
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// UPSESourceAdditionalColumn
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSESourceColumn" />
    public class UPSESourceAdditionalColumn : UPSESourceColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSESourceAdditionalColumn"/> class.
        /// </summary>
        /// <param name="addInfo">The add information.</param>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="index">The index.</param>
        /// <param name="positionInControl">The position in control.</param>
        /// <param name="keyColumn">The key column.</param>
        public UPSESourceAdditionalColumn(UPSESingleAdditionalItemInformation addInfo, UPConfigFieldControlField fieldConfig,
            int index, int positionInControl, UPSESourceColumn keyColumn)
            : base(fieldConfig, index, positionInControl)
        {
            this.KeyColumn = keyColumn;
            this.AddInfo = addInfo;
        }

        /// <summary>
        /// Gets the add information.
        /// </summary>
        /// <value>
        /// The add information.
        /// </value>
        public UPSESingleAdditionalItemInformation AddInfo { get; private set; }

        /// <summary>
        /// Gets the key column.
        /// </summary>
        /// <value>
        /// The key column.
        /// </value>
        public UPSESourceColumn KeyColumn { get; private set; }

        /// <summary>
        /// Gets the column from.
        /// </summary>
        /// <value>
        /// The column from.
        /// </value>
        public override UPSEColumnFrom ColumnFrom => UPSEColumnFrom.AdditionalSource;

        /// <summary>
        /// Raws the value for item key.
        /// </summary>
        /// <param name="itemKey">The item key.</param>
        /// <returns></returns>
        public string RawValueForItemKey(string itemKey)
        {
            return this.AddInfo.RawValueForItemKeyResultPosition(itemKey, this.PositionInControl);
        }

        /// <summary>
        /// Values for item key.
        /// </summary>
        /// <param name="itemKey">The item key.</param>
        /// <returns></returns>
        public string ValueForItemKey(string itemKey)
        {
            return this.AddInfo.ValueForItemKeyResultPosition(itemKey, this.PositionInControl);
        }
    }
}
