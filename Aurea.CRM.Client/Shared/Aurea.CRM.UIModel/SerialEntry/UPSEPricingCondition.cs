// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingCondition.cs" company="Aurea Software Gmbh">
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
//   The Pricing Condition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Pricing Condition
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEPricingConditionBase" />
    public class UPSEPricingCondition : UPSEPricingConditionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingCondition"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="dataDictionary">The data dictionary.</param>
        /// <param name="bundlePricingIdentification">The bundle pricing identification.</param>
        /// <param name="pricingSet">The pricing set.</param>
        public UPSEPricingCondition(string recordIdentification, Dictionary<string, object> dataDictionary, string bundlePricingIdentification, UPSEPricingSet pricingSet)
           : base(recordIdentification, dataDictionary, pricingSet)
        {
            this.BundlePricingIdentification = bundlePricingIdentification;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingCondition"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="bundleKeyIndex">Index of the bundle key.</param>
        /// <param name="pricingSet">The pricing set.</param>
        public UPSEPricingCondition(UPCRMResultRow row, FieldControl fieldControl, int bundleKeyIndex, UPSEPricingSet pricingSet)
             : this(row.RecordIdentificationAtIndex(0), fieldControl.FunctionNames(row),
                  bundleKeyIndex > 0 ? row.RecordIdentificationAtIndex(bundleKeyIndex) : null,
                  pricingSet)
        {
        }

        /// <summary>
        /// Gets the bundle pricing.
        /// </summary>
        /// <value>
        /// The bundle pricing.
        /// </value>
        public UPSEBundlePricing BundlePricing
             => !string.IsNullOrEmpty(this.BundlePricingIdentification) ? this.PricingSet.BundlePricingForKey(this.BundlePricingIdentification) : null;

        /// <summary>
        /// Gets the bundle pricing identification.
        /// </summary>
        /// <value>
        /// The bundle pricing identification.
        /// </value>
        public string BundlePricingIdentification { get; }
    }
}
