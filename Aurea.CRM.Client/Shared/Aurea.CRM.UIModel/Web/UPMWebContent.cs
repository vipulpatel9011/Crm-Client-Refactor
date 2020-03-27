// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMWebContent.cs" company="Aurea Software Gmbh">
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
//   The Web Content Interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using System;

    /// <summary>
    /// The UPMWebContent interface.
    /// </summary>
    public interface UPMWebContent
    {
        /// <summary>
        /// Gets or sets the web content url.
        /// </summary>
        Uri WebContentUrl { get; set; }

        /// <summary>
        /// Gets or sets the web content html.
        /// </summary>
        string WebContentHtml { get; set; }

        /// <summary>
        /// Gets or sets the report type.
        /// </summary>
        string ReportType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether load content manually.
        /// </summary>
        bool LoadContentManually { get; set; }
    }
}
