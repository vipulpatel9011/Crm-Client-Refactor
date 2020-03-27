// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricing.cs" company="Aurea Software Gmbh">
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
//   Serial Entry Pricing
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// Pricing name - item number
        /// </summary>
        public const string UPPricingName_ItemNumber = "ItemNumber";

        /// <summary>
        /// Pricing name - quantity prefix
        /// </summary>
        public const string UPPricingName_QuantityPrefix = "Quantity";

        /// <summary>
        /// Pricing name - price prefix
        /// </summary>
        public const string UPPricingName_PricePrefix = "Price";

        /// <summary>
        /// Pricing name - discount prefix
        /// </summary>
        public const string UPPricingName_DiscountPrefix = "Discount";
    }

    /// <summary>
    /// Serial Entry Pricing
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEPricingSetDelegate" />
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEPricingBulkVolumesDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="ICurrencyConversionDelegate" />
    public class UPSEPricing : UPSEPricingSetDelegate, UPSEPricingBulkVolumesDelegate, ISearchOperationHandler, ICurrencyConversionDelegate
    {
        private int loadStep;
        private UPContainerMetaInfo currentQuery;
        private Dictionary<string, UPSEPrice> priceForArticle;
        private Dictionary<string, UPSERowPricing> rowPricingForArticle;
        private Dictionary<string, object> filterParameters;


        /// <summary>
        /// Gets logging interface
        /// </summary>
        public static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricing"/> class.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <exception cref="InvalidOperationException">PriceListFieldControl is null</exception>
        public UPSEPricing(UPSerialEntry serialEntry, ViewReference viewReference, UPSEPricingDelegate theDelegate)
        {
            this.SerialEntry = serialEntry;
            this.TheDelegate = theDelegate;
            string configName = viewReference.ContextValueForKey("PriceListConfigName");
            if (!string.IsNullOrEmpty(configName))
            {
                this.PriceListSearchAndList = ConfigurationUnitStore.DefaultStore.SearchAndListByName(configName);
                if (this.PriceListSearchAndList != null)
                {
                    this.PriceListFieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", this.PriceListSearchAndList.FieldGroupName);
                }
            }

            configName = viewReference.ContextValueForKey("PriceListPriorityColumns");
            if (string.IsNullOrEmpty(configName))
            {
                this.PriceListPriorityColumns = new List<string> { "PriceList" };
            }
            else if (configName.ToUpper() == "NONE")
            {
                this.PriceListPriorityColumns = null;
            }
            else
            {
                this.PriceListPriorityColumns = configName.Split(',').ToList();
            }

            configName = viewReference.ContextValueForKey("BulkVolumeConfigName");
            if (!string.IsNullOrEmpty(configName))
            {
                this.BulkVolumes = new UPSEPricingBulkVolumes(viewReference, this);
            }

            if (this.PriceListFieldControl == null)
            {
                throw new InvalidOperationException("PriceListFieldControl is null");
            }

            configName = viewReference.ContextValueForKey("ConditionConfigName");
            if (!string.IsNullOrEmpty(configName))
            {
                string scaleConfigName = viewReference.ContextValueForKey("ConditionScaleConfigName");
                this.StandardPricing = new UPSEPricingSet(configName, scaleConfigName, this, this);
            }

            configName = viewReference.ContextValueForKey("CompanyConfigName");
            if (!string.IsNullOrEmpty(configName))
            {
                string scaleConfigName = viewReference.ContextValueForKey("CompanyScaleConfigName");
                this.CompanySpecificPricing = new UPSEPricingSet(configName, scaleConfigName, this, this);
            }

            configName = viewReference.ContextValueForKey("ActionConfigName");
            if (!string.IsNullOrEmpty(configName))
            {
                string scaleConfigName = viewReference.ContextValueForKey("ActionScaleConfigName");
                string bundleConfigName = viewReference.ContextValueForKey("BundleConfigName");
                string bundleScaleConfigName = viewReference.ContextValueForKey("BundleScaleConfigName");
                this.ActionPricing = new UPSEPricingSet(configName, scaleConfigName, bundleConfigName, bundleScaleConfigName, this, this);
            }

            configName = viewReference.ContextValueForKey("FunctionNameApplyOrder");
            this.FunctionNameApplyOrder = !string.IsNullOrEmpty(configName)
                                            ? configName.Split(',').ToList()
                                            : new List<string> { "ItemNumber" };

            configName = viewReference.ContextValueForKey("Options");
            if (!string.IsNullOrEmpty(configName))
            {
                this.Options = configName.JsonDictionaryFromString();
            }

            if (this.SerialEntry.ExplicitItemNumberFunctionName)
            {
                this.PricingItemNumber = viewReference.ContextValueForKey("ItemNumberFunctionName");
                if (string.IsNullOrEmpty(this.PricingItemNumber))
                {
                    this.PricingItemNumber = "ItemNumber";
                }
            }

            configName = viewReference.ContextValueForKey("CurrencyConversionConfigName");
            if (!string.IsNullOrEmpty(configName))
            {
                this.CurrencyConversion = new CurrencyConversion(configName);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [dont update row prices].
        /// </summary>
        /// <value>
        /// <c>true</c> if [dont update row prices]; otherwise, <c>false</c>.
        /// </value>
        public bool DontUpdateRowPrices => Convert.ToInt32(this.Options.ValueOrDefault("DontUpdateRowPrices")) != 0;

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Gets the standard pricing.
        /// </summary>
        /// <value>
        /// The standard pricing.
        /// </value>
        public UPSEPricingSet StandardPricing { get; private set; }

        /// <summary>
        /// Gets the action pricing.
        /// </summary>
        /// <value>
        /// The action pricing.
        /// </value>
        public UPSEPricingSet ActionPricing { get; private set; }

        /// <summary>
        /// Gets the company specific pricing.
        /// </summary>
        /// <value>
        /// The company specific pricing.
        /// </value>
        public UPSEPricingSet CompanySpecificPricing { get; private set; }

        /// <summary>
        /// Gets the function name apply order.
        /// </summary>
        /// <value>
        /// The function name apply order.
        /// </value>
        public List<string> FunctionNameApplyOrder { get; private set; }

        /// <summary>
        /// Gets the price list field control.
        /// </summary>
        /// <value>
        /// The price list field control.
        /// </value>
        public FieldControl PriceListFieldControl { get; private set; }

        /// <summary>
        /// Gets the price list search and list.
        /// </summary>
        /// <value>
        /// The price list search and list.
        /// </value>
        public SearchAndList PriceListSearchAndList { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPSEPricingDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public Dictionary<string, object> Options { get; private set; }

        /// <summary>
        /// Gets the bulk volumes.
        /// </summary>
        /// <value>
        /// The bulk volumes.
        /// </value>
        public UPSEPricingBulkVolumes BulkVolumes { get; private set; }

        /// <summary>
        /// Gets the pricing item number.
        /// </summary>
        /// <value>
        /// The pricing item number.
        /// </value>
        public string PricingItemNumber { get; private set; }

        /// <summary>
        /// Gets the currency conversion.
        /// </summary>
        /// <value>
        /// The currency conversion.
        /// </value>
        public CurrencyConversion CurrencyConversion { get; private set; }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public int Currency { get; private set; }

        /// <summary>
        /// Gets the price list priority columns.
        /// </summary>
        /// <value>
        /// The price list priority columns.
        /// </value>
        public List<string> PriceListPriorityColumns { get; private set; }

        /// <summary>
        /// Gets the overall discount.
        /// </summary>
        /// <value>
        /// The overall discount.
        /// </value>
        public double OverallDiscount { get; private set; }

        /// <summary>
        /// Gets the overall discount price.
        /// </summary>
        /// <value>
        /// The overall discount price.
        /// </value>
        public double OverallDiscountPrice { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has overall discount.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has overall discount; otherwise, <c>false</c>.
        /// </value>
        public bool HasOverallDiscount { get; private set; }

        /// <summary>
        /// Loads the specified filter parameters.
        /// </summary>
        /// <param name="filterParameters">The filter parameters.</param>
        public void Load(Dictionary<string, object> filterParameters)
        {
            this.Currency = Convert.ToInt32(filterParameters.ValueOrDefault("Currency"));
            if (this.Currency == 0)
            {
                this.Currency = Convert.ToInt32(filterParameters.ValueOrDefault("CopyCurrency"));
            }

            string overallDiscountString = filterParameters.ValueOrDefault("OverallDiscount") as string;
            if (!string.IsNullOrEmpty(overallDiscountString))
            {
                this.OverallDiscount = Convert.ToDouble(overallDiscountString, System.Globalization.CultureInfo.InvariantCulture);
                overallDiscountString = filterParameters.ValueOrDefault("OverallDiscountPrice") as string;
                this.OverallDiscountPrice = Convert.ToDouble(overallDiscountString, System.Globalization.CultureInfo.InvariantCulture);

                if (this.OverallDiscountPrice != 0 && this.OverallDiscount != 0)
                {
                    this.HasOverallDiscount = true;
                }
            }
            else
            {
                this.HasOverallDiscount = false;
            }

            this.filterParameters = filterParameters;
            if (this.StandardPricing != null)
            {
                this.StandardPricing.FilterParameters = this.filterParameters;
            }

            if (this.ActionPricing != null)
            {
                this.ActionPricing.FilterParameters = this.filterParameters;
            }

            if (this.CompanySpecificPricing != null)
            {
                this.CompanySpecificPricing.FilterParameters = this.filterParameters;
            }

            if (this.Currency > 0 && this.CurrencyConversion != null)
            {
                this.CurrencyConversion.Load(this.SerialEntry.RequestOption, this);
            }
            else
            {
                this.HandleCurrencyResult();
            }
        }

        /// <summary>
        /// Prices for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public UPSERowPricing PriceForRow(UPSERow row)
        {
            UPSERowPricing rowPricing;
            if (!string.IsNullOrEmpty(this.PricingItemNumber))
            {
                string itemNumber = null;
                if (!string.IsNullOrEmpty(this.SerialEntry.ItemNumberFunctionName))
                {
                    itemNumber = row.ValueForFunctionName(this.SerialEntry.ItemNumberFunctionName);
                }

                if (itemNumber == null)
                {
                    return null;
                }

                rowPricing = this.rowPricingForArticle.ValueOrDefault(itemNumber);
                if (rowPricing != null)
                {
                    return rowPricing;
                }

                UPSEPrice price = this.priceForArticle.ValueOrDefault(itemNumber);
                string rowRecordId = row.RowRecordId;
                if (price == null)
                {
                    Dictionary<string, object> dataDictionary = row.SourceFunctionValues();
                    price = new UPSEPrice(StringExtensions.InfoAreaIdRecordId(this.SerialEntry.SourceInfoAreaId, rowRecordId), dataDictionary, this);
                }

                if (price != null)
                {
                    rowPricing = new UPSERowPricing(price, this, rowRecordId);
                    if (this.rowPricingForArticle == null)
                    {
                        this.rowPricingForArticle = new Dictionary<string, UPSERowPricing>();
                    }

                    this.rowPricingForArticle[itemNumber] = rowPricing;

                    return rowPricing;
                }
            }
            else
            {
                string rowRecordId = row.RowRecordId;
                rowPricing = this.rowPricingForArticle.ValueOrDefault(rowRecordId);
                if (rowPricing != null)
                {
                    return rowPricing;
                }

                UPSEPrice price = this.priceForArticle.ValueOrDefault(rowRecordId);
                if (price == null)
                {
                    Dictionary<string, object> dataDictionary = row.SourceFunctionValues();
                    price = new UPSEPrice(StringExtensions.InfoAreaIdRecordId(this.SerialEntry.SourceInfoAreaId, rowRecordId), dataDictionary, this);
                }

                if (price != null)
                {
                    rowPricing = new UPSERowPricing(price, this, rowRecordId);
                    if (this.rowPricingForArticle == null)
                    {
                        this.rowPricingForArticle = new Dictionary<string, UPSERowPricing>();
                    }

                    this.rowPricingForArticle[rowRecordId] = rowPricing;
                    return rowPricing;
                }
            }

            return null;
        }

        /// <summary>
        /// Conditionses for data row record identifier.
        /// </summary>
        /// <param name="dataDictionary">The data dictionary.</param>
        /// <param name="rowRecordId">The row record identifier.</param>
        /// <returns></returns>
        public List<UPSEPricingConditionBase> ConditionsForDataRowRecordId(Dictionary<string, object> dataDictionary, string rowRecordId)
        {
            UPSEPricingCondition pricingCondition;
            List<UPSEPricingConditionBase> pricingConditions = new List<UPSEPricingConditionBase>();
            if (this.ActionPricing != null)
            {
                pricingCondition = this.ActionPricing.ConditionsForDataKeyOrderRowRecordId(dataDictionary, this.FunctionNameApplyOrder, rowRecordId);
                if (pricingCondition != null)
                {
                    pricingConditions.Add(pricingCondition);
                }
            }

            if (this.CompanySpecificPricing != null)
            {
                pricingCondition = this.CompanySpecificPricing.ConditionsForDataKeyOrderRowRecordId(dataDictionary, this.FunctionNameApplyOrder, rowRecordId);
                if (pricingCondition != null)
                {
                    pricingConditions.Add(pricingCondition);
                }
            }

            if (this.StandardPricing != null)
            {
                pricingCondition = this.StandardPricing.ConditionsForDataKeyOrderRowRecordId(dataDictionary, this.FunctionNameApplyOrder, rowRecordId);
                if (pricingCondition != null)
                {
                    pricingConditions.Add(pricingCondition);
                }
            }

            return pricingConditions.Count > 0 ? pricingConditions : null;
        }

        /// <summary>
        /// Pricings the set did finish with result.
        /// </summary>
        /// <param name="pricingSet">The pricing set.</param>
        /// <param name="data">The data.</param>
        public void PricingSetDidFinishWithResult(UPSEPricingSet pricingSet, object data)
        {
            if (this.loadStep < 1 && this.StandardPricing != null)
            {
                this.loadStep = 1;
                this.StandardPricing.Load();
                return;
            }

            if (this.loadStep < 2 && this.ActionPricing != null)
            {
                this.loadStep = 2;
                this.ActionPricing.Load();
                return;
            }

            if (this.loadStep < 3 && this.CompanySpecificPricing != null)
            {
                this.loadStep = 3;
                this.CompanySpecificPricing.Load();
                return;
            }

            this.TheDelegate?.PricingDidFinishWithResult(this, null);
        }

        /// <summary>
        /// Pricings the set did fail with error.
        /// </summary>
        /// <param name="pricingSet">The pricing set.</param>
        /// <param name="error">The error.</param>
        public void PricingSetDidFailWithError(UPSEPricingSet pricingSet, Exception error)
        {
            this.TheDelegate?.PricingDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.TheDelegate?.PricingDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.HandlePricingResult(result);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pricings the bulk volumes did finish with success.
        /// </summary>
        /// <param name="bulkVolumes">The bulk volumes.</param>
        /// <param name="result">The result.</param>
        public void PricingBulkVolumesDidFinishWithSuccess(UPSEPricingBulkVolumes bulkVolumes, object result)
        {
            this.HandleBulkVolumesResult();
        }

        /// <summary>
        /// Pricings the bulk volumes did fail with error.
        /// </summary>
        /// <param name="bulkVolumes">The bulk volumes.</param>
        /// <param name="error">The error.</param>
        public void PricingBulkVolumesDidFailWithError(UPSEPricingBulkVolumes bulkVolumes, Exception error)
        {
            this.TheDelegate?.PricingDidFailWithError(this, error);
        }

        /// <summary>
        /// Currencies the conversion did finish with result.
        /// </summary>
        /// <param name="currencyConversion">The currency conversion.</param>
        /// <param name="result">The result.</param>
        public void CurrencyConversionDidFinishWithResult(CurrencyConversion currencyConversion, object result)
        {
            this.HandleCurrencyResult();
        }

        /// <summary>
        /// Currencies the conversion did fail with error.
        /// </summary>
        /// <param name="currencyConversion">The currency conversion.</param>
        /// <param name="error">The error.</param>
        public void CurrencyConversionDidFailWithError(CurrencyConversion currencyConversion, Exception error)
        {
            this.TheDelegate.PricingDidFailWithError(this, error);
        }

        private void HandlePricingResult(UPCRMResult result)
        {
            int count = result.RowCount;
            Dictionary<string, UPSEPrice> articleDictionary = new Dictionary<string, UPSEPrice>(count);
            bool pricingByItemNumber = this.PricingItemNumber.Length > 0;
            int pricingItemNumberIndex = 0;
            if (pricingByItemNumber)
            {
                pricingItemNumberIndex = result.MetaInfo.IndexOfFunctionName(this.PricingItemNumber);
                Logger.LogError($"cannot execute pricing by item number because function name {this.PricingItemNumber} was not found in the pricing result");
                if (pricingItemNumberIndex < 0)
                {
                    pricingByItemNumber = false;
                }
            }

            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                string articleRecordId = pricingByItemNumber ? row.RawValueAtIndex(pricingItemNumberIndex) : row.RecordIdentificationAtIndex(1).RecordId();

                if (articleRecordId == null)
                {
                    continue;
                }

                UPSEPrice price = new UPSEPrice(row, articleRecordId, this);
                UPSEPrice existingPrice = articleDictionary.ValueOrDefault(articleRecordId);
                if (existingPrice == null)
                {
                    articleDictionary.SetObjectForKey(price, articleRecordId);
                }
                else
                {
                    bool checkCurrency = true;
                    foreach (string priorityColumn in this.PriceListPriorityColumns)
                    {
                        string existingValue = existingPrice.DataDictionary.ValueOrDefault(priorityColumn) as string;
                        string currentValue = price.DataDictionary.ValueOrDefault(priorityColumn) as string;

                        if (string.IsNullOrEmpty(existingValue) || existingValue == "0")
                        {
                            if (!string.IsNullOrEmpty(currentValue) && currentValue != "0")
                            {
                                articleDictionary[articleRecordId] = price;
                                checkCurrency = false;
                                break;
                            }
                        }
                        else if (string.IsNullOrEmpty(currentValue) || currentValue == "0")
                        {
                            checkCurrency = false;
                            break;
                        }
                    }

                    if (checkCurrency)
                    {
                        if (price.Currency > 0 && this.Currency == price.Currency)
                        {
                            articleDictionary.SetObjectForKey(price, articleRecordId);
                        }
                        else if (existingPrice.Currency != this.Currency)
                        {
                            int baseCurrency = 0;
                            if (this.CurrencyConversion != null)
                            {
                                baseCurrency = this.CurrencyConversion.BaseCurrency.CatalogCode;
                            }
                            if (baseCurrency != existingPrice.Currency)
                            {
                                if (price.Currency == baseCurrency)
                                {
                                    articleDictionary.SetObjectForKey(price, articleRecordId);
                                }
                            }
                        }
                    }
                }
            }

            this.priceForArticle = articleDictionary;
            if (this.BulkVolumes != null)
            {
                this.BulkVolumes.Load(this.filterParameters, this.SerialEntry.RequestOption);
            }
            else
            {
                this.HandleBulkVolumesResult();
            }
        }

        private void HandleBulkVolumesResult()
        {
            this.loadStep = 0;
            this.PricingSetDidFinishWithResult(null, null);
        }

        private void HandleCurrencyResult()
        {
            this.loadStep = 0;
            this.currentQuery = new UPContainerMetaInfo(this.PriceListSearchAndList, this.filterParameters);
            this.currentQuery.Find(this.SerialEntry.RequestOption, this, false);
        }
    }
}
