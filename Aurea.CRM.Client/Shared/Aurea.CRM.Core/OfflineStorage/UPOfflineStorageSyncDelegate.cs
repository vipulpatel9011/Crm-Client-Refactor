// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineStorageSyncDelegate.cs" company="Aurea Software Gmbh">
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
//   The Offline Storage Sync Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using System;

    /// <summary>
    /// Offline Storage Sync Delegate
    /// </summary>
    public interface UPOfflineStorageSyncDelegate
    {
        /// <summary>
        /// Offlines the storage did finish with result.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="result">The result.</param>
        void OfflineStorageDidFinishWithResult(UPOfflineStorage sender, object result);

        /// <summary>
        /// Offlines the storage did fail with error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        void OfflineStorageDidFailWithError(UPOfflineStorage sender, Exception error);

        /// <summary>
        /// Offlines the storage did proceed to step number of steps.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="currentStepNumber">The current step number.</param>
        /// <param name="totalStepNumber">The total step number.</param>
        void OfflineStorageDidProceedToStepNumberOfSteps(UPOfflineStorage sender, int currentStepNumber, int totalStepNumber);
    }
}
