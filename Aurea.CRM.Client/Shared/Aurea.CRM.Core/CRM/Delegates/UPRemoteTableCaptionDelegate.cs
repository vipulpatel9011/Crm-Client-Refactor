// <copyright file="UPRemoteTableCaptionDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.CRM.Delegates
{
    using System;

    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Remote table caption delegate
    /// </summary>
    public interface IRemoteTableCaptionDelegate
    {
        /// <summary>
        /// Ups the table caption did fail with error.
        /// </summary>
        /// <param name="tableCaption">The table caption.</param>
        /// <param name="error">The error.</param>
        void TableCaptionDidFailWithError(UPConfigTableCaption tableCaption, Exception error);

        /// <summary>
        /// Ups the table caption did finish with result.
        /// </summary>
        /// <param name="tableCaption">The table caption.</param>
        /// <param name="tableCaptionString">The table caption string.</param>
        void TableCaptionDidFinishWithResult(UPConfigTableCaption tableCaption, string tableCaptionString);
    }
}
