// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMDataSyncPage.cs" company="Aurea Software Gmbh">
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
//   Data Sync page implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Pages
{
    using System;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Data Sync Page
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Pages.Page" />
    public class UPMDataSyncPage : Page, IUPMDataSyncPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDataSyncPage"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMDataSyncPage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the last synchronize date.
        /// </summary>
        /// <value>
        /// The last synchronize date.
        /// </value>
        public DateTime? LastSyncDate { get; set; }

        /// <summary>
        /// Gets or sets the current synchronize message.
        /// </summary>
        /// <value>
        /// The current synchronize message.
        /// </value>
        public string CurrentSyncMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can start incremental synchronize.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can start incremental synchronize; otherwise, <c>false</c>.
        /// </value>
        public bool CanStartIncrementalSync { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can perform configuration synchronize.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can perform configuration synchronize; otherwise, <c>false</c>.
        /// </value>
        public bool CanPerformConfigurationSync { get; set; }

        /// <summary>
        /// Gets or sets the full synchronize requirement status text.
        /// </summary>
        /// <value>
        /// The full synchronize requirement status text.
        /// </value>
        public string FullSyncRequirementStatusText { get; set; }

        /// <summary>
        /// Gets or sets the full synchronize requirement status.
        /// </summary>
        /// <value>
        /// The full synchronize requirement status.
        /// </value>
        public FullSyncRequirementStatus FullSyncRequirementStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can perform language change.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can perform language change; otherwise, <c>false</c>.
        /// </value>
        public bool CanPerformLanguageChange { get; set; }
    }
}
