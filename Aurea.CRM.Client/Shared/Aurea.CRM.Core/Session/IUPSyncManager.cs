// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsBottomPanelViewModel.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Jakub Malczak
// </author>
// <summary>
//   Up sync manager interface
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Session
{
    /// <summary>
    /// Up sync manager interface
    /// </summary>
    public interface IUPSyncManager
    {
        /// <summary>
        /// Returns if incremental sync is running
        /// </summary>
        bool IncrementalSyncRunning { get; }

        /// <summary>
        /// Returns if metadata sync is running
        /// </summary>
        bool MetadataSyncRunning { get; }

        /// <summary>
        /// Returns if full sync is running
        /// </summary>
        bool FullSyncRunning { get; }
    }
}
