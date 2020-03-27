// <copyright file="SmartbookResourceManager.cs" company="Aurea Software Gmbh">
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

    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Implementation of Smartbook Resource Manager
    /// </summary>
    public class SmartbookResourceManager : ResourceManager
    {
        private readonly ServerSession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartbookResourceManager"/> class.
        /// </summary>
        /// <param name="serverSession">Server session</param>
        public SmartbookResourceManager(ServerSession serverSession)
            : base(serverSession.RemoteSession)
        {
            this.session = serverSession;
        }

        /// <summary>
        /// Gets default resource manager
        /// </summary>
        public static ResourceManager DefaultResourceManager => ServerSession.CurrentSession.ResourceManager;

        /// <inheritdoc />
        public override ResourceDownload QueueHighPriorityDownloadForResourceAtUrl(Uri url, string localFileName, DateTime? modificationDate, bool shouldAutoResume)
        {
            return this.QueueHighPriorityDownloadForResourceAtUrl(url, localFileName, this.session.SessionCredentials, modificationDate, shouldAutoResume);
        }

        /// <inheritdoc />
        public override ResourceDownload QueueHighPriorityDownloadForResourceAtUrl(Uri url, string localFileName, DateTime? modificationDate)
        {
            return this.QueueHighPriorityDownloadForResourceAtUrl(url, localFileName, this.session.SessionCredentials, modificationDate);
        }

        /// <inheritdoc />
        public override ResourceDownload QueueLowPriorityDownloadForResourceAtUrl(Uri url, string localFileName, DateTime? modificationDate)
        {
            return this.QueueLowPriorityDownloadForResourceAtUrl(url, localFileName, this.session.SessionCredentials, modificationDate);
        }
    }
}
