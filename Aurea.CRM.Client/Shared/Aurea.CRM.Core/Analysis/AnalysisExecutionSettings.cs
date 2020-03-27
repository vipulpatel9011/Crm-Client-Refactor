// <copyright file="AnalysisExecutionSettings.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.Analysis
{
    using System.Collections.Generic;
    using Configuration;
    using Drilldown;
    using Extensions;
    using Model;
    using Result;

    /// <summary>
    /// Implementation of analysis execution settings
    /// </summary>
    public class AnalysisExecutionSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExecutionSettings"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="options">Options</param>
        public AnalysisExecutionSettings(Analysis analysis, Dictionary<string, object> options)
        {
            this.Analysis = analysis;
            this.Category = this.Analysis.DefaultCategory;
            this.XCategory = this.Analysis.XCategory;
            object val = options.ValueOrDefault("ShowEmpty");
            if (val != null)
            {
                if (val.ToInt() > 0)
                {
                    this.ShowEmpty = true;
                }
                else if (val is string)
                {
                    this.ShowEmpty = ((string)val).ToLower() != "false";
                }
            }
            else
            {
                this.ShowEmpty = ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue("Analyses.ShowEmpty", false);
            }

            List<object> filterArray = null;
            foreach (AnalysisFilter filter in analysis.Filters)
            {
                if (!filter.HasFilterValues)
                {
                    continue;
                }

                if (filterArray == null)
                {
                    filterArray = new List<object> { filter };
                }
                else
                {
                    filterArray.Add(filter);
                }
            }

            this.Conditions = filterArray;
            List<AnalysisResultColumn> resultColumns = null;
            foreach (UPConfigAnalysisResultColumn configResultColumn in analysis.Configuration.ResultColumns)
            {
                AnalysisResultColumn resultColumn;
                if (!string.IsNullOrEmpty(configResultColumn.ValueName))
                {
                    resultColumn = analysis.ResultColumnWithKey(configResultColumn.ValueName);
                }
                else if (configResultColumn.AggregationType == "count" && configResultColumn.FieldId <= 0)
                {
                    resultColumn = analysis.ResultColumnWithTableNumberFieldId(configResultColumn.FieldTableId, -1);
                }
                else
                {
                    resultColumn = analysis.ResultColumnWithTableNumberFieldId(configResultColumn.FieldTableId, configResultColumn.FieldId);
                }

                if (resultColumn != null)
                {
                    resultColumn = resultColumn.ResultColumnWithAggregationType(configResultColumn.AggregationType);
                    if (resultColumns == null)
                    {
                        resultColumns = new List<AnalysisResultColumn> { resultColumn };
                    }
                    else
                    {
                        resultColumns.Add(resultColumn);
                    }
                }
            }

            this.ResultColumns = resultColumns;
            this.CurrencyCode = analysis.CurrencyConversion?.BaseCurrency?.CatalogCode ?? 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExecutionSettings"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="category">Category</param>
        /// <param name="xCategory">X Category</param>
        /// <param name="resultColumns">Result columns</param>
        /// <param name="conditions">Conditions</param>
        /// <param name="showEmpty">Show empty</param>
        /// <param name="currencyCode">Currency code</param>
        public AnalysisExecutionSettings(Analysis analysis, AnalysisCategory category, AnalysisCategory xCategory, List<AnalysisResultColumn> resultColumns, List<object> conditions, bool showEmpty, int currencyCode)
        {
            this.Category = category;
            this.XCategory = xCategory;
            this.ResultColumns = resultColumns;
            this.Conditions = conditions;
            this.Analysis = analysis;
            this.ShowEmpty = showEmpty;
            this.CurrencyCode = currencyCode;
        }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets category
        /// </summary>
        public AnalysisCategory Category { get; private set; }

        /// <summary>
        /// Gets conditions
        /// </summary>
        public List<object> Conditions { get; private set; }

        /// <summary>
        /// Gets currency code
        /// </summary>
        public int CurrencyCode { get; private set; }

        /// <summary>
        /// Gets result column
        /// </summary>
        public List<AnalysisResultColumn> ResultColumns { get; private set; }

        /// <summary>
        /// Gets a value indicating whether show empty
        /// </summary>
        public bool ShowEmpty { get; private set; }

        /// <summary>
        /// Gets x category
        /// </summary>
        public AnalysisCategory XCategory { get; private set; }

        /// <summary>
        /// Checks if equals
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>True if equal</returns>
        public bool IsEqual(AnalysisExecutionSettings obj)
        {
            if (!(obj is AnalysisExecutionSettings) || (this.Analysis != obj.Analysis) || (this.Category != obj.Category) 
                 || this.XCategory != obj.XCategory || this.ResultColumns.Count != obj.ResultColumns.Count || this.Conditions.Count != obj.Conditions.Count)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// LogSettings with category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Analysis execution settings</returns>
        public AnalysisExecutionSettings SettingsWithCategory(AnalysisCategory category)
        {
            return new AnalysisExecutionSettings(this.Analysis, category, this.XCategory, this.ResultColumns, this.Conditions, this.ShowEmpty, this.CurrencyCode);
        }

        /// <summary>
        /// LogSettings with drilldown option row
        /// </summary>
        /// <param name="option">Option</param>
        /// <param name="row">Row</param>
        /// <returns>Analysis execution settings</returns>
        public AnalysisExecutionSettings SettingsWithDrilldownOptionRow(AnalysisDrilldownOption option, AnalysisRow row)
        {
            AnalysisFilter filter = new AnalysisCategoryFilter(this.Category, row.Key);
            return this.SettingsWithAdditionalConditionCategory(filter, option.Category);
        }

        /// <summary>
        /// LogSettings with drillup option
        /// </summary>
        /// <param name="option">Option</param>
        /// <returns>Analysis execution settings</returns>
        public AnalysisExecutionSettings SettingsWithDrillupOption(AnalysisDrillupOption option)
        {
            AnalysisCategoryFilter removeFilter = null;
            AnalysisCategoryFilter lastFilter = null;
            foreach (AnalysisFilter filterCondition in this.Conditions)
            {
                if (!(filterCondition is AnalysisCategoryFilter))
                {
                    continue;
                }

                AnalysisCategoryFilter categoryFilter = (AnalysisCategoryFilter)filterCondition;
                if (categoryFilter.Category.Key == option.RemoveCategory.Key)
                {
                    removeFilter = categoryFilter;
                }
                else
                {
                    lastFilter = categoryFilter;
                }
            }

            List<object> condArray = new List<object>(this.Conditions);
            AnalysisCategory drillupCategory = null;
            if (removeFilter != null)
            {
                condArray.Remove(removeFilter);
                drillupCategory = this.Category;
            }
            else if (lastFilter != null && option.RemoveCategory.Key == this.Category.Key)
            {
                condArray.Remove(lastFilter);
                drillupCategory = lastFilter.Category;
            }

            return new AnalysisExecutionSettings(this.Analysis, drillupCategory, this.XCategory, this.ResultColumns, condArray, condArray.Count > 0 || this.Analysis.DefaultExecutionSettings.ShowEmpty, this.CurrencyCode);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Category={this.Category}, XCat={this.XCategory}, Columns={this.ResultColumns}, Filter={this.Conditions}";
        }

        /// <summary>
        /// Value for category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Value</returns>
        public string ValueForCategory(AnalysisCategory category)
        {
            if (this.Conditions != null)
            {
                foreach (AnalysisFilter condition in this.Conditions)
                {
                    string categoryValue = condition.ValueForCategory(category);
                    if (categoryValue != null)
                    {
                        return categoryValue;
                    }
                }
            }

            return null;
        }

        private AnalysisExecutionSettings SettingsWithAdditionalConditionCategory(AnalysisFilter filter, AnalysisCategory category)
        {
            List<object> conditions;
            if (filter != null)
            {
                List<object> cond = new List<object>();
                if (this.Conditions?.Count > 0)
                {
                    cond.AddRange(this.Conditions);
                }

                cond.Add(filter);
                conditions = cond;
            }
            else
            {
                conditions = this.Conditions;
            }

            if (category == null)
            {
                category = this.Category;
            }

            return new AnalysisExecutionSettings(this.Analysis, category, this.XCategory, this.ResultColumns, conditions, true, this.CurrencyCode);
        }
    }
}
