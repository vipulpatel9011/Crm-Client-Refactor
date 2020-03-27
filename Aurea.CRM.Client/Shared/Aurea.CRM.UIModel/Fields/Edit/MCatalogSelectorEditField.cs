// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MCatalogSelectorEditField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field for catalog selector
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    ///  Edit field for catalog selector
    /// </summary>
    /// <seealso cref="UPMCatalogEditField" />
    public class UPMCatalogSelectorEditField : UPMCatalogEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCatalogSelectorEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMCatalogSelectorEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>
        /// The selector.
        /// </value>
        public UPSelector Selector { get; set; }
    }
}
