// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MUserInteractionRequiredStatus.cs" company="Aurea Software Gmbh">
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
    /// Status where user interaction is required
    /// </summary>
    /// <seealso cref="UPMStatus" />
    public class UPMUserInteractionRequiredStatus : UPMStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMUserInteractionRequiredStatus"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMUserInteractionRequiredStatus(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the message field.
        /// </summary>
        /// <value>
        /// The message field.
        /// </value>
        public UPMStringField MessageField { get; set; }

        /// <summary>
        /// Gets or sets the supported user actions.
        /// </summary>
        /// <value>
        /// The supported user actions.
        /// </value>
        public List<object> SupportedUserActions { get; set; }
    }
}
