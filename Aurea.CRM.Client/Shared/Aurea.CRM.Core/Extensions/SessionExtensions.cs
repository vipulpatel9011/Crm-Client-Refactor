// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The session extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Session;

    /// <summary>
    /// The session extensions.
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Generates an Uri by creating and appending a querystring by given dictionary.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="queryStringDict">The query string dictionary.</param>
        /// <returns>
        /// The <see cref="Uri" />.
        /// </returns>
        public static Uri AppendUriWithQueryString(this Uri uri, Dictionary<string, string> queryStringDict)
        {
            var queryString = queryStringDict.ToQueryString();
            var uriString = uri.ToString();
            var completeUriString = uriString + queryString;

            return new Uri(completeUriString);
        }

        /// <summary>
        /// Creates a new document request uri by given Document key.
        /// </summary>
        /// <param name="serverSession">The server session.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The <see cref="Uri" />.
        /// </returns>
        public static Uri DocumentRequestUrlForDocumentKey(this IServerSession serverSession, string key)
        {
            Dictionary<string, string> parameterDictionary = new Dictionary<string, string> { { "Service", "Documents" }, { "DocumentKey", key } };

            return serverSession.CrmServer.MobileWebserviceUrl.AppendUriWithQueryString(parameterDictionary);
        }

        /// <summary>
        /// Creates a new document request uri by given Record Identification and Filename
        /// </summary>
        /// <param name="serverSession">The server session.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// Returns an <see cref="Uri" /> for document request.
        /// </returns>
        public static Uri DocumentRequestUrlForRecordIdentification(
            this IServerSession serverSession,
            string recordIdentification,
            string filename)
        {
            Dictionary<string, string> parameterDictionary;

            if (!string.IsNullOrEmpty(filename))
            {
                parameterDictionary = new Dictionary<string, string>
                                          {
                                              { "Service", "Documents" },
                                              { "InfoAreaId", recordIdentification.InfoAreaId() },
                                              { "RecordId", recordIdentification.RecordId() },
                                              { "FileName", filename }
                                          };
            }
            else
            {
                parameterDictionary = new Dictionary<string, string>
                                          {
                                              { "Service", "Documents" },
                                              { "InfoAreaId", recordIdentification.InfoAreaId() },
                                              { "RecordId", recordIdentification.RecordId() },
                                              { "FileName", filename }
                                          };
            }

            return serverSession.CrmServer.MobileWebserviceUrl.AppendUriWithQueryString(parameterDictionary);
        }
    }
}
