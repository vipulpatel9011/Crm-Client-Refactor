// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEQuotaHandlerDelegate.cs" company="Aurea Software Gmbh">
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
//   The UPSEQuotaHandler Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;

    /// <summary>
    /// UPSEQuotaHandlerDelegate
    /// </summary>
    public interface UPSEQuotaHandlerDelegate
    {
        /// <summary>
        /// Serials the entry quota handler did finish with result.
        /// </summary>
        /// <param name="quota">The quota.</param>
        /// <param name="result">The result.</param>
        void SerialEntryQuotaHandlerDidFinishWithResult(UPSEQuotaHandler quota, object result);

        /// <summary>
        /// Serials the entry quota handler did fail with error.
        /// </summary>
        /// <param name="quota">The quota.</param>
        /// <param name="error">The error.</param>
        void SerialEntryQuotaHandlerDidFailWithError(UPSEQuotaHandler quota, Exception error);
    }
}
