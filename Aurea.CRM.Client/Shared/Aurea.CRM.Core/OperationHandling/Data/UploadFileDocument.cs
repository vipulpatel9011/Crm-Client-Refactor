// <copyright file="UploadFileDocument.cs" company="Aurea Software Gmbh">
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
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Platform;

    /// <summary>
    /// Upload file document class implementation
    /// </summary>
    public class UploadFileDocument : UploadDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileDocument"/> class.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="filename">File name</param>
        /// <param name="mimeType">Mime type</param>
        public UploadFileDocument(string filePath, string filename, string mimeType)
            : base(filename, mimeType)
        {
            this.LocalPath = filePath;
        }

        /// <summary>
        /// Gets local path
        /// </summary>
        public string LocalPath { get; private set; }

        /// <inheritdoc />
        public override byte[] Data => SimpleIoc.Default.GetInstance<IPlatformService>()?.StorageProvider?.FileContents(this.LocalPath).GetAwaiter().GetResult();
    }
}
