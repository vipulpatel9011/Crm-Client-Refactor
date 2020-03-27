// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncDocumentDownloadUrlCache.cs" company="Aurea Software Gmbh">
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
//   The Sync Document Download Url Cache class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Common
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Sync Document Download Url Cache
    /// </summary>
    public class UPSyncDocumentDownloadUrlCache
    {
        /// <summary>
        /// The field group caches
        /// </summary>
        private Dictionary<string, UPSyncDocumentDownloadFieldGroupUrlCache> fieldGroupCaches;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDocumentDownloadUrlCache"/> class.
        /// </summary>
        public UPSyncDocumentDownloadUrlCache()
        {
            this.fieldGroupCaches = new Dictionary<string, UPSyncDocumentDownloadFieldGroupUrlCache>();
        }

        /// <summary>
        /// Fields the group URL cache for field group.
        /// </summary>
        /// <param name="fieldGroupName">
        /// Name of the field group.
        /// </param>
        /// <returns>
        /// Field grou download cache
        /// </returns>
        public UPSyncDocumentDownloadFieldGroupUrlCache FieldGroupUrlCacheForFieldGroup(string fieldGroupName)
        {
            var cache = this.fieldGroupCaches.ValueOrDefault(fieldGroupName);
            if (cache != null)
            {
                return cache;
            }

            cache = new UPSyncDocumentDownloadFieldGroupUrlCache(fieldGroupName);
            this.fieldGroupCaches[fieldGroupName] = cache;

            return cache;
        }
    }
}
