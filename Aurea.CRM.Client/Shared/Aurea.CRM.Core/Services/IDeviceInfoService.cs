// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeviceInfoService.cs" company="Aurea Software Gmbh">
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
//  Service to get device information
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Services
{
    /// <summary>
    /// Device info service interface
    /// </summary>
    public interface IDeviceInfoService
    {
        /// <summary>
        /// Returns current device model
        /// </summary>
        /// <returns>device model</returns>
        string GetDeviceModel();

        /// <summary>
        /// Returns operating system
        /// </summary>
        /// <returns>operating system</returns>
        string GetSystem();

        /// <summary>
        /// Returns application version
        /// </summary>
        /// <returns>application version</returns>
        string GetApplicationVersion();

        /// <summary>
        /// Returns screen width
        /// </summary>
        /// <returns>application screen width</returns>
        int GetApplicationScreenWidth();

        /// <summary>
        /// Set automatic screen lock state
        /// </summary>
        /// <param name="isEnabled">True if the automatic screen lock is enabled</param>
        /// <returns>The enabled state </returns>
        bool SetAutomaticScreenLock(bool isEnabled);
    }
}
