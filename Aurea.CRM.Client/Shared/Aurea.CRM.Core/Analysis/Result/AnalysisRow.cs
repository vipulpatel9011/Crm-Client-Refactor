// <copyright file="AnalysisRow.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Result
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Analysis.Drilldown;
    using Aurea.CRM.Core.Analysis.Processing;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implementation of analysis row class
    /// </summary>
    public class AnalysisRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisRow"/> class.
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="categoryValue">Category value</param>
        public AnalysisRow(AnalysisResult result, AnalysisProcessingYCategoryValue categoryValue)
        {
            var values = new List<object>();
            int i, count = result.Columns.Count;
            for (i = 0; i < count; i++)
            {
                AnalysisProcessingResultColumnValue rcv = categoryValue.ResultColumnValues[i];
                AnalysisResultCell cell = new AnalysisResultCell(rcv, this, result.Columns[i] as AnalysisColumn);
                values.Add(cell);
            }

            this.Key = categoryValue.Key;
            this.Values = values;
            this.Label = categoryValue.Label;
            this.ResultRows = categoryValue.ResultRows;
            this.Result = result;
            if (categoryValue.Category.SubCategoryName?.Length > 0)
            {
                AnalysisDrilldownOption opt = new AnalysisDrilldownOption(result.Analysis, categoryValue.Category.Category.BaseField(), categoryValue.Category.SubCategoryName);
                if (opt != null)
                {
                    this.AddDrilldownOption(opt);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisRow"/> class.
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="ycategoryValues">Y category values</param>
        public AnalysisRow(AnalysisResult result, List<AnalysisProcessingYCategoryValue> ycategoryValues)
        {
            List<object> resultRows = new List<object>();
            int i, count = result.Columns.Count;
            var resultColumnValues = new List<object>();
            for (i = 0; i < count; i++)
            {
                resultColumnValues.Add(new List<object>());
            }

            foreach (AnalysisProcessingYCategoryValue catval in ycategoryValues)
            {
                if (catval.ResultRows.Count > 0)
                {
                    resultRows.AddRange(catval.ResultRows);
                }

                for (i = 0; i < count; i++)
                {
                    var va = resultColumnValues[i] as List<object>;
                    va.Add(catval.ResultColumnValues[i]);
                }
            }

            var values = new List<object>();
            for (i = 0; i < count; i++)
            {
                AnalysisResultCell cell = new AnalysisResultCell(resultColumnValues[i] as AnalysisProcessingResultColumnValue, this, result.Columns[i] as AnalysisColumn);
                values.Add(cell);
            }

            this.Values = values;
            this.ResultRows = resultRows;
            this.Result = result;
            this.Label = LocalizedString.TextAnalysesOther;
        }

        /// <summary>
        /// Gets drilldown options
        /// </summary>
        public List<object> DrilldownOptions
        {
            get
            {
                if (this.RowSpecificDrilldownOptions == null || this.RowSpecificDrilldownOptions?.Count == 0)
                {
                    return this.Result.DrilldownOptions;
                }

                if (this.Result.DrilldownOptions == null || this.Result.DrilldownOptions?.Count == 0)
                {
                    return this.RowSpecificDrilldownOptions;
                }

                List<object> arr = new List<object>(this.RowSpecificDrilldownOptions);
                arr.AddRange(this.Result.DrilldownOptions);
                return arr;
            }
        }

        /// <summary>
        /// Gets a value indicating whether has details
        /// </summary>
        /// <returns>Returns has details</returns>
        public bool HasDetails => this.Result.HasDetailsFields && this.ResultRows.Count > 0;

        /// <summary>
        /// Gets key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets label
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Gets position in result
        /// </summary>
        public int PositionInResult { get; private set; }

        /// <summary>
        /// Gets result
        /// </summary>
        public AnalysisResult Result { get; private set; }

        /// <summary>
        /// Gets result rows
        /// </summary>
        public List<object> ResultRows { get; private set; }

        /// <summary>
        /// Gets row details
        /// </summary>
        public List<AnalysisRowDetails> RowDetails
        {
            get
            {
                var arr = new List<AnalysisRowDetails>();
                foreach (ICrmDataSourceRow row in this.ResultRows)
                {
                    arr.Add(new AnalysisRowDetails(this, row));
                }

                return arr;
            }
        }

        /// <summary>
        /// Gets row specific drilldown options
        /// </summary>
        public List<object> RowSpecificDrilldownOptions { get; private set; }

        /// <summary>
        /// Gets values
        /// </summary>
        public List<object> Values { get; private set; }

        /// <summary>
        /// String values for result row
        /// </summary>
        /// <param name="dataSourceRow">Data source row</param>
        /// <returns>List of string values for result row</returns>
        public List<string> StringValuesForResultRow(ICrmDataSourceRow dataSourceRow)
        {
            List<object> sourceFields = this.Result.DetailsFields;
            int sourceFieldCount = sourceFields.Count;
            List<string> stringArray = null;
            if (sourceFieldCount > 0)
            {
                stringArray = new List<string>();
                foreach (object field in sourceFields)
                {
                    if (field == null)
                    {
                        stringArray.Add(string.Empty);
                    }
                    else
                    {
                        AnalysisSourceField sourceField = (AnalysisSourceField)field;
                        stringArray.Add(dataSourceRow.ValueAtIndex(sourceField.QueryResultFieldIndex));
                    }
                }
            }

            return stringArray;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Key}: {this.Values}";
        }

        /// <summary>
        /// Value.AnalysisValueFunction at column index
        /// </summary>
        /// <param name="columnIndex">Column index</param>
        /// <returns>Returns value at column index</returns>
        public AnalysisResultCell ValueAtColumnIndex(int columnIndex)
        {
            return this.Values?[columnIndex] as AnalysisResultCell;
        }

        private void AddDrilldownOption(AnalysisDrilldownOption drillDownOption)
        {
            if (this.RowSpecificDrilldownOptions == null)
            {
                this.RowSpecificDrilldownOptions = new List<object> { drillDownOption };
            }
            else
            {
                this.RowSpecificDrilldownOptions.Add(drillDownOption);
            }
        }
    }
}
