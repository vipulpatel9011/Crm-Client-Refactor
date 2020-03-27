// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEQuotaHandler.cs" company="Aurea Software Gmbh">
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
//   UPSEQuotaHandler
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

    public class UPSEQuotaHandler : UPCRMLinkReaderDelegate, ISearchOperationHandler
    {
        private Dictionary<string, UPSEArticleQuotaConfiguration> articleConfigurationDictionary;
        private Dictionary<string, UPSEQuota> quotaDictionary;
        private Dictionary<string, UPSERowQuota> rowQuotaDictionary;
        private uint loadStep;
        private UPCRMLinkReader linkReader;
        private UPContainerMetaInfo currentQuery;
        private string linkRecordIdentification;

        /// <summary>
        /// Gets a value indicating whether this instance has quota columns.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has quota columns; otherwise, <c>false</c>.
        /// </value>
        public bool HasQuotaColumns => this.DestinationColumns.Count > 0;

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPSEQuotaHandlerDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public ViewReference Configuration { get; private set; }

        /// <summary>
        /// Gets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public UPRequestOption RequestOption { get; private set; }

        /// <summary>
        /// Gets the article configuration search and list.
        /// </summary>
        /// <value>
        /// The article configuration search and list.
        /// </value>
        public SearchAndList ArticleConfigurationSearchAndList { get; private set; }

        /// <summary>
        /// Gets the quota search and list.
        /// </summary>
        /// <value>
        /// The quota search and list.
        /// </value>
        public SearchAndList QuotaSearchAndList { get; private set; }

        /// <summary>
        /// Gets the parent link string.
        /// </summary>
        /// <value>
        /// The parent link string.
        /// </value>
        public string ParentLinkString { get; private set; }

        /// <summary>
        /// Gets the default quota per year.
        /// </summary>
        /// <value>
        /// The default quota per year.
        /// </value>
        public int DefaultQuotaPerYear { get; private set; }

        /// <summary>
        /// Gets the number of quota years.
        /// </summary>
        /// <value>
        /// The number of quota years.
        /// </value>
        public int NumberOfQuotaYears { get; private set; }

        /// <summary>
        /// Gets the name of the row item number function.
        /// </summary>
        /// <value>
        /// The name of the row item number function.
        /// </value>
        public string RowItemNumberFunctionName { get; private set; }

        /// <summary>
        /// Gets the name of the item number function.
        /// </summary>
        /// <value>
        /// The name of the item number function.
        /// </value>
        public string ItemNumberFunctionName { get; private set; }

        /// <summary>
        /// Gets the quota field function names.
        /// </summary>
        /// <value>
        /// The quota field function names.
        /// </value>
        public List<string> QuotaFieldFunctionNames { get; private set; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets the destination columns.
        /// </summary>
        /// <value>
        /// The destination columns.
        /// </value>
        public List<UPSEColumn> DestinationColumns { get; private set; }

        /// <summary>
        /// Gets the default quota per year without configuration.
        /// </summary>
        /// <value>
        /// The default quota per year without configuration.
        /// </value>
        public int DefaultQuotaPerYearWithoutConfiguration { get; private set; }

        /// <summary>
        /// Gets the quota link identifier.
        /// </summary>
        /// <value>
        /// The quota link identifier.
        /// </value>
        public int QuotaLinkId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [allow quota exceed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow quota exceed]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowQuotaExceed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [automatic correct quota].
        /// </summary>
        /// <value>
        /// <c>true</c> if [automatic correct quota]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoCorrectQuota { get; private set; }

        /// <summary>
        /// Gets the quota edit field control.
        /// </summary>
        /// <value>
        /// The quota edit field control.
        /// </value>
        public FieldControl QuotaEditFieldControl { get; private set; }

        /// <summary>
        /// Gets the quota template filter.
        /// </summary>
        /// <value>
        /// The quota template filter.
        /// </value>
        public UPConfigFilter QuotaTemplateFilter { get; private set; }

        /// <summary>
        /// Gets the filter parameters.
        /// </summary>
        /// <value>
        /// The filter parameters.
        /// </value>
        public Dictionary<string, object> FilterParameters { get; private set; }

        /// <summary>
        /// Gets the link record.
        /// </summary>
        /// <value>
        /// The link record.
        /// </value>
        public UPCRMRecord LinkRecord { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [calendar year periods].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [calendar year periods]; otherwise, <c>false</c>.
        /// </value>
        public bool CalendarYearPeriods { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [hide zero quota].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hide zero quota]; otherwise, <c>false</c>.
        /// </value>
        public bool HideZeroQuota { get; private set; }

        private static int GetIntVal(object objVal)
        {
            int result = 0;

            int.TryParse(objVal + string.Empty, out result);

            return result;
        }

        /// <summary>
        /// Creates the specified serial entry.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="date">The date.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPSEQuotaHandler Create(UPSerialEntry serialEntry, ViewReference configuration, DateTime date,
            UPSEQuotaHandlerDelegate theDelegate)
        {
            try
            {
                return new UPSEQuotaHandler(serialEntry, configuration, date, theDelegate);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEQuotaHandler"/> class.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="date">The date.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <exception cref="Exception">
        /// ArticleConfigurationSearchAndList is null
        /// or
        /// QuotaSearchAndList is null
        /// </exception>
        private UPSEQuotaHandler(UPSerialEntry serialEntry, ViewReference configuration, DateTime date, UPSEQuotaHandlerDelegate theDelegate)
        {
            this.SerialEntry = serialEntry;
            this.Configuration = configuration;
            this.TheDelegate = theDelegate;
            this.Date = date;
            var configStore = ConfigurationUnitStore.DefaultStore;

            this.InitializeBaseConfigStore(configStore);
            this.InitializeMax();

            this.ParentLinkString = this.Configuration.ContextValueForKey("ParentLink");
            this.RequestOption = UPCRMDataStore.RequestOptionFromString(
                this.Configuration.ContextValueForKey("RequestOption"),
                UPRequestOption.FastestAvailable);
            this.ItemNumberFunctionName = this.Configuration.ContextValueForKey("ItemNumberFunctionName");

            if (string.IsNullOrWhiteSpace(this.ItemNumberFunctionName))
            {
                this.ItemNumberFunctionName = "ItemNumber";
            }

            this.RowItemNumberFunctionName = this.Configuration.ContextValueForKey("RowItemNumberFunctionName");
            if (string.IsNullOrWhiteSpace(this.RowItemNumberFunctionName))
            {
                this.RowItemNumberFunctionName = this.ItemNumberFunctionName;
            }

            this.InitializeQuotaFieldNames();

            var configName = this.Configuration.ContextValueForKey("QuotaLinkId");
            if (!string.IsNullOrWhiteSpace(configName))
            {
                int qLinkId = -1;
                int.TryParse(configName, out qLinkId);
                this.QuotaLinkId = qLinkId;
            }
            else
            {
                this.QuotaLinkId = -1;
            }

            this.InitializeConfigOptions();

            configName = this.Configuration.ContextValueForKey("NewQuotaTemplateFilter");
            if (!string.IsNullOrWhiteSpace(configName))
            {
                this.QuotaTemplateFilter = configStore.FilterByName(configName);
            }
        }

        private void InitializeBaseConfigStore(IConfigurationUnitStore configStore)
        {
            var configName = this.Configuration.ContextValueForKey("ArticleConfigName");
            this.ArticleConfigurationSearchAndList = configStore.SearchAndListByName(configName);

            if (this.ArticleConfigurationSearchAndList == null)
            {
                throw new Exception("ArticleConfigurationSearchAndList is null");
            }

            configName = this.Configuration.ContextValueForKey("QuotaConfigName");
            this.QuotaSearchAndList = configStore.SearchAndListByName(configName);

            if (this.QuotaSearchAndList == null)
            {
                throw new Exception("QuotaSearchAndList is null");
            }

            this.QuotaEditFieldControl = configStore.FieldControlByNameFromGroup("Edit", this.QuotaSearchAndList.FieldGroupName) 
                ?? configStore.FieldControlByNameFromGroup("List", this.QuotaSearchAndList.FieldGroupName);
        }

        private void InitializeMax()
        {
            var configName = this.Configuration.ContextValueForKey("MaxSamplesPerPeriod");
            this.DefaultQuotaPerYear = 2;

            if (!string.IsNullOrWhiteSpace(configName))
            {
                int quotaYear = this.DefaultQuotaPerYear;
                int.TryParse(configName, out quotaYear);
                this.DefaultQuotaPerYear = quotaYear;
            }

            configName = this.Configuration.ContextValueForKey("MaxPeriods");
            this.NumberOfQuotaYears = 2;
            if (!string.IsNullOrWhiteSpace(configName))
            {
                int nYears = this.NumberOfQuotaYears;
                int.TryParse(configName, out nYears);
                this.NumberOfQuotaYears = nYears;
            }

            this.CalendarYearPeriods = this.Configuration.ContextValueIsSet("CalendarYearPeriods");
            configName = this.Configuration.ContextValueForKey("MaxSamplesPerPeriodNoConfiguration");

            if (!string.IsNullOrWhiteSpace(configName))
            {
                int defQuota = 0;
                int.TryParse(configName, out defQuota);
                this.DefaultQuotaPerYearWithoutConfiguration = defQuota;
            }
            else
            {
                this.DefaultQuotaPerYearWithoutConfiguration = 0;
            }
        }

        private void InitializeQuotaFieldNames()
        {
            var configName = this.Configuration.ContextValueForKey("QuotaFieldFunctionNames");

            if (!string.IsNullOrWhiteSpace(configName))
            {
                this.QuotaFieldFunctionNames = configName.Split(',').ToList();
            }
            else
            {
                this.QuotaFieldFunctionNames = new List<string> { "ItemsSent", "ItemsIssued" };
            }

            var destCol = new List<UPSEColumn>();
            foreach (var functionName in this.QuotaFieldFunctionNames)
            {
                var column = this.SerialEntry.ColumnForFunctionName(functionName);
                if (column != null && column is UPSEDestinationColumnBase)
                {
                    destCol.Add(column);
                }
            }

            if (destCol.Any())
            {
                this.DestinationColumns = destCol;
            }
        }

        private void InitializeConfigOptions()
        {
            this.AutoCorrectQuota = true;
            this.AllowQuotaExceed = false;
            this.HideZeroQuota = true;
            var configName = this.Configuration.ContextValueForKey("Options");

            if (!string.IsNullOrWhiteSpace(configName))
            {
                var optionDictionary = configName.JsonDictionaryFromString();
                var objVal = optionDictionary.ValueOrDefault("autoCorrectQuota");

                if (objVal != null)
                {
                    this.AutoCorrectQuota = GetIntVal(objVal) != 0;
                }

                objVal = optionDictionary.ValueOrDefault("allowQuotaExceed");
                if (objVal != null)
                {
                    this.AllowQuotaExceed = GetIntVal(objVal) != 0;
                }

                objVal = optionDictionary.ValueOrDefault("hideZeroQuota");
                if (objVal != null)
                {
                    this.HideZeroQuota = GetIntVal(objVal) != 0;
                }
                else
                {
                    objVal = optionDictionary.ValueOrDefault("showZeroQuota");
                    if (objVal != null)
                    {
                        this.HideZeroQuota = GetIntVal(objVal) == 0;
                    }
                }
            }
        }

        private void LoadArticleConfigurations()
        {
            this.loadStep = 1;
            this.currentQuery = new UPContainerMetaInfo(this.ArticleConfigurationSearchAndList, this.FilterParameters);
            this.currentQuery.Find(this.RequestOption, this);
        }

        private void LoadQuota()
        {
            this.loadStep = 2;
            this.currentQuery = new UPContainerMetaInfo(this.QuotaSearchAndList, this.FilterParameters);
            if (!string.IsNullOrEmpty(this.linkRecordIdentification))
            {
                this.currentQuery.SetLinkRecordIdentification(this.linkRecordIdentification, this.QuotaLinkId);
            }

            this.currentQuery.Find(this.RequestOption, this);
        }

        private void ApplyArticleConfigurationResult(UPCRMResult result)
        {
            int count = result.RowCount;
            this.articleConfigurationDictionary = new Dictionary<string, UPSEArticleQuotaConfiguration>();
            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                Dictionary<string, object> dict = row.ValuesWithFunctions();
                string itemNumber = dict.ValueOrDefault(this.ItemNumberFunctionName) as string;
                if (string.IsNullOrEmpty(itemNumber))
                {
                    continue;
                }

                if (!this.articleConfigurationDictionary.ContainsKey(itemNumber))
                {
                    UPSEArticleQuotaConfiguration articleConfiguration = new UPSEArticleQuotaConfiguration(itemNumber, dict, row.RootRecordIdentification);
                    this.articleConfigurationDictionary[itemNumber] = articleConfiguration;
                }
            }

            this.LoadQuota();
        }

        private void ApplyQuotaResult(UPCRMResult result)
        {
            int count = result.RowCount;
            this.quotaDictionary = new Dictionary<string, UPSEQuota>();
            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                Dictionary<string, object> dict = row.ValuesWithFunctions();
                string itemNumber = dict.ValueOrDefault(this.ItemNumberFunctionName) as string;
                if (string.IsNullOrEmpty(itemNumber))
                {
                    continue;
                }

                UPSEQuota quota = this.quotaDictionary.ValueOrDefault(itemNumber);
                if (quota == null)
                {
                    quota = new UPSEQuota(itemNumber, this);
                    this.quotaDictionary.SetObjectForKey(quota, itemNumber);
                }

                quota.ApplyRecordIdentificationValues(row.RootRecordIdentification, dict);
            }

            this.Finished();
        }

        private void Finished()
        {
            foreach (UPSERow row in this.SerialEntry.Positions)
            {
                int sum = row.SumForDestinationColumns(this.DestinationColumns);
                row.RowQuota.InitialCount = sum;
            }

            this.TheDelegate.SerialEntryQuotaHandlerDidFinishWithResult(this, null);
        }

        /// <summary>
        /// Loads for link record.
        /// </summary>
        /// <param name="linkRecord">The link record.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        public void LoadForLinkRecord(UPCRMRecord linkRecord, Dictionary<string, object> filterParameters)
        {
            this.FilterParameters = filterParameters;
            this.LinkRecord = linkRecord;
            this.loadStep = 0;
            if (this.ParentLinkString != null)
            {
                this.linkReader = new UPCRMLinkReader(this.LinkRecord.RecordIdentification, this.ParentLinkString, this);
                this.linkReader.Start();
            }
            else
            {
                this.linkRecordIdentification = this.LinkRecord.RecordIdentification;
                this.LoadArticleConfigurations();
            }
        }

        /// <summary>
        /// Rows the quota for item number.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <returns></returns>
        public UPSERowQuota RowQuotaForItemNumber(string itemNumber)
        {
            UPSERowQuota rowQuota = this.rowQuotaDictionary.ValueOrDefault(itemNumber);
            if (rowQuota == null)
            {
                UPSEQuota quota = this.quotaDictionary.ValueOrDefault(itemNumber);
                if (quota == null)
                {
                    quota = new UPSEQuota(itemNumber, this);
                    this.quotaDictionary.SetObjectForKey(quota, itemNumber);
                }

                rowQuota = new UPSERowQuota(itemNumber, this.articleConfigurationDictionary[itemNumber], quota, this);
                if (this.rowQuotaDictionary != null)
                {
                    this.rowQuotaDictionary.SetObjectForKey(rowQuota, itemNumber);
                }
                else
                {
                    this.rowQuotaDictionary = new Dictionary<string, UPSERowQuota> { { itemNumber, rowQuota } };
                }
            }

            return rowQuota;
        }

        /// <summary>
        /// Rows the quota for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public UPSERowQuota RowQuotaForRow(UPSERow row)
        {
            string itemNumber = row.ValueForFunctionName(this.RowItemNumberFunctionName);
            if (!string.IsNullOrEmpty(itemNumber))
            {
                return this.RowQuotaForItemNumber(itemNumber);
            }

            return null;
        }

        /// <summary>
        /// Changeds the records.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecords()
        {
            UPCRMRecord syncRecord = new UPCRMRecord(this.QuotaSearchAndList.InfoAreaId, "Sync");
            syncRecord.AddLink(new UPCRMLink(this.linkRecordIdentification, this.QuotaLinkId));
            List<UPCRMRecord> changedRecords = new List<UPCRMRecord> { syncRecord };
            foreach (UPSERowQuota rowQuota in this.rowQuotaDictionary.Values)
            {
                List<UPCRMRecord> changedRecordsForRow = rowQuota.ChangedRecords();
                if (changedRecordsForRow.Count > 0)
                {
                    changedRecords.AddRange(changedRecordsForRow);
                }
            }

            return changedRecords;
        }

        /// <summary>
        /// Determines whether [is quota column] [the specified destination column].
        /// </summary>
        /// <param name="destinationColumn">The destination column.</param>
        /// <returns>
        ///   <c>true</c> if [is quota column] [the specified destination column]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQuotaColumn(UPSEDestinationColumnBase destinationColumn)
        {
            return this.IsQuotaColumnIndex(destinationColumn.Index);
        }

        /// <summary>
        /// Determines whether [is quota column index] [the specified column index].
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>
        ///   <c>true</c> if [is quota column index] [the specified column index]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQuotaColumnIndex(int columnIndex)
        {
            foreach (UPSEColumn column in this.DestinationColumns)
            {
                if (column.Index == columnIndex)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader _linkReader, object result)
        {
            this.linkRecordIdentification = _linkReader.DestinationRecordIdentification;
            this.LoadArticleConfigurations();
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception error)
        {
            this.TheDelegate.SerialEntryQuotaHandlerDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.TheDelegate.SerialEntryQuotaHandlerDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (this.loadStep == 1)
            {
                this.ApplyArticleConfigurationResult(result);
            }
            else if (this.loadStep == 2)
            {
                this.ApplyQuotaResult(result);
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
    }
}
