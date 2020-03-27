// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMWebContentPage.cs" company="Aurea Software Gmbh">
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
//   The Web Content page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Aurea.CRM.UIModel.Web;

namespace Aurea.CRM.UIModel.Pages
{
    using System;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Web Content Page
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Pages.Page" />
    /// <seealso cref="UPMWebContent" />
    public class UPMWebContentPage : Page, UPMWebContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMWebContentPage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the web content data.
        /// </summary>
        /// <value>
        /// The web content data.
        /// </value>
        public byte[] WebContentData { get; set; }

        /// <summary>
        /// Gets or sets the web content url.
        /// </summary>
        public Uri WebContentUrl { get; set; }

        /// <summary>
        /// Gets or sets the web content html.
        /// </summary>
        public string WebContentHtml { get; set; }

        /// <summary>
        /// Gets or sets the report type.
        /// </summary>
        public string ReportType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether load content manually.
        /// </summary>
        public bool LoadContentManually { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [print enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [print enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool PrintEnabled { get; set; }
    }
}
