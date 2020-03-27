// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingSet.cs" company="Aurea Software Gmbh">
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
//   Serial Entry Pricing Set
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Serial Entry Pricing Set
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPSEPricingSet : ISearchOperationHandler
    {
        private int loadStep;
        private bool fastRequest;
        private UPContainerMetaInfo currentQuery;
        private Dictionary<string, UPSEPricingCondition> conditionDictionary;
        private Dictionary<string, UPSEBundlePricing> bundleDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingSet"/> class.
        /// </summary>
        /// <param name="configName">Name of the configuration.</param>
        /// <param name="scaleConfigName">Name of the scale configuration.</param>
        /// <param name="bundleConfigName">Name of the bundle configuration.</param>
        /// <param name="bundleScaleConfigName">Name of the bundle scale configuration.</param>
        /// <param name="pricing">The pricing.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPSEPricingSet(string configName, string scaleConfigName, string bundleConfigName, string bundleScaleConfigName,
            UPSEPricing pricing, UPSEPricingSetDelegate theDelegate)
        {
            this.fastRequest = ServerSession.CurrentSession.IsEnterprise;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.ConfigSearchAndList = configStore.SearchAndListByName(configName);
            this.ConfigFieldControl = this.ConfigSearchAndList != null ? configStore.FieldControlByNameFromGroup("List", this.ConfigSearchAndList.FieldGroupName) : null;
            if (this.ConfigFieldControl == null)
            {
                throw new InvalidOperationException("ConfigFieldControl is null");
            }

            this.TheDelegate = theDelegate;
            this.Pricing = pricing;
            if (!string.IsNullOrEmpty(scaleConfigName))
            {
                this.ScaleConfigSearchAndList = configStore.SearchAndListByName(scaleConfigName);
                this.ScaleConfigFieldControl = this.ScaleConfigSearchAndList != null ? configStore.FieldControlByNameFromGroup("List", this.ScaleConfigSearchAndList.FieldGroupName) : null;
            }

            if (!string.IsNullOrEmpty(bundleConfigName))
            {
                this.BundleConfigSearchAndList = configStore.SearchAndListByName(bundleConfigName);
                this.BundleConfigFieldControl = this.BundleConfigSearchAndList != null ? configStore.FieldControlByNameFromGroup("List", this.BundleConfigSearchAndList.FieldGroupName) : null;

                if (!string.IsNullOrEmpty(bundleScaleConfigName))
                {
                    this.BundleScaleConfigSearchAndList = configStore.SearchAndListByName(bundleScaleConfigName);
                    this.BundleScaleConfigFieldControl = this.BundleScaleConfigSearchAndList != null ? configStore.FieldControlByNameFromGroup("List", this.BundleScaleConfigSearchAndList.FieldGroupName) : null;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingSet"/> class.
        /// </summary>
        /// <param name="configName">Name of the configuration.</param>
        /// <param name="scaleConfigName">Name of the scale configuration.</param>
        /// <param name="pricing">The pricing.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPSEPricingSet(string configName, string scaleConfigName, UPSEPricing pricing, UPSEPricingSetDelegate theDelegate)
            : this(configName, scaleConfigName, null, null, pricing, theDelegate)
        {
        }

        /// <summary>
        /// Gets the configuration search and list.
        /// </summary>
        /// <value>
        /// The configuration search and list.
        /// </value>
        public SearchAndList ConfigSearchAndList { get; private set; }

        /// <summary>
        /// Gets the configuration field control.
        /// </summary>
        /// <value>
        /// The configuration field control.
        /// </value>
        public FieldControl ConfigFieldControl { get; private set; }

        /// <summary>
        /// Gets the scale configuration search and list.
        /// </summary>
        /// <value>
        /// The scale configuration search and list.
        /// </value>
        public SearchAndList ScaleConfigSearchAndList { get; private set; }

        /// <summary>
        /// Gets the scale configuration field control.
        /// </summary>
        /// <value>
        /// The scale configuration field control.
        /// </value>
        public FieldControl ScaleConfigFieldControl { get; private set; }

        /// <summary>
        /// Gets the bundle configuration search and list.
        /// </summary>
        /// <value>
        /// The bundle configuration search and list.
        /// </value>
        public SearchAndList BundleConfigSearchAndList { get; private set; }

        /// <summary>
        /// Gets the bundle configuration field control.
        /// </summary>
        /// <value>
        /// The bundle configuration field control.
        /// </value>
        public FieldControl BundleConfigFieldControl { get; private set; }

        /// <summary>
        /// Gets the bundle scale configuration search and list.
        /// </summary>
        /// <value>
        /// The bundle scale configuration search and list.
        /// </value>
        public SearchAndList BundleScaleConfigSearchAndList { get; private set; }

        /// <summary>
        /// Gets the bundle scale configuration field control.
        /// </summary>
        /// <value>
        /// The bundle scale configuration field control.
        /// </value>
        public FieldControl BundleScaleConfigFieldControl { get; private set; }

        /// <summary>
        /// Gets or sets the filter parameters.
        /// </summary>
        /// <value>
        /// The filter parameters.
        /// </value>
        public Dictionary<string, object> FilterParameters { get; set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPSEPricingSetDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets the pricing.
        /// </summary>
        /// <value>
        /// The pricing.
        /// </value>
        public UPSEPricing Pricing { get; private set; }

        /// <summary>
        /// Gets the base query.
        /// </summary>
        /// <value>
        /// The base query.
        /// </value>
        public UPContainerMetaInfo BaseQuery => new UPContainerMetaInfo(this.ConfigSearchAndList, this.FilterParameters);

        /// <summary>
        /// Gets the base scale query.
        /// </summary>
        /// <value>
        /// The base scale query.
        /// </value>
        public UPContainerMetaInfo BaseScaleQuery
        {
            get
            {
                if (this.ScaleConfigSearchAndList != null)
                {
                    UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(this.ScaleConfigSearchAndList, this.FilterParameters);
                    if (this.ConfigSearchAndList.FilterName != null)
                    {
                        UPConfigFilter baseFilter = ConfigurationUnitStore.DefaultStore.FilterByName(this.ConfigSearchAndList.FilterName);
                        if (baseFilter != null)
                        {
                            if (this.FilterParameters != null)
                            {
                                baseFilter = baseFilter.FilterByApplyingReplacements(new UPConditionValueReplacement(this.FilterParameters));
                            }

                            crmQuery.ApplyFilter(baseFilter);
                        }
                    }

                    return crmQuery;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the bundle query.
        /// </summary>
        /// <value>
        /// The bundle query.
        /// </value>
        public UPContainerMetaInfo BundleQuery 
            => this.BundleConfigSearchAndList != null ? new UPContainerMetaInfo(this.BundleConfigSearchAndList, this.FilterParameters) : null;

        /// <summary>
        /// Gets the bundle scale query.
        /// </summary>
        /// <value>
        /// The bundle scale query.
        /// </value>
        public UPContainerMetaInfo BundleScaleQuery
        {
            get
            {
                if (this.BundleScaleConfigSearchAndList != null)
                {
                    UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(this.BundleScaleConfigSearchAndList, this.FilterParameters);
                    if (this.BundleConfigSearchAndList.FilterName != null)
                    {
                        UPConfigFilter baseFilter = ConfigurationUnitStore.DefaultStore.FilterByName(this.BundleConfigSearchAndList.FilterName);
                        if (baseFilter != null)
                        {
                            if (this.FilterParameters != null)
                            {
                                baseFilter = baseFilter.FilterByApplyingReplacements(new UPConditionValueReplacement(this.FilterParameters));
                            }

                            crmQuery.ApplyFilter(baseFilter);
                        }
                    }

                    return crmQuery;
                }

                return null;
            }
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            this.StartBaseQuery();
        }

        /// <summary>
        /// Bundles the pricing for key.
        /// </summary>
        /// <param name="bundlePricingKey">The bundle pricing key.</param>
        /// <returns></returns>
        public UPSEBundlePricing BundlePricingForKey(string bundlePricingKey)
        {
            return this.bundleDictionary.ValueOrDefault(bundlePricingKey);
        }

        /// <summary>
        /// Conditionses for data key order row record identifier.
        /// </summary>
        /// <param name="matchDictionary">The match dictionary.</param>
        /// <param name="keyOrder">The key order.</param>
        /// <param name="rowRecordId">The row record identifier.</param>
        /// <returns></returns>
        public UPSEPricingCondition ConditionsForDataKeyOrderRowRecordId(Dictionary<string, object> matchDictionary, List<string> keyOrder, string rowRecordId)
        {
            int matchIndex = keyOrder.Count + 1;
            UPSEPricingCondition matchingCondition = null;
            foreach (UPSEPricingCondition condition in this.conditionDictionary.Values)
            {
                int currentMatchIndex = condition.MatchingIndexKeyOrderMaxMatchIndex(matchDictionary, keyOrder, matchIndex - 1);
                if (currentMatchIndex >= 0)
                {
                    if (currentMatchIndex < matchIndex)
                    {
                        matchingCondition = condition;
                        matchIndex = currentMatchIndex;
                        if (matchIndex == 0)
                        {
                            break;
                        }
                    }
                }
            }

            return matchingCondition;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.TheDelegate?.PricingSetDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            switch (this.loadStep)
            {
                case 0:
                    this.HandleBaseResult(result);
                    break;

                case 1:
                    this.HandleBaseScaleResult(result);
                    break;

                case 2:
                    this.HandleBundleResult(result);
                    break;

                case 3:
                    this.HandleBundleScaleResult(result);
                    break;

                default:
                    break;
            }
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

        private void FinishedLoadingSuccessfully()
        {
            this.TheDelegate?.PricingSetDidFinishWithResult(this, null);
        }

        private void HandleBaseResult(UPCRMResult result)
        {
            int count = result.RowCount;
            if (count > 0)
            {
                this.conditionDictionary = new Dictionary<string, UPSEPricingCondition>(count);
                int bundleKeyIndex = 0;
                if (this.BundleConfigFieldControl != null)
                {
                    int infoAreaIndex = this.currentQuery.IndexOfResultInfoAreaIdLinkId(this.BundleConfigFieldControl.InfoAreaId, -1);
                    if (infoAreaIndex > 0)
                    {
                        bundleKeyIndex = infoAreaIndex;
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    UPSEPricingCondition condition = new UPSEPricingCondition(
                        (UPCRMResultRow)result.ResultRowAtIndex(i), this.ConfigFieldControl, bundleKeyIndex, this);

                    this.conditionDictionary[condition.RecordIdentification] = condition;
                }

                this.StartBaseScaleQuery();
            }
            else
            {
#if  PRICINGDEMO
                this.conditionDictionary = NSMutableDictionary.TheNew();
                this.bundleDictionary = NSMutableDictionary.TheNew();
                UPSEPricingCondition condition = UPSEPricingCondition.DemoBase();
                UPSEBundlePricing bundlePricing = UPSEPricingCondition.DemoBundle();
                this.conditionDictionary.SetObjectForKey(condition, condition.RecordIdentification);
                this.bundleDictionary.SetObjectForKey(bundlePricing, bundlePricing.RecordIdentification);
#endif

                this.FinishedLoadingSuccessfully();
            }
        }

        private void HandleBaseScaleResult(UPCRMResult result)
        {
            int count = result.RowCount;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                    string conditionRecordIdentification = row.RecordIdentificationAtIndex(1);
                    if (!string.IsNullOrEmpty(conditionRecordIdentification))
                    {
                        UPSEPricingCondition root = this.conditionDictionary.ValueOrDefault(row.RecordIdentificationAtIndex(1));
                        if (root != null)
                        {
                            UPSEPricingScale pricingScale = new UPSEPricingScale(row, this.ScaleConfigFieldControl, this.Pricing);
                            root.AddScale(pricingScale);
                        }
                    }
                }
            }

            this.StartBundleQuery();
        }

        private void HandleBundleResult(UPCRMResult result)
        {
            int count = result.RowCount;
            if (count > 0)
            {
                this.bundleDictionary = new Dictionary<string, UPSEBundlePricing>(count);
                for (int i = 0; i < count; i++)
                {
                    UPSEBundlePricing bundle = new UPSEBundlePricing((UPCRMResultRow)result.ResultRowAtIndex(i), this.ConfigFieldControl, this.Pricing);
                    this.bundleDictionary[bundle.RecordIdentification] = bundle;
                }

                this.StartBundleScaleQuery();
            }
            else
            {
                this.FinishedLoadingSuccessfully();
            }
        }

        private void HandleBundleScaleResult(UPCRMResult result)
        {
            int count = result.RowCount;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                    string bundleRecordIdentification = row.RecordIdentificationAtIndex(1);
                    if (!string.IsNullOrEmpty(bundleRecordIdentification))
                    {
                        UPSEBundlePricing root = this.bundleDictionary.ValueOrDefault(row.RecordIdentificationAtIndex(1));
                        if (root != null)
                        {
                            UPSEBundlePricingScale bundleScale = new UPSEBundlePricingScale(row, this.BundleScaleConfigFieldControl, this.Pricing);
                            root.AddScale(bundleScale);
                        }
                    }
                }
            }

            this.FinishedLoadingSuccessfully();
        }

        private void StartBaseQuery()
        {
            this.loadStep = 0;
            this.currentQuery = this.BaseQuery;
            if (this.currentQuery != null)
            {
                this.currentQuery.Find(this.Pricing.SerialEntry.RequestOption, this);
            }
            else
            {
                this.FinishedLoadingSuccessfully();
            }
        }

        private void StartBaseScaleQuery()
        {
            this.currentQuery = this.BaseScaleQuery;
            if (this.currentQuery != null)
            {
                this.loadStep = 1;
                this.currentQuery.Find(this.Pricing.SerialEntry.RequestOption, this);
            }
            else
            {
                this.StartBundleQuery();
            }
        }

        private void StartBundleQuery()
        {
            this.currentQuery = this.BundleQuery;
            if (this.currentQuery != null)
            {
                this.loadStep = 2;
                this.currentQuery.Find(this.Pricing.SerialEntry.RequestOption, this);
            }
            else
            {
                this.FinishedLoadingSuccessfully();
            }
        }

        private void StartBundleScaleQuery()
        {
            this.currentQuery = this.BundleScaleQuery;
            if (this.currentQuery != null)
            {
                this.loadStep = 3;
                this.currentQuery.Find(this.Pricing.SerialEntry.RequestOption, this);
            }
            else
            {
                this.FinishedLoadingSuccessfully();
            }
        }
    }
}
