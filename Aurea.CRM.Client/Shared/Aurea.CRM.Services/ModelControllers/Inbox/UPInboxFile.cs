// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPInboxFile.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   Inbox File
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Inbox
{
    using System;

    /// <summary>
    /// Inbox File
    /// </summary>
    public class UPInboxFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPInboxFile"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="url">The URL.</param>
        public UPInboxFile(string path, Uri url)
        {
            this.Path = path;
            this.URL = url;
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public Uri URL { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }
    }
}
