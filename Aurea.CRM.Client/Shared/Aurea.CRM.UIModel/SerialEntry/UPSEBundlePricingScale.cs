// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEBundlePricingScale.cs" company="Aurea Software Gmbh">
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
//   The bundle pricing scale
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Bundle Pricing Scale
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEPricingBase" />
    public class UPSEBundlePricingScale : UPSEPricingBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingBase"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="dataDictionary">The data dictionary.</param>
        /// <param name="pricing">The pricing.</param>
        public UPSEBundlePricingScale(string recordIdentification, Dictionary<string, object> dataDictionary, UPSEPricing pricing)
            : base(recordIdentification, dataDictionary, pricing)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEBundlePricingScale"/> class.
        /// </summary>
        /// <param name="pricingBase">The pricing base.</param>
        /// <param name="minusQuantity">The minus quantity.</param>
        public UPSEBundlePricingScale(UPSEPricingBase pricingBase, int minusQuantity)
            : base(pricingBase, minusQuantity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEBundlePricingScale"/> class.
        /// </summary>
        /// <param name="pricingBase">The pricing base.</param>
        /// <param name="minusPrice">The minus price.</param>
        public UPSEBundlePricingScale(UPSEPricingBase pricingBase, double minusPrice)
            : base(pricingBase, minusPrice)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingBase"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="pricing">The pricing.</param>
        public UPSEBundlePricingScale(UPCRMResultRow row, FieldControl fieldControl, UPSEPricing pricing)
            : base(row, fieldControl, pricing)
        {
        }

        /// <summary>
        /// Bundles the pricing by reducing quantity.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public UPSEBundlePricingScale BundlePricingByReducingQuantity(int quantity)
        {
            return new UPSEBundlePricingScale(this, quantity);
        }

        /// <summary>
        /// Bundles the pricing by reducing price.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <returns></returns>
        public UPSEBundlePricingScale BundlePricingByReducingPrice(double price)
        {
            return new UPSEBundlePricingScale(this, (double)price);
        }
    }
}
