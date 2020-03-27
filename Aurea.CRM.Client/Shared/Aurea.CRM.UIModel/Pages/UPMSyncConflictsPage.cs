// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSyncConflictsPage.cs" company="Aurea Software Gmbh">
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
//   Sync Conflict page implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Pages
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Sync Conflicts Page
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Pages.Page" />
    public class UPMSyncConflictsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMSyncConflictsPage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the synchronize conflict email.
        /// </summary>
        /// <value>
        /// The synchronize conflict email.
        /// </value>
        public string SyncConflictEmail { get; set; }

        /// <summary>
        /// Gets the number of synchronize conflicts.
        /// </summary>
        /// <value>
        /// The number of synchronize conflicts.
        /// </value>
        public int NumberOfSyncConflicts => this.Children.Count;

        /// <summary>
        /// Gets the synchronize conflicts.
        /// </summary>
        /// <value>
        /// The synchronize conflicts.
        /// </value>
        public List<UPMElement> SyncConflicts => this.Children;

        /// <summary>
        /// Synchronizes the index of the conflict at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMSyncConflict SyncConflictAtIndex(int index)
        {
            return index < this.Children.Count ? this.Children[index] as UPMSyncConflict : null;
        }

        /// <summary>
        /// Adds the synchronize conflict.
        /// </summary>
        /// <param name="syncConflict">The synchronize conflict.</param>
        public void AddSyncConflict(UPMSyncConflict syncConflict)
        {
            this.AddChild(syncConflict);
        }

        /// <summary>
        /// Removes the conflict.
        /// </summary>
        /// <param name="syncConflict">The synchronize conflict.</param>
        public void RemoveConflict(UPMSyncConflict syncConflict)
        {
            var conflict = this.Children.FirstOrDefault(x => x.Identifier.MatchesIdentifier(syncConflict.Identifier));

            if (conflict != null)
            {
                this.Children.Remove(conflict);
            }
        }
    }
}
