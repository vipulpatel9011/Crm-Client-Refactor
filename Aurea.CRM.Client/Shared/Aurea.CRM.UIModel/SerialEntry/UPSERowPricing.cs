// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSERowPricing.cs" company="Aurea Software Gmbh">
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
//   Serai Entry Row Pricing
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Serial Entry Row Pricing
    /// </summary>
    public class UPSERowPricing
    {
        private bool conditionsLoaded;
        private bool bundleConditionsLoaded;
        private bool currentConditionsLoaded;
        private UPSEPricingConditionBase currentConditions;
        private UPSEPricingConditionBase conditions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERowPricing"/> class.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="priceProvider">The price provider.</param>
        /// <param name="rowRecordId">The row record identifier.</param>
        public UPSERowPricing(UPSEPrice price, UPSEPricing priceProvider, string rowRecordId)
        {
            this.Price = price;
            this.PriceProvider = priceProvider;
            this.RowRecordId = rowRecordId;
            this.bundleConditionsLoaded = false;
            this.conditionsLoaded = false;
            this.currentConditionsLoaded = false;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is bundle pricing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is bundle pricing; otherwise, <c>false</c>.
        /// </value>
        public bool IsBundlePricing => this.BundleConditions != null;

        /// <summary>
        /// Gets a value indicating whether this instance has free goods.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has free goods; otherwise, <c>false</c>.
        /// </value>
        public bool HasFreeGoods => (this.currentConditions?.HasFreeGoods ?? false);

        /// <summary>
        /// Gets a value indicating whether this instance has unit price.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has unit price; otherwise, <c>false</c>.
        /// </value>
        public bool HasUnitPrice
        {
            get
            {
                if (!this.conditionsLoaded)
                {
                    this.LoadConditions();
                }

                if ((this.AllConditionArray?.Any(condition => condition.HasUnitPrice) ?? false))
                {
                    return true;
                }

                return this.AllConditionArray?.Count <= 0 && !this.Price.HasPriceScale && !this.Price.HasDiscountScale;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has discount.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has discount; otherwise, <c>false</c>.
        /// </value>
        public bool HasDiscount
        {
            get
            {
                if (this.currentConditions != null)
                {
                    return this.currentConditions.HasDiscount;
                }

                if (!this.conditionsLoaded)
                {
                    this.LoadConditions();
                }

                return (this.conditions?.HasDiscount ?? false) || (this.BundleConditions?.HasDiscount ?? false);
            }
        }

        /// <summary>
        /// Gets the other positions.
        /// </summary>
        /// <value>
        /// The other positions.
        /// </value>
        public List<UPSERow> OtherPositions => this.currentConditions.OtherPositions;

        /// <summary>
        /// Gets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public UPSEPrice Price { get; private set; }

        /// <summary>
        /// Gets the conditions.
        /// </summary>
        /// <value>
        /// The conditions.
        /// </value>
        public UPSEPricingConditionBase Conditions
        {
            get
            {
                if (!this.conditionsLoaded)
                {
                    this.LoadConditions();
                }

                return this.conditions;
            }
        }

        /// <summary>
        /// Gets the bundle conditions.
        /// </summary>
        /// <value>
        /// The bundle conditions.
        /// </value>
        public UPSEBundlePricing BundleConditions { get; private set; }

        /// <summary>
        /// Gets the price provider.
        /// </summary>
        /// <value>
        /// The price provider.
        /// </value>
        public UPSEPricing PriceProvider { get; private set; }

        /// <summary>
        /// Gets the row record identifier.
        /// </summary>
        /// <value>
        /// The row record identifier.
        /// </value>
        public string RowRecordId { get; private set; }

        /// <summary>
        /// Gets the bundle identification.
        /// </summary>
        /// <value>
        /// The bundle identification.
        /// </value>
        public string BundleIdentification { get; private set; }

        /// <summary>
        /// Gets all condition array.
        /// </summary>
        /// <value>
        /// All condition array.
        /// </value>
        public List<UPSEPricingConditionBase> AllConditionArray { get; private set; }

        /// <summary>
        /// Updates the current conditions with positions.
        /// </summary>
        /// <param name="positions">The positions.</param>
        public void UpdateCurrentConditionsWithPositions(List<UPSERow> positions)
        {
            this.CurrentConditionsWithPositions(positions);
        }

        /// <summary>
        /// Currents the conditions with positions.
        /// </summary>
        /// <param name="positions">The positions.</param>
        /// <returns></returns>
        public UPSEPricingConditionBase CurrentConditionsWithPositions(List<UPSERow> positions)
        {
            if (!this.conditionsLoaded)
            {
                this.LoadConditions();
            }

            this.currentConditions = this.BundleConditions != null
                ? this.BundleConditions.BundlePricingForRowRecordIdPositions(this.RowRecordId, positions)
                : this.Conditions;

            return this.currentConditions;
        }

        /// <summary>
        /// Loads the conditions.
        /// </summary>
        public void LoadConditions()
        {
            if (!this.conditionsLoaded)
            {
                this.AllConditionArray = this.PriceProvider.ConditionsForDataRowRecordId(this.Price.DataDictionary, this.RowRecordId);
                this.conditions = this.AllConditionArray?.Count > 0 ? this.AllConditionArray?[0] : null;

                if (this.conditions != null)
                {
                    this.BundleConditions = ((UPSEPricingCondition)this.conditions).BundlePricing;
                    if (this.BundleConditions != null)
                    {
                        this.currentConditions = this.BundleConditions;
                        this.BundleConditions.AddArticleRecordId(this.RowRecordId);
                        this.BundleIdentification = this.BundleConditions.RecordIdentification;
                    }
                    else
                    {
                        this.currentConditions = this.conditions;
                    }
                }

                this.conditionsLoaded = true;
            }
        }

        /// <summary>
        /// Frees the goods for quantity row price.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="priceWithoutDiscount">The price without discount.</param>
        /// <returns></returns>
        public int FreeGoodsForQuantityRowPrice(int quantity, double priceWithoutDiscount)
        {
            UPSEPricingConditionBase current = this.currentConditions;
            if (current == null || !current.HasFreeGoods)
            {
                return 0;
            }

            return current.FreeGoodsForQuantityRowPrice(quantity, priceWithoutDiscount);
        }

        /// <summary>
        /// Discounts the information for quantity row price.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="priceWithoutDiscount">The price without discount.</param>
        /// <returns></returns>
        public UPSEPricingDiscountInfo DiscountInfoForQuantityRowPrice(int quantity, double priceWithoutDiscount)
        {
            UPSEPricingConditionBase current = this.currentConditions;
            if (current == null || !current.HasDiscount)
            {
                return null;
            }

            return current.DiscountInfoForQuantityRowPrice(quantity, priceWithoutDiscount);
        }

        /// <summary>
        /// Units the price for quantity row price.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="priceWithoutDiscount">The price without discount.</param>
        /// <returns></returns>
        public double UnitPriceForQuantityRowPrice(int quantity, double priceWithoutDiscount)
        {
            foreach (UPSEPricingConditionBase current in this.AllConditionArray)
            {
                if (!current.HasUnitPrice)
                {
                    continue;
                }

                return current.UnitPriceForQuantityRowPrice(quantity, priceWithoutDiscount);
            }

            return this.Price.UnitPrice;
        }
    }
}
