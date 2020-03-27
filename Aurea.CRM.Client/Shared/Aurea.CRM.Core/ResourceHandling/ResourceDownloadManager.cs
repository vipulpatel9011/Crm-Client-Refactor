// <copyright file="ResourceDownloadManager.cs" company="Aurea Software Gmbh">
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
    using System.Threading.Tasks;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Session;
    using GalaSoft.MvvmLight.Messaging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using OperationHandling;
    using Extensions;

    /// <summary>
    /// Implementation of resource download manager class
    /// </summary>
    public class ResourceDownloadManager : IRemoteDataDelegate
    {
        private const int BufferSize = 1024;
        private Stream currentDownloadFileHandle;
        private ResourceDownload currentResourceDownload;
        private IPlatformService platformService;
        private RemoteSession remoteSession;
        private bool serverSupportRangeHeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDownloadManager"/> class.
        /// </summary>
        /// <param name="session">Remote session</param>
        public ResourceDownloadManager(RemoteSession session)
        {
            this.ResourceDownloads = new List<ResourceDownload>();
            this.remoteSession = session;
            this.serverSupportRangeHeader = false;
        }

        /// <summary>
        /// Gets resource downloads
        /// </summary>
        public List<ResourceDownload> ResourceDownloads { get; }

        private IPlatformService PlatformService => this.platformService
                                                    ?? (this.platformService = SimpleIoc.Default.GetInstance<IPlatformService>());

        private IStorageProvider StorageService => this.PlatformService?.StorageProvider;

        /// <summary>
        /// Adds given resource to download list
        /// </summary>
        /// <param name="resourceDownload">Resource to download</param>
        public void AddResourceDownload(ResourceDownload resourceDownload)
        {
            resourceDownload.DownloadManager = this;
            if (this.ResourceDownloads.Contains(resourceDownload))
            {
                return;
            }

            this.ResourceDownloads.Add(resourceDownload);
            this.StartDownloadForFrontMostFile();
        }

        /// <summary>
        /// Cancels all downloads
        /// </summary>
        public void CancelAllDownloads()
        {
            Messenger.Default.Send(SyncManagerMessage.Create(SyncManagerMessageKey.WillCancelAllDownloadsNotification));

            this.currentResourceDownload?.CancelDownload();

            this.ResourceDownloads.Clear();
        }

        /// <summary>
        /// Cancels resource download
        /// </summary>
        /// <param name="resourceDownload">Resource download</param>
        /// <returns>Returns true if succeeds</returns>
        public bool CancelResourceDownload(ResourceDownload resourceDownload)
        {
            var cancelledDownload = false;

            if (this.currentDownloadFileHandle != null)
            {
                this.currentDownloadFileHandle.Flush();
                this.currentDownloadFileHandle.Dispose();
                this.currentDownloadFileHandle = null;
            }

            if (this.currentResourceDownload == resourceDownload)
            {
                if (this.serverSupportRangeHeader == false)
                {
                    Exception error;
                    this.StorageService.TryDelete(this.currentResourceDownload.LocalUrl, out error);
                }

                this.currentResourceDownload.Connection?.Cancel();
                this.currentResourceDownload.Connection = null;
                this.currentResourceDownload = null;
            }

            if (this.ResourceDownloads.Contains(resourceDownload))
            {
                cancelledDownload = true;
            }

            this.ResourceDownloads.Remove(resourceDownload);
            Messenger.Default.Send(SyncManagerMessage.Create(
                    SyncManagerMessageKey.WillCancelRunningDownloadNotification,
                    new Dictionary<string, object> { { "TheObject", resourceDownload } }));

            return cancelledDownload;
        }

        /// <summary>
        /// Moves given resource download list to front of the queue
        /// </summary>
        /// <param name="resourceDownloadsToMove">Resource download list</param>
        /// <returns>Returns true if succeeds</returns>
        public bool MoveResourceDownloadsToFrontOfQueue(List<ResourceDownload> resourceDownloadsToMove)
        {
            if (resourceDownloadsToMove.Count == 0)
            {
                return false;
            }

            var firstDownload = resourceDownloadsToMove[0];
            if (this.currentResourceDownload == firstDownload)
            {
                return false;
            }

            foreach (var download in resourceDownloadsToMove)
            {
                this.ResourceDownloads.Remove(download);
            }

            this.ResourceDownloads.InsertRange(0, resourceDownloadsToMove);

            if (this.currentDownloadFileHandle != null)
            {
                this.currentDownloadFileHandle.Flush();
                this.currentDownloadFileHandle.Dispose();
                this.currentDownloadFileHandle = null;
            }

            ResourceDownload cancelledDownload = this.currentResourceDownload;

            if (this.currentResourceDownload != null)
            {
                this.currentResourceDownload.Connection?.Cancel();
                this.currentResourceDownload.Connection = null;
                this.currentResourceDownload = null;
            }

            if (cancelledDownload != null)
            {
                Messenger.Default.Send(SyncManagerMessage.Create(
                        SyncManagerMessageKey.WillCancelRunningDownloadNotification,
                        new Dictionary<string, object> { { "TheObject", cancelledDownload } }));
            }

            this.StartDownloadForFrontMostFile();

            return true;
        }

        /// <summary>
        /// Moves given resource download to front of the queue
        /// </summary>
        /// <param name="resourceDownload">Resource download</param>
        /// <returns>Returns true if succeeds</returns>
        public bool MoveResourceDownloadToFrontOfQueue(ResourceDownload resourceDownload)
        {
            return this.MoveResourceDownloadsToFrontOfQueue(new List<ResourceDownload> { resourceDownload });
        }

        /// <inheritdoc/>
        public void RemoteDataDidFailLoadingWithError(RemoteData remoteData, Exception error)
        {
            this.currentResourceDownload.DownloadFailed();
            this.ContinueQueue();
        }

        /// <inheritdoc/>
        public void RemoteDataDidFinishLoading(RemoteData remoteData)
        {
            this.serverSupportRangeHeader = remoteData.ResponseHeaders.ValueOrDefault("Accept-Ranges") == "bytes";

            using (var writeStream = this.currentDownloadFileHandle)
            {
                writeStream.Write(remoteData.Data, 0, remoteData.Data.Length);
            }

            this.currentResourceDownload.DownloadFinished();
            this.ContinueQueue();
        }

        /// <inheritdoc/>
        public void RemoteDataProgressChanged(ulong progress, ulong total)
        {
            this.currentResourceDownload.NumberOfBytesDownloaded = progress;
            this.currentResourceDownload.NumberOfBytesToDownload = total;
            this.currentResourceDownload.DownloadProgress();
        }

        /// <summary>
        /// Returns resource download object for given uri
        /// </summary>
        /// <param name="downloadUrl">Download uri</param>
        /// <returns><see cref="ResourceDownload"/></returns>
        public ResourceDownload ResourceDownloadForUrl(Uri downloadUrl)
        {
            ResourceDownload download = null;

            foreach (var downloadItem in this.ResourceDownloads)
            {
                if (downloadItem.DownloadUrl.AbsoluteUri == downloadUrl.AbsoluteUri)
                {
                    download = downloadItem;
                    break;
                }
            }

            return download;
        }

        /// <summary>
        /// Starts processing download queue
        /// </summary>
        public void StartProcessingDownloadQueue()
        {
            this.StartDownloadForFrontMostFile();
        }

        /// <summary>
        /// Stop processing download queue
        /// </summary>
        public void StopProcessingDownloadQueue()
        {
            this.currentDownloadFileHandle?.Dispose();
            this.currentDownloadFileHandle = null;

            var currentDownload = this.currentResourceDownload;

            if (this.currentResourceDownload != null)
            {
                this.currentResourceDownload.Connection?.Cancel();
                this.currentResourceDownload.Connection = null;
                this.currentResourceDownload = null;
            }

            if (currentDownload != null)
            {
                Messenger.Default.Send(SyncManagerMessage.Create(
                       SyncManagerMessageKey.WillCancelRunningDownloadNotification,
                       new Dictionary<string, object> { { "TheObject", currentDownload } }));
            }
        }

        private void ContinueQueue()
        {
            this.ResourceDownloads.Remove(this.currentResourceDownload);
            this.currentResourceDownload.Connection = null;
            this.currentResourceDownload = null;

            this.StartDownloadForFrontMostFile();
        }

        private async void StartDownloadForFrontMostFile()
        {
            if (this.ResourceDownloads.Count == 0 || this.currentResourceDownload != null)
            {
                return;
            }

            this.StopProcessingDownloadQueue();

            this.currentResourceDownload = this.ResourceDownloads[0];
            ResourceDownload downloadToStart = this.currentResourceDownload;
            var remoteData = new RemoteData(this.currentResourceDownload.DownloadUrl, this.remoteSession, this);

            var userAgent = ServerSession.CurrentSession.CrmServer.UserAgent;
            if (userAgent?.Length > 0)
            {
                remoteData.CustomHttpHeaderFields["User-Agent"] = userAgent;
            }

            ulong? downloadOffset = null;
            var path = this.currentResourceDownload.LocalUrl;
            if (this.StorageService.FileExists(path) && this.currentResourceDownload.ShouldAutoResume && this.serverSupportRangeHeader)
            {
                this.currentDownloadFileHandle = await this.StorageService.GetFileStreamForWrite(path);
                this.currentDownloadFileHandle.Seek(this.currentDownloadFileHandle.Length, SeekOrigin.Begin);
                downloadOffset = await this.StorageService.GetFileSize(path);
            }
            else
            {
                if (this.StorageService.FileExists(path))
                {
                    Exception error;
                    this.StorageService.TryDelete(path, out error);
                }

                await this.StorageService.CreateFile(path);
                this.currentDownloadFileHandle = await this.StorageService.GetFileStreamForWrite(path);
            }

            if (downloadOffset.HasValue)
            {
                remoteData.CustomHttpHeaderFields["Range"] = $"bytes={downloadOffset}-";
                this.currentResourceDownload.NumberOfBytesDownloaded = (uint)downloadOffset.Value;
            }

            if (downloadToStart != null)
            {
                this.StartRequest(remoteData);
            }
        }

        private void StartRequest(RemoteData request)
        {
            if (this.remoteSession == null)
            {
                return;
            }

            this.currentResourceDownload.Connection = request;

            request.CookieStorage = this.remoteSession?.CookieStorage;
            request.Load();
        }
    }
}
