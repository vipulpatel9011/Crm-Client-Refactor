// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEComputedBundlePricing.cs" company="Aurea Software Gmbh">
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
//   Computed Bundle Pricing
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;

    /// <summary>
    /// Computed Bundle Pricing
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEPricingConditionBase" />
    public class UPSEComputedBundlePricing : UPSEPricingConditionBase
    {
        private List<UPSERow> otherPositions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEComputedBundlePricing"/> class.
        /// </summary>
        /// <param name="bundlePricing">The bundle pricing.</param>
        /// <param name="rowRecordId">The row record identifier.</param>
        /// <param name="rows">The rows.</param>
        public UPSEComputedBundlePricing(UPSEBundlePricing bundlePricing, string rowRecordId, List<UPSERow> rows)
            : base(bundlePricing.RecordIdentification, bundlePricing.DataDictionary, null)
        {
            this.OtherQuantity = 0;
            this.RowRecordId = rowRecordId;
            bool reduceByPrice = bundlePricing.HasPriceBoundaries;
            foreach (UPSERow row in rows)
            {
                if (row.RowRecordId == this.RowRecordId || row.RowPricing.BundleConditions.RecordIdentification != this.RecordIdentification)
                {
                    continue;
                }

                if (this.otherPositions == null)
                {
                    this.otherPositions = new List<UPSERow> { row };
                }
                else
                {
                    this.otherPositions.Add(row);
                }

                if (reduceByPrice)
                {
                    this.OtherEndPrice += row.EndPriceWithoutDiscount;
                }
                else
                {
                    this.OtherQuantity += (int)row.Quantity;
                }
            }

            if (reduceByPrice)
            {
                if (this.OtherEndPrice == 0)
                {
                    this.scales = bundlePricing.Scales;
                }
                else if (bundlePricing.Scales.Count > 0)
                {
                    List<UPSEPricingBase> scaleArray = new List<UPSEPricingBase>();
                    foreach (UPSEBundlePricingScale scale in bundlePricing.Scales)
                    {
                        UPSEBundlePricingScale adjustedScale = scale.BundlePricingByReducingPrice(this.OtherEndPrice);
                        if (adjustedScale != null)
                        {
                            scaleArray.Add(adjustedScale);
                        }
                    }

                    this.scales = scaleArray;
                }
            }
            else if (this.OtherQuantity == 0)
            {
                this.scales = bundlePricing.Scales;
            }
            else if (bundlePricing.Scales.Count > 0)
            {
                List<UPSEPricingBase> scaleArray = new List<UPSEPricingBase>();
                foreach (UPSEBundlePricingScale scale in bundlePricing.Scales)
                {
                    UPSEBundlePricingScale adjustedScale = scale.BundlePricingByReducingQuantity(this.OtherQuantity);
                    if (adjustedScale != null)
                    {
                        scaleArray.Add(adjustedScale);
                    }
                }

                this.scales = scaleArray;
            }
        }

        /// <summary>
        /// Gets the row record identifier.
        /// </summary>
        /// <value>
        /// The row record identifier.
        /// </value>
        public string RowRecordId { get; }

        /// <summary>
        /// Gets the other quantity.
        /// </summary>
        /// <value>
        /// The other quantity.
        /// </value>
        public int OtherQuantity { get; }

        /// <summary>
        /// Gets the other end price.
        /// </summary>
        /// <value>
        /// The other end price.
        /// </value>
        public double OtherEndPrice { get; }

        /// <summary>
        /// Gets the other positions.
        /// </summary>
        /// <value>
        /// The other positions.
        /// </value>
        public override List<UPSERow> OtherPositions => this.otherPositions;
    }
}
