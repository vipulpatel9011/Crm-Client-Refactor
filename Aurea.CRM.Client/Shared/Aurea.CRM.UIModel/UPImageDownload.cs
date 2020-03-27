// <copyright file="UPImageDownload.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel
{
    using System;

    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// Downloadable image infomation
    /// </summary>
    public class UPImageDownload
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public Uri Url { get; set; }

        /// <summary>
        /// Gets or sets the edit field.
        /// </summary>
        /// <value>
        /// The edit field.
        /// </value>
        public UPMImageEditField EditField { get; set; }
    }
}
