// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSyncConflictWithContext.cs" company="Aurea Software Gmbh">
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
//   Sync Conflict With Context implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.OfflineStorage;

    /// <summary>
    /// Sync Conflict with Context
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMSyncConflict" />
    public class UPMSyncConflictWithContext : UPMSyncConflict
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMContainer"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSyncConflictWithContext(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public UPOfflineRequest OfflineRequest { get; set; }
    }
}
