// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPWebContentMetadataDelegate.cs" company="Aurea Software Gmbh">
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
//   UPWebContentMetadataDelegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using System;

    /// <summary>
    /// UPWebContentMetadataDelegate
    /// </summary>
    public interface UPWebContentMetadataDelegate
    {
        /// <summary>
        /// Webs the content meta data finished with XML string.
        /// </summary>
        /// <param name="clientReport">The client report.</param>
        /// <param name="xmlString">The XML string.</param>
        void WebContentMetaDataFinishedWithXmlString(UPWebContentMetadata clientReport, string xmlString);

        /// <summary>
        /// Webs the content meta data failed with error.
        /// </summary>
        /// <param name="clientReport">The client report.</param>
        /// <param name="error">The error.</param>
        void WebContentMetaDataFailedWithError(UPWebContentMetadata clientReport, Exception error);

        /// <summary>
        /// Webs the content meta data finished with redirect URL.
        /// </summary>
        /// <param name="clientReport">The client report.</param>
        /// <param name="url">The URL.</param>
        void WebContentMetaDataFinishedWithRedirectUrl(UPWebContentMetadata clientReport, Uri url);

        /// <summary>
        /// Gets a value indicating whether this instance is online.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is online; otherwise, <c>false</c>.
        /// </value>
        bool IsOnline { get; }
    }
}
