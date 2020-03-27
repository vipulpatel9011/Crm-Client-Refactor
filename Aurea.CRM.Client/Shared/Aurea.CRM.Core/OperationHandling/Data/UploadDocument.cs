// <copyright file="UploadDocument.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.OperationHandling.Data
{
    /// <summary>
    /// Upload document class implementation
    /// </summary>
    public class UploadDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadDocument"/> class.
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="mimeType">Mime type</param>
        public UploadDocument(string filename, string mimeType)
        {
            this.Filename = filename;
            this.MimeType = mimeType;
        }

        /// <summary>
        /// Gets data
        /// </summary>
        public virtual byte[] Data => null;

        /// <summary>
        /// Gets mime type
        /// </summary>
        public string MimeType { get; private set; }

        /// <summary>
        /// Gets file name
        /// </summary>
        public string Filename { get; private set; }
    }
}
