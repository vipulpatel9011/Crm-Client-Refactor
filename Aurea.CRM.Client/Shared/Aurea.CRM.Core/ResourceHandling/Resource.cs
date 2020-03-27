// <copyright file="Resource.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.ResourceHandling
{
    using System;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Platform;

    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Resource class implementation
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="localUrl">
        /// The local url.
        /// </param>
        public Resource(Uri url, string localUrl)
        {
            this.Url = url;
            this.LocalUrl = localUrl;
        }

        /// <summary>
        /// Gets content type
        /// </summary>
        public string MimeType => SimpleIoc.Default.GetInstance<IStorageProvider>().GetFileContentType(this.LocalUrl).GetAwaiter().GetResult();

        /// <summary>
        /// Gets resource data
        /// </summary>
        public byte[] ResourceData => SimpleIoc.Default.GetInstance<IStorageProvider>().FileContents(this.LocalUrl).GetAwaiter().GetResult();

        /// <summary>
        /// Gets length of resource data
        /// </summary>
        public ulong NumberOfBytes => SimpleIoc.Default.GetInstance<IStorageProvider>().GetFileSize(this.LocalUrl).GetAwaiter().GetResult();

        /// <summary>
        /// Gets readable size of resource
        /// </summary>
        public ResourceSize ReadableSize => new ResourceSize(this.NumberOfBytes);

        /// <summary>
        /// Gets original creation date of resource
        /// </summary>
        public DateTime OriginalCreationDate => SimpleIoc.Default.GetInstance<IStorageProvider>().GetFileCreatedDate(this.LocalUrl).GetAwaiter().GetResult();

        /// <summary>
        /// Gets last modification date of resource
        /// </summary>
        public DateTime LastModificationDate => SimpleIoc.Default.GetInstance<IStorageProvider>().GetFileLastModified(this.LocalUrl).GetAwaiter().GetResult();

        /// <summary>
        /// Gets the url.
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// Gets the local url.
        /// </summary>
        public string LocalUrl { get; private set; }

        /// <summary>
        /// Deletes resource
        /// </summary>
        /// <returns>Returns true if succeeds</returns>
        public bool DeleteResource()
        {
            Exception error = null;
            var result = SimpleIoc.Default.GetInstance<IStorageProvider>().TryDelete(this.LocalUrl, out error);
            if (error != null)
            {
                ConfigurationUnitStore.Logger.LogError(error.Message);
            }

            return result;
        }
    }
}
