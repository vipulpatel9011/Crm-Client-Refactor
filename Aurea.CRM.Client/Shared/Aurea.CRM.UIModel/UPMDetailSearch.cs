// <copyright file="UPMDetailSearch.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Detail Search
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMDetailSearch : UPMElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDetailSearch"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMDetailSearch(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the tile field.
        /// </summary>
        /// <value>
        /// The tile field.
        /// </value>
        public UPMStringField TileField { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public AureaColor Color { get; set; }

        /// <summary>
        /// Gets or sets the switch to detail search action.
        /// </summary>
        /// <value>
        /// The switch to detail search action.
        /// </value>
        public UPMAction SwitchToDetailSearchAction { get; set; }
    }
}
