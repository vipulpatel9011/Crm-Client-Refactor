// <copyright file="UploadDataDocument.cs" company="Aurea Software Gmbh">
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
    /// Upload data document class implementation
    /// </summary>
    public class UploadDataDocument : UploadDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadDataDocument"/> class.
        /// </summary>
        /// <param name="documentData">Documents data</param>
        /// <param name="filename">File name</param>
        /// <param name="mimeType">Mime type</param>
        public UploadDataDocument(byte[] documentData, string filename, string mimeType)
            : base(filename, mimeType)
        {
            this.DocumentData = documentData;
        }

        /// <summary>
        /// Gets document data
        /// </summary>
        public byte[] DocumentData { get; private set; }

        /// <inheritdoc />
        public override byte[] Data => this.DocumentData;
    }
}
