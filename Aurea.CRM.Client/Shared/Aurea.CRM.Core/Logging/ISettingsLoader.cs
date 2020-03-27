// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISettingsLoader.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Defines the ISettingsLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Logging
{
    /// <summary>
    /// The SettingsLoader interface.
    /// </summary>
    public interface ISettingsLoader
    {
        /// <summary>
        /// The get bool from storage.
        /// </summary>
        /// <param name="settingStr">
        /// The setting str.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool GetBoolFromStorage(string settingStr);

        /// <summary>
        /// The get string from storage.
        /// </summary>
        /// <param name="settingStr">
        /// The setting str.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetStringFromStorage(string settingStr);
    }
}
