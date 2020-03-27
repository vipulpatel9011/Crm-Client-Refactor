// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingBulkVolumesDelegate.cs" company="Aurea Software Gmbh">
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
//   Pricing Bulk Volumes Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;

    /// <summary>
    /// Pricing Bulk Volumes Delegate
    /// </summary>
    public interface UPSEPricingBulkVolumesDelegate
    {
        /// <summary>
        /// Pricings the bulk volumes did finish with success.
        /// </summary>
        /// <param name="bulkVolumes">The bulk volumes.</param>
        /// <param name="result">The result.</param>
        void PricingBulkVolumesDidFinishWithSuccess(UPSEPricingBulkVolumes bulkVolumes, object result);

        /// <summary>
        /// Pricings the bulk volumes did fail with error.
        /// </summary>
        /// <param name="bulkVolumes">The bulk volumes.</param>
        /// <param name="error">The error.</param>
        void PricingBulkVolumesDidFailWithError(UPSEPricingBulkVolumes bulkVolumes, Exception error);
    }
}
