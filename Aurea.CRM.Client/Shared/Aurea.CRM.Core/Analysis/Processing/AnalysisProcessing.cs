// <copyright file="AnalysisProcessing.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Processing
{
    using System.Collections.Generic;
    using System.Linq;
    using CRM;
    using CRM.Features;
    using DataSource;
    using Drilldown;
    using Extensions;
    using Model;
    using Result;

    /// <summary>
    /// Implementation of analysis processing
    /// </summary>
    public class AnalysisProcessing
    {
        private int initialSortColumnIndex;
        private List<object> resultColumns;
        private Dictionary<string, AnalysisProcessingXCategoryValue> xCategories;
        private List<AnalysisProcessingXCategoryValue> xCategoryArray;
        private Dictionary<string, AnalysisProcessingYCategoryValue> yCategories;
        private List<AnalysisProcessingYCategoryValue> yCategoryArray;
        private AnalysisProcessingYCategoryValue sumYCategoryValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessing"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="settings">Settings</param>
        /// <param name="dataSource">Data source</param>
        public AnalysisProcessing(Analysis analysis, AnalysisExecutionSettings settings, ICrmDataSource dataSource)
        {
            this.Analysis = analysis;
            this.Settings = settings;
            this.DataSource = dataSource;
            if (this.Settings.ResultColumns.Count > 0)
            {
                this.resultColumns = new List<object>();
                foreach (AnalysisResultColumn col in this.Settings.ResultColumns)
                {
                    this.resultColumns.Add(new AnalysisProcessingResultColumn(col, this));
                }
            }
        }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets currency conversion
        /// </summary>
        public CurrencyConversion CurrencyConversion
        {
            get
            {
                return this.Analysis.CurrencyConversion;
            }
        }

        /// <summary>
        /// Gets data source
        /// </summary>
        public ICrmDataSource DataSource { get; private set; }

        /// <summary>
        /// Gets result columns
        /// </summary>
        public List<object> ResultColumns => this.resultColumns;

        /// <summary>
        /// Gets settings
        /// </summary>
        public AnalysisExecutionSettings Settings { get; private set; }

        /// <summary>
        /// Gets sum y category value
        /// </summary>
        public AnalysisProcessingYCategoryValue SumYCategoryValue
        {
            get
            {
                if (this.sumYCategoryValue == null)
                {
                    this.sumYCategoryValue = new AnalysisProcessingYCategoryValue(this);
                }

                return this.sumYCategoryValue;
            }
        }

        /// <summary>
        /// Adjusts value for currency
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <param name="currencyString">Currency string</param>
        /// <returns>Return adjusted value for currency</returns>
        public double AdjustValueForCurrency(double originalValue, string currencyString)
        {
            CurrencyConversion currencyConversion = this.CurrencyConversion;
            if (currencyConversion == null)
            {
                return originalValue;
            }

            int currencyCode = currencyString.ToInt();
            int analysisCurrency = this.Settings.CurrencyCode;
            if (currencyCode == 0 || currencyCode == analysisCurrency || analysisCurrency == 0)
            {
                return originalValue;
            }

            var conversionRate = currencyConversion.ExchangeRateFromCodeToCode(currencyCode, analysisCurrency);
            if (conversionRate == 0 || conversionRate == 1)
            {
                return originalValue;
            }

            return originalValue / conversionRate;
        }

        /// <summary>
        /// Computes result
        /// </summary>
        /// <returns>Returns analysis result</returns>
        public AnalysisResult ComputeResult()
        {
            ComputeYCategoryValues();

            var result = new AnalysisResult(Analysis, DataSource);

            result = AddAnalysisResultColumns(result);

            result = SetSortColumn(result);

            result = AddAnalysisResultRows(result);

            result.SumLine = new AnalysisRow(result, SumYCategoryValue);

            result = AddDrillDownOptions(result);

            result = AddDrillUpOptions(result);

            return result;
        }

        /// <summary>
        /// Sorts y category by first column
        /// </summary>
        /// <returns>List of categories sorted by first column</returns>
        public List<AnalysisProcessingYCategoryValue> SortYCategoryByFirstColumn() => this.yCategoryArray.OrderBy(a => a.NumberResultAtIndex(this.initialSortColumnIndex)).ToList();

        /// <summary>
        /// Sorts y category by key
        /// </summary>
        /// <returns>List of categories sorted by key</returns>
        public List<AnalysisProcessingYCategoryValue> SortYCategoryByKey() => this.yCategoryArray.OrderBy(a => a.Key).ToList();

        /// <summary>
        /// Sorts y category by label
        /// </summary>
        /// <returns>List of categories sorted by label</returns>
        public List<AnalysisProcessingYCategoryValue> SortYCategoryByLabel() => this.yCategoryArray.OrderBy(a => a.Label).ToList();

        private bool RowMatchesFilters(ICrmDataSourceRow row)
        {
            if (this.Settings.Conditions != null)
            {
                foreach (AnalysisFilter condition in this.Settings.Conditions)
                {
                    if (!condition.MatchesRow(row))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private List<object> XCategoriesForRow(ICrmDataSourceRow row)
        {
            List<object> categoryValues = this.Settings.XCategory.CategoriesForRow(row);
            if (categoryValues.Count == 0)
            {
                return null;
            }

            List<object> apCategoryValues = new List<object>();
            foreach (AnalysisCategoryValue categoryValue in categoryValues)
            {
                var xcategory = this.xCategories.ValueOrDefault(categoryValue.Key);
                if (xcategory == null)
                {
                    xcategory = new AnalysisProcessingXCategoryValue(this, categoryValue);
                    if (this.xCategories == null)
                    {
                        this.xCategories = new Dictionary<string, AnalysisProcessingXCategoryValue> { { xcategory.Key, xcategory } };
                        this.xCategoryArray = new List<AnalysisProcessingXCategoryValue> { xcategory };
                    }
                    else
                    {
                        this.xCategories.SetObjectForKey(xcategory, xcategory.Key);
                        this.xCategoryArray.Add(xcategory);
                    }
                }

                apCategoryValues.Add(xcategory);
            }

            return apCategoryValues;
        }

        private AnalysisProcessingXCategoryValue XCategoryForRow(ICrmDataSourceRow row)
        {
            AnalysisCategoryValue categoryValue = this.Settings.XCategory?.CategoryValueForRow(row);
            if (categoryValue == null)
            {
                return null;
            }
            else
            {
                var xcategory = this.xCategories.ValueOrDefault(categoryValue.Key);
                if (xcategory == null)
                {
                    xcategory = new AnalysisProcessingXCategoryValue(this, categoryValue);
                    if (this.xCategories == null)
                    {
                        this.xCategories = new Dictionary<string, AnalysisProcessingXCategoryValue> { { xcategory.Key, xcategory } };
                        this.xCategoryArray = new List<AnalysisProcessingXCategoryValue> { xcategory };
                    }
                    else
                    {
                        this.xCategories.SetObjectForKey(xcategory, xcategory.Key);
                        this.xCategoryArray.Add(xcategory);
                    }
                }

                return xcategory;
            }
        }

        private List<object> YCategoriesForRow(ICrmDataSourceRow row)
        {
            List<object> categoryValues = this.Settings.Category.CategoriesForRow(row);
            if (categoryValues.Count == 0)
            {
                if (!this.Settings.ShowEmpty)
                {
                    return null;
                }

                AnalysisCategoryValue emptyValue = this.Settings.Category.EmptyValue();
                if (emptyValue != null)
                {
                    categoryValues = new List<object> { emptyValue };
                }
            }

            if (categoryValues.Count == 1)
            {
                var v = categoryValues[0] as AnalysisProcessingYCategoryValue;
                if (v.Key.Length == 0)
                {
                    if (!this.Settings.ShowEmpty)
                    {
                        return null;
                    }
                }
            }

            List<object> apCategoryValues = new List<object>();
            foreach (AnalysisCategoryValue categoryValue in categoryValues)
            {
                var ycategory = this.yCategories.ValueOrDefault(categoryValue.Key);
                if (ycategory == null)
                {
                    ycategory = new AnalysisProcessingYCategoryValue(this, categoryValue);
                    if (this.yCategories == null)
                    {
                        this.yCategories = new Dictionary<string, AnalysisProcessingYCategoryValue> { { ycategory.Key, ycategory } };
                        this.yCategoryArray = new List<AnalysisProcessingYCategoryValue> { ycategory };
                    }
                    else
                    {
                        this.yCategories.SetObjectForKey(ycategory, ycategory.Key);
                        this.yCategoryArray.Add(ycategory);
                    }
                }

                apCategoryValues.Add(ycategory);
            }

            if (apCategoryValues.Count == 0 && this.Settings.ShowEmpty)
            {
                AnalysisCategoryValue emptyValue = this.Settings.Category.EmptyValue();
                if (emptyValue != null)
                {
                    return new List<object> { emptyValue };
                }
            }

            return apCategoryValues;
        }

        private AnalysisProcessingYCategoryValue YCategoryForRow(ICrmDataSourceRow row)
        {
            AnalysisCategoryValue categoryValue = this.Settings.Category.CategoryValueForRow(row);
            if (categoryValue == null || categoryValue.Key.Length == 0)
            {
                if (this.Settings.ShowEmpty)
                {
                    categoryValue = this.Settings.Category.EmptyValue();
                }
                else
                {
                    return null;
                }
            }

            var ycategory = this.yCategories.ValueOrDefault(categoryValue.Key);
            if (ycategory == null)
            {
                ycategory = new AnalysisProcessingYCategoryValue(this, categoryValue);
                if (this.yCategories == null)
                {
                    this.yCategories = new Dictionary<string, AnalysisProcessingYCategoryValue> { { ycategory.Key, ycategory } };
                    this.yCategoryArray = new List<AnalysisProcessingYCategoryValue> { ycategory };
                }
                else
                {
                    this.yCategories.SetObjectForKey(ycategory, ycategory.Key);
                    this.yCategoryArray.Add(ycategory);
                }
            }

            return ycategory;
        }

        /// <summary>
        /// Compute Y Category Values
        /// </summary>
        private void ComputeYCategoryValues()
        {
            var rowCount = DataSource.RowCount;
            var sumComplete = true;
            if ((Settings?.Category?.ArrayCategory ?? false) || (Settings?.XCategory?.ArrayCategory ?? false))
            {
                sumComplete = AddXnYCategoriesValue(rowCount, sumComplete);
            }
            else
            {
                sumComplete = AddXnYCategoryValue(rowCount, sumComplete);
            }

            if (yCategoryArray != null)
            {
                foreach (AnalysisProcessingYCategoryValue yCategoryValue in yCategoryArray)
                {
                    yCategoryValue.ExecuteComputationStep();
                }
            }

            if (!sumComplete)
            {
                SumYCategoryValue.ExecuteComputationStep();
            }
        }

        /// <summary>
        /// Add X and Y Row Categories
        /// </summary>
        /// <param name="rowCount">
        /// Total records
        /// </param>
        /// <param name="sumComplete">
        /// Is sum complete
        /// </param>
        /// <returns>
        /// return IsSumComplete
        /// </returns>
        private bool AddXnYCategoriesValue(int rowCount, bool sumComplete)
        {
            for (var i = 0; i < rowCount; i++)
            {
                var row = DataSource.ResultRowAtIndex(i);
                if (!RowMatchesFilters(row))
                {
                    continue;
                }

                var yValues = YCategoriesForRow(row);
                if (yValues.Count == 0)
                {
                    continue;
                }

                var xValues = XCategoriesForRow(row);
                foreach (AnalysisProcessingYCategoryValue yCategoryValue in yValues)
                {
                    yCategoryValue.AddRowXCategoryValueArray(row, xValues);
                }

                if (!SumYCategoryValue.AddRowXCategoryValueArray(row, xValues))
                {
                    sumComplete = false;
                }
            }

            return sumComplete;
        }

        /// <summary>
        /// Add X and Y Row Category
        /// </summary>
        /// <param name="rowCount">
        /// Total records
        /// </param>
        /// <param name="sumComplete">
        /// Is sum complete
        /// </param>
        /// <returns>
        /// return IsSumComplete
        /// </returns>
        private bool AddXnYCategoryValue(int rowCount, bool sumComplete)
        {
            for (var i = 0; i < rowCount; i++)
            {
                var row = DataSource.ResultRowAtIndex(i);
                if (!RowMatchesFilters(row))
                {
                    continue;
                }

                var yCategoryValue = YCategoryForRow(row);
                if (yCategoryValue == null)
                {
                    continue;
                }

                var xCategoryValue = XCategoryForRow(row);
                yCategoryValue.AddRowXCategory(row, xCategoryValue);

                if (!SumYCategoryValue.AddRowXCategory(row, xCategoryValue))
                {
                    sumComplete = false;
                }
            }

            return sumComplete;
        }

        /// <summary>
        /// Add Columns to AnalysisResult object
        /// </summary>
        /// <param name="result">
        /// <see cref="AnalysisResult"/> object
        /// </param>
        /// <returns>
        /// <see cref="AnalysisResult"/> updated object
        /// </returns>
        private AnalysisResult AddAnalysisResultColumns(AnalysisResult result)
        {
            foreach (AnalysisProcessingResultColumn resultColumn in resultColumns)
            {
                var col = new AnalysisColumn(resultColumn.ResultColumn, xCategoryArray);
                result.AddColumn(col);
            }

            return result;
        }

        /// <summary>
        /// Sets AnalysisResult sort column value
        /// </summary>
        /// <param name="result">
        /// <see cref="AnalysisResult"/> object
        /// </param>
        /// <returns>
        /// <see cref="AnalysisResult"/> updated object
        /// </returns>
        private AnalysisResult SetSortColumn(AnalysisResult result)
        {
            var sortByFirstColumnValue = Settings.Category.SortByFirstColumnValue();
            if (sortByFirstColumnValue)
            {
                initialSortColumnIndex = 0;
                while (initialSortColumnIndex < result.Columns.Count && ((AnalysisColumn)result.Columns[initialSortColumnIndex]).IsTextColumn)
                {
                    initialSortColumnIndex++;
                }

                if (initialSortColumnIndex < result.Columns.Count)
                {
                    SortYCategoryByFirstColumn();
                    result.SortColumn = initialSortColumnIndex;
                }
                else
                {
                    sortByFirstColumnValue = false;
                }
            }

            if (!sortByFirstColumnValue)
            {
                if (Settings.Category.SortByKey())
                {
                    SortYCategoryByKey();
                    result.SortColumn = -1;
                }
                else
                {
                    SortYCategoryByLabel();
                    result.SortColumn = -1;
                }
            }

            return result;
        }

        /// <summary>
        /// Add Rows to AnalysisResult object
        /// </summary>
        /// <param name="result">
        /// <see cref="AnalysisResult"/> object
        /// </param>
        /// <returns>
        /// <see cref="AnalysisResult"/> updated object
        /// </returns>
        private AnalysisResult AddAnalysisResultRows(AnalysisResult result)
        {
            var maxNumberOfRows = Settings.Category.MaxNumberOfRows;
            if (maxNumberOfRows > 0 && yCategoryArray.Count > maxNumberOfRows)
            {
                var otherCategory = new List<AnalysisProcessingYCategoryValue>();
                foreach (var categoryValue in yCategoryArray)
                {
                    if (--maxNumberOfRows > 0)
                    {
                        var r = new AnalysisRow(result, categoryValue) as AnalysisRow;
                        result.AddRow(r);
                    }
                    else
                    {
                        otherCategory.Add(categoryValue);
                    }
                }

                var row = new AnalysisRow(result, otherCategory);
                result.AddRow(row);
            }
            else
            {
                foreach (var catVal in yCategoryArray)
                {
                    var row = new AnalysisRow(result, catVal);
                    result.AddRow(row);
                }
            }

            return result;
        }

        /// <summary>
        /// Add DrillDown Option
        /// </summary>
        /// <param name="result">
        /// <see cref="AnalysisResult"/> object
        /// </param>
        /// <returns>
        /// <see cref="AnalysisResult"/> updated object
        /// </returns>
        private AnalysisResult AddDrillDownOptions(AnalysisResult result)
        {
            foreach (AnalysisCategory category in Analysis.CategoryDictionary.Values)
            {
                if (category.Key == Settings.Category.Key)
                {
                    continue;
                }

                var categoryValue = Settings.ValueForCategory(category);
                if (categoryValue != null)
                {
                    continue;
                }

                result.AddDrilldownOption(new AnalysisDrilldownOption(Analysis, category));
                result.AddCategoryOption(category);
            }

            return result;
        }

        /// <summary>
        /// Add DrillUp Option
        /// </summary>
        /// <param name="result">
        /// <see cref="AnalysisResult"/> object
        /// </param>
        /// <returns>
        /// <see cref="AnalysisResult"/> updated object
        /// </returns>
        private AnalysisResult AddDrillUpOptions(AnalysisResult result)
        {
            var foundDrillup = false;
            if (Settings.Conditions != null)
            {
                foreach (AnalysisCategoryFilter filter in Settings.Conditions)
                {
                    if (!(filter is AnalysisCategoryFilter))
                    {
                        continue;
                    }

                    foundDrillup = true;
                    result.AddDrillupOption(new AnalysisDrillupOption(Analysis, filter));
                }
            }

            if (foundDrillup)
            {
                result.AddDrillupOption(new AnalysisDrillupOption(Analysis, Settings.Category, true));
            }

            return result;
        }
    }
}
