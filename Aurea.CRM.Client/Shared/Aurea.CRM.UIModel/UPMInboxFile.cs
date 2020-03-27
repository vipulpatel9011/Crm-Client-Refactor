// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMInboxFile.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.UIModel
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Inbox File
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMInboxFile : UPMElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMInboxFile"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMInboxFile(IIdentifier identifier)
            : base(identifier)
        {
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

        /// <summary>
        /// Gets or sets the MIME tye.
        /// </summary>
        /// <value>
        /// The MIME tye.
        /// </value>
        public string MimeTye { get; set; }

        /// <summary>
        /// Gets or sets the formatted date.
        /// </summary>
        /// <value>
        /// The formatted date.
        /// </value>
        public string FormattedDate { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the size of the formatted.
        /// </summary>
        /// <value>
        /// The size of the formatted.
        /// </value>
        public string FormattedSize { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public long Size { get; set; }

        //public UIImage Icon { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public AureaColor Color { get; set; }
    }
}
