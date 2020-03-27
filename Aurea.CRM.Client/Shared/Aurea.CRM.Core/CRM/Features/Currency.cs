// <copyright file="Currency.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.CRM.Features
{
    /// <summary>
    /// Currency implementation
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Currency"/> class.
        /// </summary>
        /// <param name="catalogCode">Catalog code</param>
        /// <param name="baseCode">Base code</param>
        /// <param name="exchangeRate">Exchange rate</param>
        /// <param name="baseCode2">Base code 2</param>
        /// <param name="exchangeRate2">Exchange rate 2</param>
        public Currency(int catalogCode, int baseCode, double exchangeRate, int baseCode2, double exchangeRate2)
        {
            this.CatalogCode = catalogCode;
            this.BaseCatalogCode = baseCode;
            this.ExchangeRate = exchangeRate;
            this.ExchangeRate2 = exchangeRate2;
            this.BaseCatalogCode2 = baseCode2;
        }

        /// <summary>
        /// Gets base catalog code
        /// </summary>
        public int BaseCatalogCode { get; private set; }

        /// <summary>
        /// Gets base catalog code 2
        /// </summary>
        public int BaseCatalogCode2 { get; private set; }

        /// <summary>
        /// Gets catalog code
        /// </summary>
        public int CatalogCode { get; private set; }

        /// <summary>
        /// Gets exchange rate
        /// </summary>
        public double ExchangeRate { get; private set; }

        /// <summary>
        /// Gets exchange rate 2
        /// </summary>
        public double ExchangeRate2 { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is base currency
        /// </summary>
        public bool IsBaseCurrency => this.CatalogCode == this.BaseCatalogCode;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"code={this.CatalogCode} base={this.BaseCatalogCode} rate={this.ExchangeRate.ToString("0.00")} base2={this.BaseCatalogCode2} rate2={this.ExchangeRate2.ToString("0.00")}";
        }
    }
}
