// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCharacteristicsItem.cs" company="Aurea Software Gmbh">
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
//   The UPMCharacteristics Item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Characteristics
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// UPMCharacteristicsItem
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMCharacteristicsItem : UPMElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCharacteristicsItem"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMCharacteristicsItem(IIdentifier identifier)
            : base(identifier)
        {
            this.EditFields = new List<UPMField>();
        }

        /// <summary>
        /// Gets or sets the item name field.
        /// </summary>
        /// <value>
        /// The item name field.
        /// </value>
        public UPMStringField ItemNameField { get; set; }

        /// <summary>
        /// Gets or sets the selected field.
        /// </summary>
        /// <value>
        /// The selected field.
        /// </value>
        public UPMBooleanEditField SelectedField { get; set; }

        /// <summary>
        /// Gets or sets the edit fields.
        /// </summary>
        /// <value>
        /// The edit fields.
        /// </value>
        public List<UPMField> EditFields { get; set; }
    }
}
