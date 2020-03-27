// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingDiscountInfo.cs" company="Aurea Software Gmbh">
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
//   The Pricing Discount Info
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    /// <summary>
    /// Pricing Discount Info
    /// </summary>
    public class UPSEPricingDiscountInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingDiscountInfo"/> class.
        /// </summary>
        /// <param name="discount">The discount.</param>
        /// <param name="dontCache">if set to <c>true</c> [dont cache].</param>
        public UPSEPricingDiscountInfo(double discount, bool dontCache)
        {
            this.Discount = discount;
            this.MinQuantity = -1;
            this.MaxQuantity = -1;
            this.DontCache = dontCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingDiscountInfo"/> class.
        /// </summary>
        /// <param name="discount">The discount.</param>
        /// <param name="minQuantity">The minimum quantity.</param>
        /// <param name="maxQuantity">The maximum quantity.</param>
        public UPSEPricingDiscountInfo(double discount, int minQuantity, int maxQuantity)
        {
            this.Discount = discount;
            this.MinQuantity = minQuantity;
            this.MaxQuantity = maxQuantity;
        }

        /// <summary>
        /// Gets the discount.
        /// </summary>
        /// <value>
        /// The discount.
        /// </value>
        public double Discount { get; }

        /// <summary>
        /// Gets the minimum quantity.
        /// </summary>
        /// <value>
        /// The minimum quantity.
        /// </value>
        public int MinQuantity { get; }

        /// <summary>
        /// Gets the maximum quantity.
        /// </summary>
        /// <value>
        /// The maximum quantity.
        /// </value>
        public int MaxQuantity { get; }

        /// <summary>
        /// Gets a value indicating whether [dont cache].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [dont cache]; otherwise, <c>false</c>.
        /// </value>
        public bool DontCache { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            if (this.MinQuantity < 0 && this.MaxQuantity < 0)
            {
                return $"Discount: {this.Discount:0.00}";
            }

            return $"Discount: {this.Discount:0.00} (Quantity {this.MinQuantity} - {this.MaxQuantity})";
        }
    }
}
