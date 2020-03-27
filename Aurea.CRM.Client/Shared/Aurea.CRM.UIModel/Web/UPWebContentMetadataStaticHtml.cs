// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPWebContentMetadataStaticHtml.cs" company="Aurea Software Gmbh">
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
//   The Web Content Metadata Static Html
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    /// <summary>
    /// The Web Content Metadata Static Html
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Web.UPWebContentMetadata" />
    public class UPWebContentMetadataStaticHtml : UPWebContentMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentMetadataStaticHtml"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentMetadataStaticHtml(UPWebContentMetadataDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public override string ReportType => "Html";
    }
}
