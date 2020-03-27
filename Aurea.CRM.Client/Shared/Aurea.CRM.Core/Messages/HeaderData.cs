// <copyright file="HeaderData.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Serdar Tepeyurt
// </author>
namespace Aurea.CRM.Core.Messages
{
    using System;

    /// <summary>
    /// Implementation of header data
    /// </summary>
    public class HeaderData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderData"/> class.
        /// </summary>
        public HeaderData()
        {
        }

        private HeaderData(HeaderData data)
        {
            this.Title = data?.Title;
            this.ImageUri = data?.ImageUri;
            this.Detail = data?.Detail;
            this.ImageVisible = data?.ImageVisible ?? false;
            this.IsSingleLine = data?.IsSingleLine ?? false;
        }

        /// <summary>
        /// Gets or sets header title text
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets header detail text
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// Gets or sets header image Uri
        /// </summary>
        public Uri ImageUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether image is visible
        /// </summary>
        public bool ImageVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is single line
        /// </summary>
        public bool IsSingleLine { get; set; }

        /// <summary>
        /// Creates and returns a new instance of <see cref="HeaderData"/> by given old data and new <see cref="HeaderBarMessage"/>
        /// </summary>
        /// <param name="data">Old data</param>
        /// <param name="message">Message, also includes new data</param>
        /// <returns><see cref="HeaderData"/></returns>
        public static HeaderData CreateNew(HeaderData data, HeaderBarMessage message)
        {
            var headerData = new HeaderData(data);
            if (message.IsTitleUpdate)
            {
                headerData.Title = message.Data.Title;
                return headerData;
            }

            headerData = message.Data;

            return headerData;
        }
    }
}
