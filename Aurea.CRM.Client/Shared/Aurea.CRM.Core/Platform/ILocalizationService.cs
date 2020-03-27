// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocalizationService.cs" company="Aurea Software Gmbh">
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
//   exposes the platform specific localization strings
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Platform
{
    /// <summary>
    /// exposes the platform specific localization strings
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// the localized text
        /// </returns>
        string GetString(string key, string defaultValue);
    }
}
