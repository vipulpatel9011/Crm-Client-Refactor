// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMWebContentGroup.cs" company="Aurea Software Gmbh">
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
//   The Web Content Group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Web;

    public class UPMWebContentGroup : UPMGroup, UPMWebContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMWebContentGroup"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMWebContentGroup(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether load content manually.
        /// </summary>
        public bool LoadContentManually { get; set; }

        /// <summary>
        /// Gets or sets the recommended height.
        /// </summary>
        public int RecommendedHeight { get; set; }

        /// <summary>
        /// Gets or sets the report type.
        /// </summary>
        public string ReportType { get; set; }

        /// <summary>
        /// Gets or sets the web content url.
        /// </summary>
        public Uri WebContentUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether local url used.
        /// </summary>
        public bool LocalUrlUsed { get; set; }

        /// <summary>
        /// Gets or sets the web content html.
        /// </summary>
        public string WebContentHtml { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether disable open in url.
        /// </summary>
        public bool DisableOpenInUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether open in modal frame.
        /// </summary>
        public bool OpenInModalFrame { get; set; }
    }
}
