// <copyright file="ResourceDownload.cs" company="Aurea Software Gmbh">
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
    using System.Net;
    using Session;

    /// <summary>
    /// Resource Download
    /// </summary>
    public class ResourceDownload
    {
        private double progress;

        /// <summary>
        /// Event handler for download finished event
        /// </summary>
        public event EventHandler DownloadFinishedEvent;

        /// <summary>
        /// Event handler for download failed event
        /// </summary>
        public event EventHandler DownloadFailedEvent;

        /// <summary>
        /// Event handler for download finished event
        /// </summary>
        public event EventHandler DownloadProgressEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDownload"/> class.
        /// </summary>
        /// <param name="downloadURL">Download uri</param>
        /// <param name="localURL">Local uri</param>
        /// <param name="credentials">Credentials</param>
        /// <param name="modificationDate">Modification date</param>
        public ResourceDownload(Uri downloadURL, string localURL, NetworkCredential credentials, DateTime? modificationDate)
        {
            this.DownloadUrl = downloadURL;
            this.LocalUrl = localURL;
            this.NumberOfBytesToDownload = 0;
            this.NumberOfBytesDownloaded = 0;
            this.ShouldAutoResume = true;
            this.Credentials = credentials;
            this.ModificationDate = modificationDate;
        }

        /// <summary>
        /// Gets the download url.
        /// </summary>
        public Uri DownloadUrl { get; private set; }

        /// <summary>
        /// Gets the local url.
        /// </summary>
        public string LocalUrl { get; private set; }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        public double Progress
        {
            get
            {
                this.progress = (double)this.NumberOfBytesDownloaded / (double)this.NumberOfBytesToDownload;

                if (this.progress < 0)
                {
                    this.progress = 0;
                }

                if (this.progress > 1)
                {
                    this.progress = 1;
                }

                return this.progress;
            }
        }

        /// <summary>
        /// Gets readable size to download
        /// </summary>
        public ResourceSize ReadableSizeToDownload => new ResourceSize(this.NumberOfBytesToDownload);

        /// <summary>
        /// Gets readable size downloaded
        /// </summary>
        public ResourceSize ReadableSizeDownloaded => new ResourceSize(this.NumberOfBytesDownloaded);

        /// <summary>
        /// Gets or sets number of bytes to download
        /// </summary>
        public ulong NumberOfBytesToDownload { get; set; }

        /// <summary>
        /// Gets or sets number of bytes downloaded
        /// </summary>
        public ulong NumberOfBytesDownloaded { get; set; }

        /// <summary>
        /// Gets a value indicating whether download can be cancelled
        /// </summary>
        public bool CanCancelDownload => true;

        /// <summary>
        /// Gets or sets a value indicating whether should auto resume
        /// </summary>
        public bool ShouldAutoResume { get; set; }

        /// <summary>
        /// Gets or sets download manager
        /// </summary>
        public ResourceDownloadManager DownloadManager { get; set; }

        /// <summary>
        /// Gets connection
        /// </summary>
        public RemoteData Connection { get; set; }

        /// <summary>
        /// Gets finished status
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// Gets credentials
        /// </summary>
        public NetworkCredential Credentials { get; private set; }

        /// <summary>
        /// Gets modification date
        /// </summary>
        public DateTime? ModificationDate { get; private set; }

        /// <summary>
        /// Cancels download
        /// </summary>
        /// <returns>Returns true if succeeds</returns>
        public bool CancelDownload()
        {
            if (this.CanCancelDownload == false)
            {
                return false;
            }

            return this.PerformCancel();
        }

        /// <summary>
        /// Handles download finished event
        /// </summary>
        public void DownloadFinished()
        {
            this.Finished = true;
            this.DownloadFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles download failed event
        /// </summary>
        public void DownloadFailed()
        {
            this.Finished = true;
            this.DownloadFailedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles download progress event
        /// </summary>
        public void DownloadProgress()
        {
            this.DownloadProgressEvent?.Invoke(this, EventArgs.Empty);
        }

        private bool PerformCancel()
        {
            return this.DownloadManager.CancelResourceDownload(this);
        }
    }
}