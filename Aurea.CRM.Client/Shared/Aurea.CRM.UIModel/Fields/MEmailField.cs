// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MEmailField.cs" company="Aurea Software Gmbh">
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
//   UI control for showing an email value
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// UI control for showing an email value
    /// </summary>
    /// <seealso cref="UPMStringField" />
    public class UPMEmailField : UPMStringField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMEmailField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMEmailField(IIdentifier identifier, UPMAction action = null)
            : base(identifier)
        {
            this.Action = action;
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public UPMAction Action { get; }
    }
}
