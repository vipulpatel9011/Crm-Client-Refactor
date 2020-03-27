// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MStatus.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Basic status view implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Status
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Basic status view implementation
    /// </summary>
    /// <seealso cref="UPMElement" />
    public class UPMStatus : UPMElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMStatus"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMStatus(IIdentifier identifier)
            : base(identifier)
        {
            this.Children = new List<UPMStringField>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [blocks user interaction].
        /// </summary>
        /// <value>
        /// <c>true</c> if [blocks user interaction]; otherwise, <c>false</c>.
        /// </value>
        public bool BlocksUserInteraction { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        public List<UPMStringField> Children { get; set; }
    }
}
