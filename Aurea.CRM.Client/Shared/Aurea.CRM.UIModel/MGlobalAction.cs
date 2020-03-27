// <copyright file="MGlobalAction.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Global Action
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMAction" />
    public class UPMGlobalAction : UPMAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGlobalAction"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMGlobalAction(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the additional label text.
        /// </summary>
        /// <value>
        /// The additional label text.
        /// </value>
        public string AdditionalLabelText { get; set; }
    }
}
