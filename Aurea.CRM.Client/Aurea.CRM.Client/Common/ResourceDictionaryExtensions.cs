// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionaryExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Ioan Armenean (Nelutu)
// </author>
// <summary>
//   Extensions for the ResourceDictionary class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Client.UI.Common
{
    using System.Linq;
    using Xamarin.Forms;

    /// <summary>
    /// Extensions for the ResourceDictionary class
    /// </summary>
    public static class ResourceDictionaryExtensions
    {
        /// <summary>
        /// Gets style with given key
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="dict">Dictionary</param>
        /// <param name="key">Key</param>
        /// <returns>The style specified by the key parameter</returns>
        public static TResult GetStyle<TResult>(this ResourceDictionary dict, string key)
        {
            var result = default(TResult);

            if (!string.IsNullOrEmpty(key as string))
            {
                var containerDict = dict.MergedDictionaries.FirstOrDefault(a => a.ContainsKey(key ?? string.Empty));
                if (containerDict != null)
                {
                    result = (TResult)containerDict[key];
                }
            }

            return result;
        }
    }
}
