// <copyright file="DocumentData.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Document data class implementation
// </summary>

namespace Aurea.CRM.Core.CRM.Features
{
    using System;

    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.ResourceHandling;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Document data.
    /// </summary>
    public class DocumentData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentData"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="mimeType">
        /// The mime type.
        /// </param>
        /// <param name="documentDate">
        /// The document date.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="serverUpdateDate">
        /// The server update date.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <param name="dateString">
        /// The date string.
        /// </param>
        /// <param name="d1RecordIdentification">
        /// The d 1 record identification.
        /// </param>
        public DocumentData(
            string recordIdentification,
            string title,
            string mimeType,
            DateTime? documentDate,
            ulong length,
            DateTime? serverUpdateDate,
            string displayText,
            string dateString,
            string d1RecordIdentification)
        {
            this.Title = title;
            this.MimeType = mimeType;
            this.DocumentDate = documentDate;
            this.Length = length;
            this.ServerUpdateDate = serverUpdateDate;
            this.RecordIdentification = recordIdentification;
            this.DisplayText = displayText;
            this.IconName = string.Empty;
            this.DateString = dateString;
            this.D1RecordIdentification = d1RecordIdentification;
        }

        /// <summary>
        /// Gets the d 1 record identification.
        /// </summary>
        public string D1RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the date string.
        /// </summary>
        public string DateString { get; private set; }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        public string DisplayText { get; private set; }

        /// <summary>
        /// Gets the document date.
        /// </summary>
        public DateTime? DocumentDate { get; private set; }

        /// <summary>
        /// Gets or sets the icon name.
        /// </summary>
        public string IconName { get; set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public ulong Length { get; private set; }

        /// <summary>
        /// Gets the mime type.
        /// </summary>
        public string MimeType { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the server update date.
        /// </summary>
        public DateTime? ServerUpdateDate { get; private set; }

        /// <summary>
        /// Gets the size string.
        /// </summary>
        public string SizeString
        {
            get
            {
                var size = new ResourceSize(this.Length);
                var unit = size.SuggestedUnit();
                var unitString = ResourceSize.RenderableForUnit(unit);
                return $"{size.ValueConvertedTo(unit).ToString("F1")} {unitString}";
            }
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The url.
        /// </summary>
        public Uri Url => ServerSession.CurrentSession.DocumentRequestUrlForRecordIdentification(this.RecordIdentification, this.Title);

        /// <summary>
        /// The url for d 1 record id.
        /// </summary>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        public Uri UrlForD1RecordId()
        {
            if (!string.IsNullOrEmpty(this.D1RecordIdentification))
            {
                return ServerSession.CurrentSession.DocumentRequestUrlForRecordIdentification(this.D1RecordIdentification, this.Title);
            }

            return null;
        }
    }
}
