// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingSetDelegate.cs" company="Aurea Software Gmbh">
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
//   Pricing Set Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;

    /// <summary>
    /// Pricing Set Delegate
    /// </summary>
    public interface UPSEPricingSetDelegate
    {
        /// <summary>
        /// Pricings the set did finish with result.
        /// </summary>
        /// <param name="pricingSet">The pricing set.</param>
        /// <param name="data">The data.</param>
        void PricingSetDidFinishWithResult(UPSEPricingSet pricingSet, object data);

        /// <summary>
        /// Pricings the set did fail with error.
        /// </summary>
        /// <param name="pricingSet">The pricing set.</param>
        /// <param name="error">The error.</param>
        void PricingSetDidFailWithError(UPSEPricingSet pricingSet, Exception error);
    }
}
