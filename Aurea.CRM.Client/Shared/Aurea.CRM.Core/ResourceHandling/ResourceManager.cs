// <copyright file="ResourceManager.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Session;

    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// The Resource download status.
    /// </summary>
    public enum ResourceDownloadStatus
    {
        /// <summary>
        /// Indicates pending status
        /// </summary>
        Pending,

        /// <summary>
        /// Indicates running status
        /// </summary>
        Running
    }

    /// <summary>
    /// Implementation of ResourceManager class
    /// </summary>
    public class ResourceManager
    {
        private readonly ResourceDownloadManager highPriorityDownloadManager;
        private readonly ResourceDownloadManager lowPriorityDownloadManager;
        private readonly RemoteSession remoteSession;
        private readonly IPlatformService platformService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager"/> class.
        /// </summary>
        /// <param name="rmtSession">Remote session</param>
        public ResourceManager(RemoteSession rmtSession)
        {
            this.remoteSession = rmtSession;
            this.highPriorityDownloadManager = new ResourceDownloadManager(this.remoteSession);
            this.lowPriorityDownloadManager = new ResourceDownloadManager(this.remoteSession);
            this.platformService = SimpleIoc.Default.GetInstance<IPlatformService>();
        }

        /// <summary>
        /// Gets local path for persistent resources
        /// </summary>
        public string LocalPathForPersistentResources
        {
            get
            {
                var transientResourcesDirectory = Path.Combine(this.LocalCachesDirectory, "Persistent");
                if (!this.platformService.StorageProvider.DirectoryExists(transientResourcesDirectory))
                {
                    this.platformService.StorageProvider.CreateDirectory(transientResourcesDirectory);
                }

                return transientResourcesDirectory;
            }
        }

        /// <summary>
        /// Gets local path for transient resources
        /// </summary>
        public string LocalPathForTransientResources
        {
            get
            {
                var transientResourcesDirectory = Path.Combine(this.LocalCachesDirectory, "Transient");
                if (!this.platformService.StorageProvider.DirectoryExists(transientResourcesDirectory))
                {
                    this.platformService.StorageProvider.CreateDirectory(transientResourcesDirectory);
                }

                return transientResourcesDirectory;
            }
        }

        /// <summary>
        /// Gets high priority download manager
        /// </summary>
        public ResourceDownloadManager HighPriorityDownloadManager => this.highPriorityDownloadManager;

        /// <summary>
        /// Gets low priority download manager
        /// </summary>
        public ResourceDownloadManager LowPriorityDownloadManager => this.lowPriorityDownloadManager;

        /// <summary>
        /// Gets session specific local cache path
        /// </summary>
        protected string LocalCachesDirectory => ServerSession.CurrentSession.SessionSpecificCachesPath;

        /// <summary>
        /// Stops processing of download queue
        /// </summary>
        public void StopProcessingDownloadQueue()
        {
            this.highPriorityDownloadManager.StopProcessingDownloadQueue();
            this.lowPriorityDownloadManager.StopProcessingDownloadQueue();
        }

        /// <summary>
        /// Starts processing of download queue
        /// </summary>
        public void StartProcessingDownloadQueue()
        {
            this.highPriorityDownloadManager.StartProcessingDownloadQueue();
            this.lowPriorityDownloadManager.StartProcessingDownloadQueue();
        }

        /// <summary>
        /// Cancels all downloads
        /// </summary>
        public void CancelAllDownloads()
        {
            this.highPriorityDownloadManager.CancelAllDownloads();
            this.lowPriorityDownloadManager.CancelAllDownloads();
        }

        /// <summary>
        /// Generates local path for resource url
        /// </summary>
        /// <param name="url">Url of resource</param>
        /// <param name="localBasePath">Local base path</param>
        /// <param name="localFileName">Local file name, this can be null</param>
        /// <returns>Local path of resource url</returns>
        public string LocalPathForResourceAtUrl(Uri url, string localBasePath, string localFileName)
        {
            if (url == null)
            {
                return null;
            }

            return Path.Combine(
                localBasePath,
                localFileName == null ? $"{url.AbsoluteUri.Md5String()}.crm" : $"{url.AbsoluteUri.Md5String()}{Path.GetExtension(localFileName)}");
        }

        /// <summary>
        /// Queues high priority download for resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="localFileName">Local file name</param>
        /// <param name="modificationDate">Modification date</param>
        /// <param name="shouldAutoResume">Should auto resume</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueHighPriorityDownloadForResourceAtUrl(Uri url, string localFileName, DateTime? modificationDate, bool shouldAutoResume)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, this.highPriorityDownloadManager, this.LocalPathForTransientResources, localFileName, null, modificationDate, shouldAutoResume);
        }

        /// <summary>
        /// Queues high priority download for resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="localFileName">Local file name</param>
        /// <param name="modificationDate">Modification date</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueHighPriorityDownloadForResourceAtUrl(Uri url, string localFileName, DateTime? modificationDate)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, this.highPriorityDownloadManager, this.LocalPathForTransientResources, localFileName, null, modificationDate);
        }

        /// <summary>
        /// Queues high priority download for resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="localFileName">Local file name</param>
        /// <param name="credentials">User credentials</param>
        /// <param name="modificationDate">Modification date</param>
        /// <param name="shouldAutoResume">Should auto resume</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueHighPriorityDownloadForResourceAtUrl(Uri url, string localFileName, NetworkCredential credentials, DateTime? modificationDate, bool shouldAutoResume)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, this.highPriorityDownloadManager, this.LocalPathForTransientResources, localFileName, credentials, modificationDate, shouldAutoResume);
        }

        /// <summary>
        /// Queues high priority download for resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="localFileName">Local file name</param>
        /// <param name="credentials">User credentials</param>
        /// <param name="modificationDate">Modification date</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueHighPriorityDownloadForResourceAtUrl(Uri url, string localFileName, NetworkCredential credentials, DateTime? modificationDate)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, this.highPriorityDownloadManager, this.LocalPathForTransientResources, localFileName, credentials, modificationDate);
        }

        /// <summary>
        /// Queues low priority download for resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="localFileName">Local file name</param>
        /// <param name="credentials">User credentials</param>
        /// <param name="modificationDate">Modification date</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueLowPriorityDownloadForResourceAtUrl(Uri url, string localFileName, NetworkCredential credentials, DateTime? modificationDate)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, this.lowPriorityDownloadManager, this.LocalPathForTransientResources, localFileName, credentials, modificationDate);
        }

        /// <summary>
        /// Queues low priority download for resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="localFileName">Local file name</param>
        /// <param name="modificationDate">Modification date</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueLowPriorityDownloadForResourceAtUrl(Uri url, string localFileName, DateTime? modificationDate)
        {
            return this.QueueLowPriorityDownloadForResourceAtUrl(url, localFileName, null, modificationDate);
        }

        /// <summary>
        /// Queues high priority download for resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="modificationDate">Modification date</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueHighPriorityDownloadForResourceAtUrl(Uri url, DateTime? modificationDate)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, this.highPriorityDownloadManager, this.LocalPathForTransientResources, null, null, modificationDate);
        }

        /// <summary>
        /// Queues low priority download for resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="modificationDate">Modification date</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueLowPriorityDownloadForResourceAtUrl(Uri url, DateTime? modificationDate)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, this.lowPriorityDownloadManager, this.LocalPathForTransientResources, null, null, modificationDate);
        }

        /// <summary>
        /// Queues high priority download for persistent resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="modificationDate">Modification date</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueHighPriorityDownloadForPersistentResourceAtUrl(Uri url, DateTime? modificationDate)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, this.highPriorityDownloadManager, this.LocalPathForPersistentResources, null, null, modificationDate);
        }

        /// <summary>
        /// Queues low priority download for persistent resource at url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="modificationDate">Modification date</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public virtual ResourceDownload QueueLowPriorityDownloadForPersistentResourceAtUrl(Uri url, DateTime? modificationDate)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, this.lowPriorityDownloadManager, this.LocalPathForPersistentResources, null, null, modificationDate);
        }

        /// <summary>
        /// Checks if has running download for resource at url
        /// </summary>
        /// <param name="url">Uri</param>
        /// <returns>If yes returns <see cref="ResourceDownload"/>, otherwise null</returns>
        public ResourceDownload HasRunningDownloadForResourceAtUrl(Uri url)
        {
            var existingDownload = this.highPriorityDownloadManager.ResourceDownloadForUrl(url);
            if (existingDownload == null)
            {
                existingDownload = this.lowPriorityDownloadManager.ResourceDownloadForUrl(url);
            }

            return existingDownload;
        }

        /// <summary>
        /// Checks if has local version of given resource uri
        /// </summary>
        /// <param name="url">Resource url</param>
        /// <param name="localFileName">Local file name</param>
        /// <returns>Returns true if has local version</returns>
        public bool HasLocalVersionOfResourceForUrl(Uri url, string localFileName)
        {
            var localUrl = this.LocalPathForResourceAtUrl(url, this.LocalPathForPersistentResources, localFileName);
            if (!this.platformService.StorageProvider.FileExists(localUrl))
            {
                localUrl = this.LocalPathForResourceAtUrl(url, this.LocalPathForPersistentResources, null);
                if (!this.platformService.StorageProvider.FileExists(localUrl))
                {
                    localUrl = this.LocalPathForResourceAtUrl(url, this.LocalPathForTransientResources, localFileName);
                    if (!this.platformService.StorageProvider.FileExists(localUrl))
                    {
                        localUrl = this.LocalPathForResourceAtUrl(url, this.LocalPathForTransientResources, null);
                    }
                }
            }

            return localUrl != null && this.platformService.StorageProvider.FileExists(localUrl);
        }

        /// <summary>
        /// HasLocalVersionOfResourceForDocument
        /// </summary>
        /// <param name="_document">document</param>
        /// <returns>true if local version</returns>
        public bool HasLocalVersionOfResourceForDocument(dynamic _document)
        {
            var localversion = false;
            if (_document.Url != null)
            {
                localversion = this.HasLocalVersionOfResourceForUrl(_document.Url, _document.LocalFileName);
            }

            if (!localversion)
            {
                if (_document.D1Url != null)
                {
                    localversion = this.HasLocalVersionOfResourceForUrl(_document.D1Url, _document.LocalFileName);
                }
            }

            return localversion;
        }
               
        /// <summary>
        /// Returns resource for given url
        /// </summary>
        /// <param name="url">Resource Url</param>
        /// <param name="localFileName">Local file name</param>
        /// <returns><see cref="Resource"/></returns>
        public Resource ResourceForUrl(Uri url, string localFileName)
        {
            var localUrl = this.LocalPathForResourceAtUrl(url, this.LocalPathForPersistentResources, localFileName);
            if (!this.platformService.StorageProvider.FileExists(localUrl))
            {
                localUrl = this.LocalPathForResourceAtUrl(url, this.LocalPathForPersistentResources, null);
                if (!this.platformService.StorageProvider.FileExists(localUrl))
                {
                    localUrl = this.LocalPathForResourceAtUrl(url, this.LocalPathForTransientResources, localFileName);
                    if (!this.platformService.StorageProvider.FileExists(localUrl))
                    {
                        localUrl = this.LocalPathForResourceAtUrl(url, this.LocalPathForTransientResources, null);
                    }
                }
            }

            if (this.platformService.StorageProvider.FileExists(localUrl))
            {
                return new Resource(url, localUrl);
            }

            return null;
        }

        /// <summary>
        /// ResourceForDocument
        /// </summary>
        /// <param name="_document">document</param>
        /// <returns>resource</returns>
        public Resource ResourceForDocument(dynamic _document)
        {
            Resource resource = null;
            if (_document.Url != null)
            {
                resource = this.ResourceForUrl(_document.Url, _document.LocalFileName);
            }

            if (resource == null)
            {
                if (_document.D1Url != null)
                {
                    resource = this.ResourceForUrl(_document.D1Url, _document.LocalFileName);
                }
            }

            return resource;
        }

        /// <summary>
        /// Deletes local version of resource
        /// </summary>
        /// <param name="url">Resource Url</param>
        /// <returns>Returns true if succeeds</returns>
        public bool DeleteLocalVersionOfResourceForUrl(Uri url)
        {
            var resource = this.ResourceForUrl(url, null);
            return resource.DeleteResource();
        }

        /// <summary>
        /// Prioritizes given download
        /// </summary>
        /// <param name="resourceDownload">Resource Download instance</param>
        /// <returns>Returns true if succeeds</returns>
        public bool PrioritizeDownload(ResourceDownload resourceDownload)
        {
            return resourceDownload.DownloadManager.MoveResourceDownloadToFrontOfQueue(resourceDownload);
        }

        /// <summary>
        /// Prioritizes download by given resource url
        /// </summary>
        /// <param name="url">Resource url</param>
        /// <returns>Returns true if succeeds</returns>
        public bool PrioritizeDownloadForResourceAtUrl(Uri url)
        {
            return this.PrioritizeDownload(this.HasRunningDownloadForResourceAtUrl(url));
        }

        /// <summary>
        /// Prioritizes given list of resource downloads
        /// </summary>
        /// <param name="resourceDownloads">List of resource downloads</param>
        /// <returns>Returns true if succeeds</returns>
        public bool PrioritizeDownloads(List<ResourceDownload> resourceDownloads)
        {
            var highPriorityDownloads = new List<ResourceDownload>();
            var lowPriorityDownloads = new List<ResourceDownload>();

            foreach (var download in resourceDownloads)
            {
                if (download.DownloadManager == this.highPriorityDownloadManager)
                {
                    highPriorityDownloads.Add(download);
                }
                else
                {
                    lowPriorityDownloads.Add(download);
                }
            }

            return this.highPriorityDownloadManager.MoveResourceDownloadsToFrontOfQueue(highPriorityDownloads) || this.lowPriorityDownloadManager.MoveResourceDownloadsToFrontOfQueue(lowPriorityDownloads);
        }

        /// <summary>
        /// Prioritizes downloads by given url list
        /// </summary>
        /// <param name="urls">List of resource urls</param>
        /// <returns>Returns true if succeeds</returns>
        public bool PrioritizeDownloadsForResourcesAtUrls(List<Uri> urls)
        {
            var downloads = new List<ResourceDownload>();

            foreach (var url in urls)
            {
                var download = this.HasRunningDownloadForResourceAtUrl(url);
                if (download != null)
                {
                    downloads.Add(download);
                }
            }

            return this.PrioritizeDownloads(downloads);
        }

        private ResourceDownload DownloadForResourceAtUrlLocalBasePathLocalFileNameCredentialsModificationDate(Uri url, string localBasePath, string localFileName, NetworkCredential credentials, DateTime? modificationDate)
        {
            var localUrl = this.LocalPathForResourceAtUrl(url, localBasePath, localFileName);
            ResourceDownload download = new ResourceDownload(url, localUrl, credentials, modificationDate);
            return download;
        }

        private ResourceDownload AttemptToQueueDownloadForResourceAtUrl(Uri url, ResourceDownloadManager downloadManager, string localBasePath, string localFileName, NetworkCredential credentials, DateTime? modificationDate, bool shouldAutoResume)
        {
            ResourceDownload existingDownload = this.HasRunningDownloadForResourceAtUrl(url);
            if (existingDownload != null)
            {
                return existingDownload;
            }

            ResourceDownload download = this.DownloadForResourceAtUrlLocalBasePathLocalFileNameCredentialsModificationDate(url, localBasePath, localFileName, credentials, modificationDate);
            download.ShouldAutoResume = shouldAutoResume;
            downloadManager.AddResourceDownload(download);
            return download;
        }

        private ResourceDownload AttemptToQueueDownloadForResourceAtUrl(Uri url, ResourceDownloadManager downloadManager, string localBasePath, string localFileName, NetworkCredential credentials, DateTime? modificationDate)
        {
            return this.AttemptToQueueDownloadForResourceAtUrl(url, downloadManager, localBasePath, localFileName, credentials, modificationDate, true);
        }
    }
}
