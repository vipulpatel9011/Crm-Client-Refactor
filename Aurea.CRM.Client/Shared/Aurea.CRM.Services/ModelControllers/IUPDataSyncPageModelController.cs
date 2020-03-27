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
//   Data sync page controller interface
// </summary>
// -------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Services.ModelControllers
{
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// Synchronization page model controller interface
    /// </summary>
    public interface IUPDataSyncPageModelController
    {
        /// <summary>
        /// Gets data sync page
        /// </summary>
        IUPMDataSyncPage DataSyncPage { get; }

        /// <summary>
        /// Perform full synchronization
        /// </summary>
        void PerformFullSync();

        /// <summary>
        /// Perform metedata synchronization
        /// </summary>
        void PerformMetadataRefresh();

        /// <summary>
        /// Performa data synchronization
        /// </summary>
        void PerformDataRefresh();
    }
}
