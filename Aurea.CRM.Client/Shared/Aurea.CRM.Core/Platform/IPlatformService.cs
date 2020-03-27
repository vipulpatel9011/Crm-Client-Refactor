// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlatformService.cs" company="Aurea Software Gmbh">
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
//   Defines the Platform specific interfaces
//   Each Platform will inject the implementations for these services
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Platform
{
    using System;

    /// <summary>
    /// Defines the Platform specific interfaces
    /// Each Platform will inject the implementations for these services
    /// </summary>
    public interface IPlatformService
    {
        /// <summary>
        /// Gets the localization service.
        /// </summary>
        /// <value>
        /// The localization service.
        /// </value>
        ILocalizationService LocalizationService { get; }

        /// <summary>
        /// Gets the storage provider.
        /// </summary>
        /// <value>
        /// The storage provider.
        /// </value>
        IStorageProvider StorageProvider { get; }

        /// <summary>
        /// Logout the application.
        /// </summary>
        /// <param name="terminate">
        /// if set to <c>true</c> [terminate] the application.
        /// </param>
        void ApplicationLogout(bool terminate);

        /// <summary>
        /// Clears the page navigation back stack.
        /// </summary>
        void ClearBackStack();

        /// <summary>
        /// Runs the action in main thread.
        /// </summary>
        /// <param name="action">The action.</param>
        void RunInMainThread(Action action);
    }
}
