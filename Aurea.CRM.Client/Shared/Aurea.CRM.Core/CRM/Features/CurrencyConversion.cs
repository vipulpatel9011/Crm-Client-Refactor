// <copyright file="CurrencyConversion.cs" company="Aurea Software Gmbh">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// Currency conversion implementation
    /// </summary>
    public class CurrencyConversion : ISearchOperationHandler
    {
        private UPContainerMetaInfo crmQuery;
        private Dictionary<int, object> currencyDictionary;
        private FieldControl fieldControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyConversion"/> class.
        /// </summary>
        /// <param name="searchAndListConfigName">Search and list config name</param>
        public CurrencyConversion(string searchAndListConfigName)
        {
            this.SearchAndListConfigName = searchAndListConfigName;
        }

        /// <summary>
        /// Gets default conversion
        /// </summary>
        public static CurrencyConversion DefaultConversion
        {
            get
            {
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                string currencyConversionSearchAndListName = configStore.ConfigValue("System.CurrencyConversion");
                if (currencyConversionSearchAndListName.Length == 0)
                {
                    if (configStore.SearchAndListByName("WKCurrencyConversion") != null)
                    {
                        currencyConversionSearchAndListName = "WKCurrencyConversion";
                    }
                    else if (configStore.SearchAndListByName("WTCurrencyConversion") != null)
                    {
                        currencyConversionSearchAndListName = "WTCurrencyConversion";
                    }
                }

                CurrencyConversion conversion = new CurrencyConversion(currencyConversionSearchAndListName);
                return conversion.Load() ? conversion : null;
            }
        }

        /// <summary>
        /// Gets base currency
        /// </summary>
        public Currency BaseCurrency { get; private set; }

        /// <summary>
        /// Gets list of currencies
        /// </summary>
        public List<object> Currencies => this.currencyDictionary.Values.ToList();

        /// <summary>
        /// Gets currency delegate
        /// </summary>
        public ICurrencyConversionDelegate CurrencyDelegate { get; private set; }

        /// <summary>
        /// Gets search and list config name
        /// </summary>
        public string SearchAndListConfigName { get; private set; }

        /// <summary>
        /// Exchange rate from code to code
        /// </summary>
        /// <param name="fromCode">From code</param>
        /// <param name="toCode">To code</param>
        /// <returns>Exchange rate</returns>
        public double ExchangeRateFromCodeToCode(int fromCode, int toCode)
        {
            if (fromCode == toCode)
            {
                return 1.0;
            }

            Currency fromCurrency = this.CurrencyFromCode(fromCode);
            if (fromCurrency != null)
            {
                if (fromCurrency.BaseCatalogCode == toCode)
                {
                    return fromCurrency.ExchangeRate;
                }

                if (fromCurrency.BaseCatalogCode2 == toCode)
                {
                    return fromCurrency.ExchangeRate2;
                }
            }

            Currency toCurrency = this.CurrencyFromCode(toCode);
            if (toCurrency != null)
            {
                if (toCurrency.BaseCatalogCode == fromCode && toCurrency.ExchangeRate > 0)
                {
                    return 1 / toCurrency.ExchangeRate;
                }

                if (toCurrency.BaseCatalogCode2 == fromCode && toCurrency.ExchangeRate2 > 0)
                {
                    return 1 / toCurrency.ExchangeRate2;
                }

                if (fromCurrency != null && toCurrency.ExchangeRate != 0.0 && fromCurrency.BaseCatalogCode == toCurrency.BaseCatalogCode)
                {
                    return fromCurrency.ExchangeRate / toCurrency.ExchangeRate;
                }
            }

            return 1.0;
        }

        /// <inheritdoc/>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            ICurrencyConversionDelegate currencyDelegate = this.CurrencyDelegate;
            this.CurrencyDelegate = null;
            this.crmQuery = null;
            currencyDelegate.CurrencyConversionDidFailWithError(this, error);
        }

        /// <inheritdoc/>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
        }

        /// <inheritdoc/>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
        }

        /// <inheritdoc/>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            ICurrencyConversionDelegate currencyDelegate = this.CurrencyDelegate;
            this.HandleResult(result);
            this.CurrencyDelegate = null;
            this.crmQuery = null;
            currencyDelegate.CurrencyConversionDidFinishWithResult(this, this);
        }

        /// <inheritdoc/>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"baseCurrency: {this.BaseCurrency}\nall: {this.Currencies}";
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndList = configStore.SearchAndListByName(this.SearchAndListConfigName);
            UPConfigFilter filter = null;
            if (searchAndList != null)
            {
                filter = configStore.FilterByName(searchAndList.FilterName);
            }

            if (searchAndList == null)
            {
                return false;
            }

            this.fieldControl = configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName);
            if (this.fieldControl != null)
            {
                this.crmQuery = new UPContainerMetaInfo(this.fieldControl, filter, null);
            }

            if (this.crmQuery == null)
            {
                return false;
            }

            var result = this.crmQuery.Find();
            this.HandleResult(result);
            return true;
        }

        /// <summary>
        /// Loads the specified request option.
        /// </summary>
        /// <param name="requestOption">The request option.</param>
        /// <param name="currencyDelegate">The currency delegate.</param>
        /// <returns></returns>
        public bool Load(UPRequestOption requestOption, ICurrencyConversionDelegate currencyDelegate)
        {
            if (this.CurrencyDelegate != null)
            {
                return false;
            }

            this.CurrencyDelegate = currencyDelegate;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndList = configStore.SearchAndListByName(this.SearchAndListConfigName);
            UPConfigFilter filter = null;
            if (searchAndList != null)
            {
                filter = configStore.FilterByName(searchAndList.FilterName);
            }

            this.fieldControl = configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName);
            if (this.fieldControl != null)
            {
                this.crmQuery = new UPContainerMetaInfo(this.fieldControl, filter, null);
            }

            if (this.crmQuery == null)
            {
                this.CurrencyDelegate = null;
                this.crmQuery = null;
                currencyDelegate.CurrencyConversionDidFailWithError(this, new Exception($"invalid searchAndList configuration {this.SearchAndListConfigName}"));
                return true;
            }

            this.crmQuery.Find(requestOption, this);
            return true;
        }

        private void AddCurrency(Currency currency)
        {
            this.currencyDictionary.SetObjectForKey(currency, currency.CatalogCode);
            if (currency.IsBaseCurrency)
            {
                this.BaseCurrency = currency;
            }
        }

        private Currency CurrencyFromCode(int code)
        {
            return (Currency)this.currencyDictionary[code];
        }

        private void HandleResult(UPCRMResult result)
        {
            this.currencyDictionary = new Dictionary<int, object>();
            if (result != null && result.RowCount > 0)
            {
                UPConfigFieldControlField currencyField = null, baseCurrencyField = null, exchangeRateField = null, baseCurrency2Field = null, exchangeRate2Field = null;
                foreach (FieldControlTab tab in this.fieldControl.Tabs)
                {
                    foreach (UPConfigFieldControlField field in tab.Fields)
                    {
                        if (field.Function == "Currency")
                        {
                            currencyField = field;
                        }
                        else if (field.Function == "BaseCurrency")
                        {
                            baseCurrencyField = field;
                        }
                        else if (field.Function == "ExchangeRate")
                        {
                            exchangeRateField = field;
                        }
                        else if (field.Function == "BaseCurrency2")
                        {
                            baseCurrency2Field = field;
                        }
                        else if (field.Function == "ExchangeRate2")
                        {
                            exchangeRate2Field = field;
                        }
                    }
                }

                if (currencyField != null && baseCurrencyField != null && exchangeRateField != null && result.RowCount > 0)
                {
                    int i, count = result.RowCount;
                    for (i = 0; i < count; i++)
                    {
                        UPCRMResultRow row = result.ResultRowAtIndex(i) as UPCRMResultRow;
                        int code = row.RawValueAtIndex(currencyField.TabIndependentFieldIndex).ToInt();
                        int baseCurrencyValue = row.RawValueAtIndex(baseCurrencyField.TabIndependentFieldIndex).ToInt();
                        double exchangeRate = row.RawValueAtIndex(exchangeRateField.TabIndependentFieldIndex).ToDouble();
                        double exchangeRate2 = 0;
                        int baseCurrency2Value = 0;
                        if (baseCurrency2Field != null && exchangeRate2Field != null)
                        {
                            baseCurrency2Value = row.RawValueAtIndex(baseCurrency2Field.TabIndependentFieldIndex).ToInt();
                            exchangeRate2 = row.RawValueAtIndex(exchangeRate2Field.TabIndependentFieldIndex).ToDouble();
                        }

                        this.AddCurrency(new Currency(code, baseCurrencyValue, exchangeRate, baseCurrency2Value, exchangeRate2));
                    }
                }
            }
        }
    }
}
