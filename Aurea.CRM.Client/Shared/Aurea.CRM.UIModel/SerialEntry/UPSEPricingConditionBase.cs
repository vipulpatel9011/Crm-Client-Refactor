// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingConditionBase.cs" company="Aurea Software Gmbh">
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
//   The Pricing Condition Base
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Pricing Condition Base class
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEPricingBase" />
    public class UPSEPricingConditionBase : UPSEPricingBase
    {
        /// <summary>
        /// The scales
        /// </summary>
        protected List<UPSEPricingBase> scales;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingConditionBase"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="dataDictionary">The data dictionary.</param>
        /// <param name="pricingSet">The pricing set.</param>
        public UPSEPricingConditionBase(string recordIdentification, Dictionary<string, object> dataDictionary, UPSEPricingSet pricingSet)
            : base(recordIdentification, dataDictionary, pricingSet.Pricing)
        {
            this.PricingSet = pricingSet;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingConditionBase"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="pricingSet">The pricing set.</param>
        public UPSEPricingConditionBase(UPCRMResultRow row, FieldControl fieldControl, UPSEPricingSet pricingSet)
            : this(row.RecordIdentificationAtIndex(0), fieldControl.FunctionNames(row), pricingSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingConditionBase"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="pricing">The pricing.</param>
        public UPSEPricingConditionBase(UPCRMResultRow row, FieldControl fieldControl, UPSEPricing pricing)
            : base(row, fieldControl, pricing)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance has scale information.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has scale information; otherwise, <c>false</c>.
        /// </value>
        public bool HasScaleInformation => this.scales.Count > 0;

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => true;

        /// <summary>
        /// Gets the scales.
        /// </summary>
        /// <value>
        /// The scales.
        /// </value>
        public List<UPSEPricingBase> Scales => this.scales;

        /// <summary>
        /// Gets the pricing set.
        /// </summary>
        /// <value>
        /// The pricing set.
        /// </value>
        public UPSEPricingSet PricingSet { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has discount.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has discount; otherwise, <c>false</c>.
        /// </value>
        public override bool HasDiscount => base.HasDiscount || this.Scales.Any(item => item.HasDiscount);

        /// <summary>
        /// Gets a value indicating whether this instance has free goods.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has free goods; otherwise, <c>false</c>.
        /// </value>
        public override bool HasFreeGoods => base.HasFreeGoods || this.Scales.Any(item => item.HasFreeGoods);

        /// <summary>
        /// Gets a value indicating whether this instance has price boundaries.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has price boundaries; otherwise, <c>false</c>.
        /// </value>
        public override bool HasPriceBoundaries => base.HasPriceBoundaries || this.Scales.Any(item => item.HasPriceBoundaries);

        /// <summary>
        /// Gets a value indicating whether this instance has quantity boundaries.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has quantity boundaries; otherwise, <c>false</c>.
        /// </value>
        public override bool HasQuantityBoundaries => base.HasQuantityBoundaries || this.Scales.Any(item => item.HasQuantityBoundaries);

        /// <summary>
        /// Gets a value indicating whether this instance has unit price.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has unit price; otherwise, <c>false</c>.
        /// </value>
        public override bool HasUnitPrice => base.HasUnitPrice || this.Scales.Any(item => item.HasUnitPrice);

        /// <summary>
        /// Adds the scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        public void AddScale(UPSEPricingBase scale)
        {
            if (this.scales == null)
            {
                this.scales = new List<UPSEPricingBase> { scale };
            }
            else
            {
                this.scales.Add(scale);
            }
        }

        private UPSEPricingBase ScaleForQuantity(int quantity)
        {
            return this.scales.FirstOrDefault(scale => scale.MatchesQuantity(quantity));
        }

        private int MinQuantityForScale()
        {
            int minQuantity = -1;
            foreach (UPSEPricingBase scale in this.scales)
            {
                if (scale.MinQuantity < 0)
                {
                    return 0;
                }

                if (minQuantity == -1 || minQuantity > scale.MinQuantity)
                {
                    minQuantity = scale.MinQuantity;
                }
            }

            return minQuantity;
        }

        private int MaxQuantityForScale()
        {
            int maxQuantity = -1;
            foreach (UPSEPricingBase scale in this.scales)
            {
                if (scale.MaxQuantity < 0)
                {
                    return -1;
                }

                if (maxQuantity < scale.MaxQuantity)
                {
                    maxQuantity = scale.MaxQuantity;
                }
            }

            return maxQuantity;
        }

        private UPSEPricingBase ScaleForRowPrice(double rowPrice)
        {
            return this.scales.FirstOrDefault(scale => scale.MatchesRowPrice(rowPrice));
        }

        private double MinRowPriceForScale()
        {
            double minPrice = -1;
            foreach (UPSEPricingBase scale in this.scales)
            {
                if (scale.MinEndPrice < 0)
                {
                    return 0;
                }

                if (minPrice < 0 || minPrice > scale.MinEndPrice)
                {
                    minPrice = scale.MinEndPrice;
                }
            }

            return minPrice;
        }

        private double MaxRowPriceForScale()
        {
            double maxPrice = -1;
            foreach (UPSEPricingBase scale in this.scales)
            {
                if (scale.MaxEndPrice < 0)
                {
                    return -1;
                }

                if (maxPrice < scale.MaxEndPrice)
                {
                    maxPrice = scale.MaxEndPrice;
                }
            }

            return maxPrice;
        }

        /// <summary>
        /// Frees the goods for quantity row price.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="priceWithoutDiscount">The price without discount.</param>
        /// <returns></returns>
        public int FreeGoodsForQuantityRowPrice(int quantity, double priceWithoutDiscount)
        {
            int freeGoods = 0;
            UPSEPricingBase scale = this.HasQuantityBoundaries ? this.ScaleForQuantity(quantity) : this.ScaleForRowPrice(priceWithoutDiscount);

            if (scale != null)
            {
                freeGoods = scale.FreeGoods;
            }

            return freeGoods > 0 ? freeGoods : this.FreeGoods;
        }

        /// <summary>
        /// Discounts the information for quantity row price.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="priceWithoutDiscount">The price without discount.</param>
        /// <returns></returns>
        public UPSEPricingDiscountInfo DiscountInfoForQuantityRowPrice(int quantity, double priceWithoutDiscount)
        {
            UPSEPricingBase scale = this.HasQuantityBoundaries ? this.ScaleForQuantity(quantity) : this.ScaleForRowPrice(priceWithoutDiscount);

            double discount = 0;
            if (scale != null)
            {
                discount = scale.Discount;
            }

            if (discount == 0)
            {
                discount = this.Discount;
            }

            if (scale != null)
            {
                if (scale.HasQuantityBoundaries)
                {
                    return new UPSEPricingDiscountInfo(discount, scale.MinQuantity, this.MaxQuantity);
                }

                if (priceWithoutDiscount > 0 && quantity != 0)
                {
                    double unitPrice = priceWithoutDiscount / quantity;
                    int minQuantity = (int)Math.Ceiling(scale.MinEndPrice / unitPrice);
                    int maxQuantity = (int)Math.Floor(scale.MaxEndPrice / unitPrice);
                    return new UPSEPricingDiscountInfo(discount, minQuantity, maxQuantity);
                }

                if (priceWithoutDiscount == 0)
                {
                    return new UPSEPricingDiscountInfo(discount, true);
                }
            }
            else if (this.HasQuantityBoundaries)
            {
                int minQuantityForDiscount = this.MinQuantityForScale();
                if (minQuantityForDiscount > 0 && quantity < minQuantityForDiscount)
                {
                    return new UPSEPricingDiscountInfo(discount, 0, minQuantityForDiscount - 1);
                }

                int maxQuantityForDiscount = this.MaxQuantityForScale();
                if (quantity > maxQuantityForDiscount)
                {
                    return new UPSEPricingDiscountInfo(discount, maxQuantityForDiscount + 1, -1);
                }
            }
            else if (priceWithoutDiscount == 0)
            {
                return new UPSEPricingDiscountInfo(discount, true);
            }
            else if (quantity != 0)
            {
                double minEndPrice = this.MinRowPriceForScale();
                if (priceWithoutDiscount < minEndPrice)
                {
                    double unitPrice = priceWithoutDiscount / quantity;
                    int minQuantity = (int)Math.Ceiling(minEndPrice / unitPrice);
                    return new UPSEPricingDiscountInfo(discount, 0, minQuantity - 1);
                }

                double maxEndPrice = this.MaxRowPriceForScale();
                if (priceWithoutDiscount > maxEndPrice)
                {
                    double unitPrice = priceWithoutDiscount / quantity;
                    int maxQuantity = (int)Math.Floor(maxEndPrice / unitPrice);
                    return new UPSEPricingDiscountInfo(discount, maxQuantity + 1, -1);
                }
            }

            return new UPSEPricingDiscountInfo(discount, false);
        }

        /// <summary>
        /// Units the price for quantity row price.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="priceWithoutDiscount">The price without discount.</param>
        /// <returns></returns>
        public double UnitPriceForQuantityRowPrice(int quantity, double priceWithoutDiscount)
        {
            double unitPrice = 0;
            UPSEPricingBase scale = this.HasQuantityBoundaries ? this.ScaleForQuantity(quantity) : this.ScaleForRowPrice(priceWithoutDiscount);

            if (scale != null)
            {
                unitPrice = scale.UnitPrice;
            }

            return unitPrice != 0 ? unitPrice : this.UnitPrice;
        }
    }
}
