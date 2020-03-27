// <copyright file="ResourceManagerExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.Extensions
{
    using Aurea.CRM.UIModel;
    using Core.ResourceHandling;

    /// <summary>
    /// Resource manager extensions
    /// </summary>
    public static class ResourceManagerExtensions
    {
        /// <summary>
        /// The has local version of resource for document.
        /// </summary>
        /// <param name="resourceManager">
        /// The resource manager.
        /// </param>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <returns>
        /// <see cref="bool"/>.
        /// </returns>
        public static bool HasLocalVersionOfResourceForDocument(
            this ResourceManager resourceManager,
            UPMDocument document)
        {
            var localVersion = false;
            if (document.Url != null)
            {
                localVersion = resourceManager.HasLocalVersionOfResourceForUrl(document.Url, document.LocalFileName);
            }

            if (!localVersion)
            {
                if (document.D1Url != null)
                {
                    localVersion = resourceManager.HasLocalVersionOfResourceForUrl(
                        document.D1Url,
                        document.LocalFileName);
                }
            }

            return localVersion;
        }

        /// <summary>
        /// The resource for document.
        /// </summary>
        /// <param name="resourceManager">
        /// The resource manager.
        /// </param>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <returns>
        /// The <see cref="UPResource"/>.
        /// </returns>
        public static Resource ResourceForDocument(this ResourceManager resourceManager, UPMDocument document)
        {
            Resource resource = null;
            if (document.Url != null)
            {
                resource = resourceManager.ResourceForUrl(document.Url, document.LocalFileName);
            }

            if (resource == null)
            {
                if (document.D1Url != null)
                {
                    resource = resourceManager.ResourceForUrl(document.D1Url, document.LocalFileName);
                }
            }

            return resource;
        }
    }
}