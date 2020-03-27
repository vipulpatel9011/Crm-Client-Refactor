// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSerialEntrySelectorCatalogEditField.cs" company="Aurea Software Gmbh">
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
//   The UPMSerialEntrySelectorCatalogEditField.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.SerialEntry;

    /// <summary>
    /// UPMSerialEntrySelectorCatalogEditField
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Fields.SerialEntry.UPMSerialEntryCatalogEditField" />
    public class UPMSerialEntrySelectorCatalogEditField : UPMSerialEntryCatalogEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSerialEntrySelectorCatalogEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="column">The column.</param>
        /// <param name="position">The position.</param>
        public UPMSerialEntrySelectorCatalogEditField(IIdentifier identifier, UPSEColumn column, UPMSEPosition position)
            : base(identifier, column, position)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSerialEntrySelectorCatalogEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="column">The column.</param>
        /// <param name="position">The position.</param>
        /// <param name="multiSelectMode">if set to <c>true</c> [multi select mode].</param>
        public UPMSerialEntrySelectorCatalogEditField(IIdentifier identifier, UPSEColumn column, UPMSEPosition position, bool multiSelectMode)
            : base(identifier, column, position, multiSelectMode)
        {
        }

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>
        /// The selector.
        /// </value>
        public UPSelector Selector { get; set; }

        /// <summary>
        /// Gets or sets the selector options.
        /// </summary>
        /// <value>
        /// The selector options.
        /// </value>
        public Dictionary<string, UPSelectorOption> SelectorOptions { get; set; }

        /// <summary>
        /// Options for name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public UPSelectorOption OptionForName(string name)
        {
            return this.SelectorOptions[name];
        }
    }
}
