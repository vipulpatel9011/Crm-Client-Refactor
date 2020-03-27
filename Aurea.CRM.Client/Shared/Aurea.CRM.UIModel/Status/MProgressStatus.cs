// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MProgressStatus.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Status view with progress
    /// </summary>
    /// <seealso cref="UPMStatus" />
    public class UPMProgressStatus : UPMStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMProgressStatus"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMProgressStatus(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the status message field.
        /// </summary>
        /// <value>
        /// The status message field.
        /// </value>
        public UPMStringField StatusMessageField { get; set; }
    }
}
