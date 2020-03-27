// <copyright file="UriExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension method container for URI objects
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Extracts query string dictionary by given Uri object
        /// </summary>
        /// <param name="uri">Uri object</param>
        /// <returns>Dictionary{string,string} object</returns>
        public static Dictionary<string, string> ExtractQueryString(this Uri uri)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            string[] querySegments = uri.Query.Split('&');
            foreach (string segment in querySegments)
            {
                string[] parts = segment.Split('=');
                if (parts.Length > 1)
                {
                    string key = parts[0].Trim('?', ' ');
                    string val = parts[1].Trim();

                    queryParameters.Add(key, val);
                }
            }

            return queryParameters;
        }
    }
}
