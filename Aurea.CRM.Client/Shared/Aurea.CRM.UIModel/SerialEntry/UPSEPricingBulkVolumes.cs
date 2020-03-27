// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingBulkVolumes.cs" company="Aurea Software Gmbh">
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
//   Pricing Bulk Volumes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// Serial Entry Pricing Bulk Volumes
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPSEPricingBulkVolumes : ISearchOperationHandler
    {
        private UPContainerMetaInfo crmQuery;
        private Dictionary<string, UPSEPricingBulkVolume> bulkVolumePerItemNumber;
        private List<UPSEPricingBulkVolumeMatch> otherBulkVolumeMatches;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingBulkVolumes"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <exception cref="InvalidOperationException">FieldControl is null</exception>
        public UPSEPricingBulkVolumes(ViewReference viewReference, UPSEPricingBulkVolumesDelegate theDelegate)
        {
            this.TheDelegate = theDelegate;
            string configName = viewReference.ContextValueForKey("BulkVolumeConfigName");
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (!string.IsNullOrEmpty(configName))
            {
                this.SearchAndListConfig = configStore.SearchAndListByName(configName);
                if (this.SearchAndListConfig != null)
                {
                    this.FieldControl = configStore.FieldControlByNameFromGroup("List", this.SearchAndListConfig.FieldGroupName);
                }
            }

            if (this.FieldControl == null)
            {
                throw new InvalidOperationException("FieldControl is null");
            }

            this.ItemNumberFunctionName = viewReference.ContextValueForKey("BulkVolumeItemNumber");
            if (string.IsNullOrEmpty(this.ItemNumberFunctionName))
            {
                this.ItemNumberFunctionName = "ItemNumber";
            }

            configName = viewReference.ContextValueForKey("BulkVolumeMatchFunctionNames");
            if (!string.IsNullOrEmpty(configName))
            {
                var parts = configName.Split(';');
                List<UPSEPricingBulkVolumeMatch> otherMatches = new List<UPSEPricingBulkVolumeMatch>(parts.Length);
                otherMatches.AddRange(parts.Select(part => new UPSEPricingBulkVolumeMatch(part)).Where(otherMatch => otherMatch != null));

                if (otherMatches.Count > 0)
                {
                    this.otherBulkVolumeMatches = otherMatches;
                }
            }
        }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPSEPricingBulkVolumesDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets the search and list configuration.
        /// </summary>
        /// <value>
        /// The search and list configuration.
        /// </value>
        public SearchAndList SearchAndListConfig { get; private set; }

        /// <summary>
        /// Gets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; private set; }

        /// <summary>
        /// Gets the name of the item number function.
        /// </summary>
        /// <value>
        /// The name of the item number function.
        /// </value>
        public string ItemNumberFunctionName { get; private set; }

        /// <summary>
        /// Loads the specified data dictionary.
        /// </summary>
        /// <param name="dataDictionary">The data dictionary.</param>
        /// <param name="requestOption">The request option.</param>
        public void Load(Dictionary<string, object> dataDictionary, UPRequestOption requestOption)
        {
            this.crmQuery = new UPContainerMetaInfo(this.SearchAndListConfig, dataDictionary);
            this.crmQuery.Find(requestOption, this);
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.TheDelegate.PricingBulkVolumesDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            int count = result.RowCount;
            Dictionary<string, UPSEPricingBulkVolume> bulkVolumnPerItemNumber = new Dictionary<string, UPSEPricingBulkVolume>();
            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                UPSEPricingBulkVolume volume = new UPSEPricingBulkVolume(row, this);
                if (volume == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(volume.ItemNumber))
                {
                    bulkVolumnPerItemNumber[volume.ItemNumber] = volume;
                }
                else
                {
                    foreach (UPSEPricingBulkVolumeMatch bulkVolumeMatch in this.otherBulkVolumeMatches)
                    {
                        if (bulkVolumeMatch.AddBulkVolume(volume))
                        {
                            break;
                        }
                    }
                }
            }

            if (bulkVolumnPerItemNumber.Count > 0)
            {
                this.bulkVolumePerItemNumber = bulkVolumnPerItemNumber;
            }

            this.TheDelegate.PricingBulkVolumesDidFinishWithSuccess(this, this);
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
        /// Bulks the volume for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public UPSEPricingBulkVolume BulkVolumeForRow(UPSERow row)
        {
            Dictionary<string, object> sourceFunctionValues = row.SourceValueDictionary;
            if (!sourceFunctionValues.ContainsKey(this.ItemNumberFunctionName))
            {
                string copyItemNumber = sourceFunctionValues.ValueOrDefault($"Copy{this.ItemNumberFunctionName}") as string;
                if (!string.IsNullOrEmpty(copyItemNumber))
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>(sourceFunctionValues);
                    dict[this.ItemNumberFunctionName] = copyItemNumber;
                    sourceFunctionValues = dict;
                }
            }

            return this.BulkVolumeForData(sourceFunctionValues);
        }

        /// <summary>
        /// Bulks the volume for data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public UPSEPricingBulkVolume BulkVolumeForData(Dictionary<string, object> data)
        {
            string itemNumber = data.ValueOrDefault(this.ItemNumberFunctionName) as string;
            if (!string.IsNullOrEmpty(itemNumber))
            {
                UPSEPricingBulkVolume bulkVolume = this.bulkVolumePerItemNumber.ValueOrDefault(itemNumber);
                if (bulkVolume != null)
                {
                    return bulkVolume;
                }
            }

            foreach (UPSEPricingBulkVolumeMatch volumeMatch in this.otherBulkVolumeMatches)
            {
                UPSEPricingBulkVolume bulkVolume = volumeMatch.BulkVolumeForDictionary(data);
                if (bulkVolume != null)
                {
                    return bulkVolume;
                }
            }

            return null;
        }

        /// <summary>
        /// Bulks the quantity index for row quantity.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public int BulkQuantityIndexForRowQuantity(UPSERow row, double quantity)
        {
            if (quantity < 0)
            {
                string quantityString = row.ValueForFunctionName("Quantity");
                if (!string.IsNullOrEmpty(quantityString))
                {
                    quantity = Convert.ToDouble(quantityString, System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            UPSEPricingBulkVolume bulkVolume = this.BulkVolumeForRow(row);
            if (bulkVolume != null)
            {
                return bulkVolume.IndexForQuantity(quantity);
            }

            return -1;
        }
    }
}
