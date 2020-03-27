// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOpenUrlOperationDelegate.cs" company="Aurea Software Gmbh">
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
//   The Open Url Operation Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;

    /// <summary>
    /// Open Url Operation Delegate
    /// </summary>
    public interface UPOpenUrlOperationDelegate
    {
        /// <summary>
        /// Opens the URL operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        void OpenUrlOperationDidFailWithError(OpenUrlOperation operation, Exception error);

        /// <summary>
        /// Opens the URL operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="url">The URL.</param>
        void OpenUrlOperationDidFinishWithResult(OpenUrlOperation operation, Uri url);
    }
}
