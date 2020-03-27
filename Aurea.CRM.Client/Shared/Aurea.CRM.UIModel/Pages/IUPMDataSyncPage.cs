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
//   Data sync page interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Pages
{
    using System;

    /// <summary>
    /// Data sync page interface
    /// </summary>
    public interface IUPMDataSyncPage
    {
        /// <summary>
        /// Synchronization requirement status
        /// </summary>
        string FullSyncRequirementStatusText { get; }

        /// <summary>
        /// Last sync date
        /// </summary>
        DateTime? LastSyncDate { get; }
    }
}
