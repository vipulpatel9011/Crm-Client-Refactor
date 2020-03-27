// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingBase.cs" company="Aurea Software Gmbh">
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
//   The Pricing Base
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The function name - unitprice
        /// </summary>
        public const string FUNCTIONNAME_UNITPRICE = "UnitPrice";

        /// <summary>
        /// The functionname - minquantity
        /// </summary>
        public const string FUNCTIONNAME_MINQUANTITY = "MinQuantity";

        /// <summary>
        /// The functionname - maxquantity
        /// </summary>
        public const string FUNCTIONNAME_MAXQUANTITY = "MaxQuantity";

        /// <summary>
        /// The functionname - freegoods
        /// </summary>
        public const string FUNCTIONNAME_FREEGOODS = "FreeGoods";

        /// <summary>
        /// The functionname - minprice
        /// </summary>
        public const string FUNCTIONNAME_MINPRICE = "MinPrice";

        /// <summary>
        /// The functionname - maxprice
        /// </summary>
        public const string FUNCTIONNAME_MAXPRICE = "MaxPrice";

        /// <summary>
        /// The functionname - discount
        /// </summary>
        public const string FUNCTIONNAME_DISCOUNT = "Discount";

        /// <summary>
        /// The functionname - currency
        /// </summary>
        public const string FUNCTIONNAME_CURRENCY = "Currency";
    }

    /// <summary>
    /// Pricing Base
    /// </summary>
    public class UPSEPricingBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingBase"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="dataDictionary">The data dictionary.</param>
        /// <param name="pricing">The pricing.</param>
        public UPSEPricingBase(string recordIdentification, Dictionary<string, object> dataDictionary, UPSEPricing pricing)
        {
            this.RecordIdentification = recordIdentification;
            this.DataDictionary = dataDictionary;
            this.Pricing = pricing;
            this.Currency = Convert.ToInt32(this.DataDictionary.ValueOrDefault(Constants.FUNCTIONNAME_CURRENCY));
            int currency = Convert.ToInt32(this.DataDictionary.ValueOrDefault("Currency"));
            this.ExchangeRate = 1.0f;

            if (currency > 0 && currency != this.Pricing.Currency)
            {
                this.ExchangeRate = this.Pricing.CurrencyConversion.ExchangeRateFromCodeToCode(currency, this.Pricing.Currency);
                if (this.ExchangeRate == 0.0)
                {
                    this.ExchangeRate = 1.0f;
                }
            }

            string value = this.DataDictionary.ValueOrDefault(Constants.FUNCTIONNAME_MINQUANTITY) as string;
            if (!string.IsNullOrEmpty(value))
            {
                this.MinQuantity = Convert.ToInt32(value);
                this.HasQuantityBoundaries = this.MinQuantity > 0;
            }

            value = this.DataDictionary.ValueOrDefault(Constants.FUNCTIONNAME_MAXQUANTITY) as string;
            if (!string.IsNullOrEmpty(value))
            {
                this.MaxQuantity = Convert.ToInt32(value);
                this.HasQuantityBoundaries = this.HasQuantityBoundaries || (this.MaxQuantity > 0);
            }

            value = this.DataDictionary.ValueOrDefault(Constants.FUNCTIONNAME_FREEGOODS) as string;
            if (!string.IsNullOrEmpty(value))
            {
                this.FreeGoods = Convert.ToInt32(value);
                this.HasFreeGoods = this.FreeGoods > 0;
            }

            value = this.DataDictionary.ValueOrDefault(Constants.FUNCTIONNAME_MINPRICE) as string;
            if (!string.IsNullOrEmpty(value))
            {
                this.MinEndPrice = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture) * this.ExchangeRate;
                this.HasPriceBoundaries = this.MinEndPrice != 0;
            }

            value = this.DataDictionary.ValueOrDefault(Constants.FUNCTIONNAME_MAXPRICE) as string;
            if (!string.IsNullOrEmpty(value))
            {
                this.MaxEndPrice = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture) * this.ExchangeRate;
                this.HasPriceBoundaries = this.HasPriceBoundaries || this.MaxEndPrice != 0;
            }

            value = this.DataDictionary.ValueOrDefault(Constants.FUNCTIONNAME_DISCOUNT) as string;
            if (!string.IsNullOrEmpty(value))
            {
                this.Discount = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
                this.HasDiscount = this.Discount != 0;
            }

            value = this.DataDictionary.ValueOrDefault(Constants.FUNCTIONNAME_UNITPRICE) as string;
            if (!string.IsNullOrEmpty(value))
            {
                this.UnitPrice = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture) * this.ExchangeRate;
                this.HasUnitPrice = this.UnitPrice != 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingBase"/> class.
        /// </summary>
        /// <param name="pricingBase">The pricing base.</param>
        /// <param name="minusQuantity">The minus quantity.</param>
        public UPSEPricingBase(UPSEPricingBase pricingBase, int minusQuantity)
        {
            if (!pricingBase.HasQuantityBoundaries)
            {
                throw new InvalidOperationException("HasQuantityBoundaries is false");
            }

            if (pricingBase.MaxQuantity > 0 && minusQuantity >= pricingBase.MaxQuantity)
            {
                throw new InvalidOperationException("Check failed");
            }

            if (pricingBase.MaxQuantity > 0)
            {
                this.MaxQuantity = pricingBase.MaxQuantity - minusQuantity;
            }
            else
            {
                this.MaxQuantity = pricingBase.MaxQuantity;
            }

            if (pricingBase.MinQuantity > minusQuantity)
            {
                this.MinQuantity = pricingBase.MinQuantity - minusQuantity;
            }
            else
            {
                this.MinQuantity = 0;
            }

            this.HasDiscount = pricingBase.HasDiscount;
            this.Discount = pricingBase.Discount;
            this.HasFreeGoods = pricingBase.HasFreeGoods;
            this.FreeGoods = pricingBase.FreeGoods;
            this.HasQuantityBoundaries = true;
            this.HasPriceBoundaries = false;
            this.HasUnitPrice = pricingBase.HasUnitPrice;
            this.UnitPrice = pricingBase.UnitPrice;
            this.RecordIdentification = pricingBase.RecordIdentification;
            this.DataDictionary = pricingBase.DataDictionary;
            this.Pricing = pricingBase.Pricing;
            this.Currency = pricingBase.Currency;
            this.BaseScale = pricingBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingBase"/> class.
        /// </summary>
        /// <param name="pricingBase">The pricing base.</param>
        /// <param name="minusPrice">The minus price.</param>
        public UPSEPricingBase(UPSEPricingBase pricingBase, double minusPrice)
        {
            if (!pricingBase.HasPriceBoundaries)
            {
                throw new InvalidOperationException("HasPriceBoundaries is false");
            }

            if (pricingBase.MaxEndPrice > 0 && minusPrice >= pricingBase.MaxEndPrice)
            {
                throw new InvalidOperationException("Check failed");
            }

            if (pricingBase.MaxEndPrice > 0)
            {
                this.MaxEndPrice = pricingBase.MaxEndPrice - minusPrice;
            }
            else
            {
                this.MaxEndPrice = pricingBase.MaxEndPrice;
            }

            if (pricingBase.MinEndPrice > minusPrice)
            {
                this.MinEndPrice = pricingBase.MinEndPrice - minusPrice;
            }
            else
            {
                this.MinEndPrice = 0;
            }

            this.HasDiscount = pricingBase.HasDiscount;
            this.Discount = pricingBase.Discount;
            this.HasFreeGoods = pricingBase.HasFreeGoods;
            this.FreeGoods = pricingBase.FreeGoods;
            this.HasQuantityBoundaries = false;
            this.HasPriceBoundaries = true;
            this.HasUnitPrice = pricingBase.HasUnitPrice;
            this.UnitPrice = pricingBase.UnitPrice;
            this.RecordIdentification = pricingBase.RecordIdentification;
            this.DataDictionary = pricingBase.DataDictionary;
            this.Pricing = pricingBase.Pricing;
            this.Currency = pricingBase.Currency;
            this.BaseScale = pricingBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingBase"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="pricing">The pricing.</param>
        public UPSEPricingBase(UPCRMResultRow row, FieldControl fieldControl, UPSEPricing pricing)
            : this(row.RecordIdentificationAtIndex(0), fieldControl.FunctionNames(row), pricing)
        {
        }

        /// <summary>
        /// Gets the quantity boundaries string.
        /// </summary>
        /// <value>
        /// The quantity boundaries string.
        /// </value>
        public string QuantityBoundariesString
        {
            get
            {
                if (this.MinQuantity > 0)
                {
                    if (this.MaxQuantity > 0)
                    {
                        return $"{this.MinQuantity} - {this.MaxQuantity}";
                    }

                    return $">={this.MinQuantity}";
                }
                else if (this.MaxQuantity > 0)
                {
                    return $"<={this.MaxQuantity}";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the price boundaries string.
        /// </summary>
        /// <value>
        /// The price boundaries string.
        /// </value>
        public string PriceBoundariesString
        {
            get
            {
                if (this.MinEndPrice > 0)
                {
                    return this.MaxEndPrice > 0 ? $"{this.MinEndPrice:##.00} - {this.MaxEndPrice:##.00}" : $">={this.MinEndPrice:##.00}";
                }

                return this.MaxEndPrice > 0 ? $"<={this.MaxEndPrice:##.00}" : string.Empty;
            }
        }

        /// <summary>
        /// Gets the free goods string.
        /// </summary>
        /// <value>
        /// The free goods string.
        /// </value>
        public string FreeGoodsString => this.FreeGoods > 0 ? this.FreeGoods.ToString() : string.Empty;

        /// <summary>
        /// Gets the discount string.
        /// </summary>
        /// <value>
        /// The discount string.
        /// </value>
        public string DiscountString => this.Discount != 0 ? $"{this.Discount * 100:##0.00}" : string.Empty;

        /// <summary>
        /// Gets the unit price string.
        /// </summary>
        /// <value>
        /// The unit price string.
        /// </value>
        public string UnitPriceString => this.UnitPrice != 0 ? $"{this.UnitPrice:#0.00}" : string.Empty;

        /// <summary>
        /// Gets the other positions.
        /// </summary>
        /// <value>
        /// The other positions.
        /// </value>
        public virtual List<UPSERow> OtherPositions => null;

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; }

        /// <summary>
        /// Gets the data dictionary.
        /// </summary>
        /// <value>
        /// The data dictionary.
        /// </value>
        public Dictionary<string, object> DataDictionary { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has discount.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has discount; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasDiscount { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has free goods.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has free goods; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasFreeGoods { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has price boundaries.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has price boundaries; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasPriceBoundaries { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has quantity boundaries.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has quantity boundaries; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasQuantityBoundaries { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has unit price.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has unit price; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasUnitPrice { get; }

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
        /// Gets the minimum end price.
        /// </summary>
        /// <value>
        /// The minimum end price.
        /// </value>
        public double MinEndPrice { get; }

        /// <summary>
        /// Gets the maximum end price.
        /// </summary>
        /// <value>
        /// The maximum end price.
        /// </value>
        public double MaxEndPrice { get; }

        /// <summary>
        /// Gets the free goods.
        /// </summary>
        /// <value>
        /// The free goods.
        /// </value>
        public int FreeGoods { get; }

        /// <summary>
        /// Gets the unit price.
        /// </summary>
        /// <value>
        /// The unit price.
        /// </value>
        public double UnitPrice { get; }

        /// <summary>
        /// Gets the discount.
        /// </summary>
        /// <value>
        /// The discount.
        /// </value>
        public double Discount { get; }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public int Currency { get; }

        /// <summary>
        /// Gets the pricing.
        /// </summary>
        /// <value>
        /// The pricing.
        /// </value>
        public UPSEPricing Pricing { get; }

        /// <summary>
        /// Gets the exchange rate.
        /// </summary>
        /// <value>
        /// The exchange rate.
        /// </value>
        public double ExchangeRate { get; }

        /// <summary>
        /// Gets the base scale.
        /// </summary>
        /// <value>
        /// The base scale.
        /// </value>
        public UPSEPricingBase BaseScale { get; private set; }

        /// <summary>
        /// Matchings the index of the index key order maximum match.
        /// </summary>
        /// <param name="matchDictionary">The match dictionary.</param>
        /// <param name="keyOrder">The key order.</param>
        /// <param name="maxMatchIndex">Maximum index of the match.</param>
        /// <returns></returns>
        public int MatchingIndexKeyOrderMaxMatchIndex(Dictionary<string, object> matchDictionary, List<string> keyOrder, int maxMatchIndex)
        {
            if (maxMatchIndex >= keyOrder.Count)
            {
                maxMatchIndex = keyOrder.Count - 1;
            }

            for (int i = 0; i <= maxMatchIndex; i++)
            {
                string key = keyOrder[i];
                string val = this.DataDictionary.ValueOrDefault(key) as string;
                if (!string.IsNullOrEmpty(val))
                {
                    return val == matchDictionary.ValueOrDefault(key) as string ? i : -1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Matcheses the quantity.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public bool MatchesQuantity(int quantity)
        {
            if (!this.HasQuantityBoundaries)
            {
                return false;
            }

            if (quantity < this.MinQuantity)
            {
                return false;
            }

            return this.MaxQuantity == 0 || quantity <= this.MaxQuantity;
        }

        /// <summary>
        /// Matcheses the row price.
        /// </summary>
        /// <param name="rowPrice">The row price.</param>
        /// <returns></returns>
        public bool MatchesRowPrice(double rowPrice)
        {
            if (!this.HasPriceBoundaries)
            {
                return false;
            }

            if (rowPrice < this.MinEndPrice)
            {
                return false;
            }

            return this.MaxEndPrice == 0 || !(rowPrice > this.MaxEndPrice);
        }
    }
}
