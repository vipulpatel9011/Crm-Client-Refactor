// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPrice.cs" company="Aurea Software Gmbh">
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
//   Serial Entry Price
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Serial Entry Price
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEPricingBase" />
    public class UPSEPrice : UPSEPricingBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPrice"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="dataDictionary">The data dictionary.</param>
        /// <param name="pricing">The pricing.</param>
        public UPSEPrice(string recordIdentification, Dictionary<string, object> dataDictionary, UPSEPricing pricing)
            : base(recordIdentification, dataDictionary, pricing)
        {
            this.ArticleRecordId = recordIdentification.RecordId();
            if (pricing.BulkVolumes != null)
            {
                this.FillScalesFromDataDictionary(dataDictionary);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPrice"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="recordId">The record identifier.</param>
        /// <param name="pricing">The pricing.</param>
        public UPSEPrice(UPCRMResultRow row, string recordId, UPSEPricing pricing)
            : this(row?.RootRecordIdentification, row?.ValuesWithFunctions(), pricing)
        {
            this.ArticleRecordId = recordId;
        }

        /// <summary>
        /// Gets the article record identifier.
        /// </summary>
        /// <value>
        /// The article record identifier.
        /// </value>
        public string ArticleRecordId { get; private set; }

        /// <summary>
        /// Gets the discount scale.
        /// </summary>
        /// <value>
        /// The discount scale.
        /// </value>
        public List<double> DiscountScale { get; private set; }

        /// <summary>
        /// Gets the price scale.
        /// </summary>
        /// <value>
        /// The price scale.
        /// </value>
        public List<double> PriceScale { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has price scale.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has price scale; otherwise, <c>false</c>.
        /// </value>
        public bool HasPriceScale => this.PriceScale?.Count > 0;

        /// <summary>
        /// Gets a value indicating whether this instance has discount scale.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has discount scale; otherwise, <c>false</c>.
        /// </value>
        public bool HasDiscountScale => this.DiscountScale?.Count > 0;

        private void FillScalesFromDataDictionary(Dictionary<string, object> dataDictionary)
        {
            List<double> priceScale = new List<double>();
            bool hasItem = false;
            double lastUnitPrice = this.UnitPrice * this.ExchangeRate;

            for (int i = 0; i < 10; i++)
            {
                double val = Convert.ToDouble(dataDictionary.ValueOrDefault($"UnitPrice{i}"), System.Globalization.CultureInfo.InvariantCulture);
                if (val > 0)
                {
                    lastUnitPrice = val * this.ExchangeRate;
                    hasItem = true;
                }

                priceScale.Add(lastUnitPrice);
            }

            if (hasItem)
            {
                this.PriceScale = priceScale;
                return;
            }

            List<double> discountScale = new List<double>();
            double lastDiscount = 0;

            for (int i = 0; i < 10; i++)
            {
                double val = Convert.ToDouble(dataDictionary.ValueOrDefault($"Discount{i}"), System.Globalization.CultureInfo.InvariantCulture);
                if (val > 0)
                {
                    lastDiscount = val;
                    hasItem = true;
                }

                discountScale.Add(lastDiscount);
            }

            if (hasItem)
            {
                this.DiscountScale = discountScale;
            }
        }
    }
}
