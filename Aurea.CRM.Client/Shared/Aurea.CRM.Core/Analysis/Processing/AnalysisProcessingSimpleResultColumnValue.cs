// <copyright file="AnalysisProcessingSimpleResultColumnValue.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Analysis.Model;
    using CRM;
    using Extensions;

    /// <summary>
    /// Implementation of simple result column value
    /// </summary>
    public class AnalysisProcessingSimpleResultColumnValue : AnalysisProcessingResultColumnValue
    {
        private double aggregatedValue;
        private Dictionary<string, object> appliedRecordIdentifications;
        private int count;
        private int queryTableIndex;
        private Dictionary<string, object> xResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingSimpleResultColumnValue"/> class.
        /// </summary>
        /// <param name="resultColumn">Result column</param>
        /// <param name="yCategory">Y category</param>
        /// <param name="processingResultColumn">Processing result column</param>
        public AnalysisProcessingSimpleResultColumnValue(AnalysisSourceFieldResultColumn resultColumn, AnalysisProcessingYCategoryValue yCategory, AnalysisProcessingResultColumn processingResultColumn)
            : base(processingResultColumn, yCategory)
        {
            this.ResultColumn = resultColumn;
            this.SourceField = resultColumn.AnalysisField;
            this.WeightField = resultColumn.WeightField;
            this.CurrencyField = resultColumn.CurrencyField;
            this.aggregatedValue = 0;
            this.AggregationType = resultColumn.AggregationType;
            this.appliedRecordIdentifications = new Dictionary<string, object>();
            this.queryTableIndex = resultColumn.AnalysisField.AnalysisTable.QueryTableIndex;
            this.count = 0;
        }

        /// <summary>
        /// Gets aggregation type
        /// </summary>
        public AnalysisAggregationType AggregationType { get; private set; }

        /// <inheritdoc/>
        public override int Count => this.count;

        /// <summary>
        /// Gets currency field
        /// </summary>
        public AnalysisSourceField CurrencyField { get; private set; }

        /// <summary>
        /// Gets result
        /// </summary>
        public override double Result
        {
            get
            {
                if (this.AggregationType.Avg && this.count > 0)
                {
                    return this.aggregatedValue / this.count;
                }

                return this.aggregatedValue;
            }
        }

        /// <summary>
        /// Gets result column
        /// </summary>
        public AnalysisSourceFieldResultColumn ResultColumn { get; private set; }

        /// <summary>
        /// Gets source field
        /// </summary>
        public AnalysisSourceField SourceField { get; private set; }

        /// <summary>
        /// Gets weight field
        /// </summary>
        public AnalysisSourceField WeightField { get; private set; }

        /// <inheritdoc/>
        public override bool ApplyRowXCategoryValueArraySumLine(ICrmDataSourceRow dataSourceRow, List<object> xCategoryValueArray, bool sumLine)
        {
            var v = dataSourceRow.RawValueAtIndex(this.SourceField.QueryResultFieldIndex);
            var recordIdentification = $"{this.YCategoryValue.Key}:{dataSourceRow.RecordIdentificationAtIndex(this.queryTableIndex)}";
            if (this.appliedRecordIdentifications.ValueOrDefault(recordIdentification) != null)
            {
                return false;
            }

            this.appliedRecordIdentifications[recordIdentification] = 1;
            double doubleValue = v.ToDouble();
            if (this.WeightField != null)
            {
                var weightValue = dataSourceRow.RawValueAtIndex(this.WeightField.QueryResultFieldIndex);
                doubleValue *= weightValue.ToDouble();
            }

            if (this.CurrencyField != null)
            {
                string currencyValue = dataSourceRow.RawValueAtIndex(this.CurrencyField.QueryResultFieldIndex);
                doubleValue = this.ProcessingResultColumn.ProcessingContext.AdjustValueForCurrency(doubleValue, currencyValue);
            }

            if (this.AggregationType.Min)
            {
                if (this.count == 0 || this.aggregatedValue > doubleValue)
                {
                    this.aggregatedValue = doubleValue;
                }
            }
            else if (this.AggregationType.Max)
            {
                if (this.aggregatedValue < doubleValue || this.count == 0)
                {
                    this.aggregatedValue = doubleValue;
                }
            }
            else
            {
                this.aggregatedValue += doubleValue;
            }

            this.count++;
            if (xCategoryValueArray != null)
            {
                foreach (AnalysisProcessingXCategoryValue xCategoryValue in xCategoryValueArray)
                {
                    AnalysisProcessingSimpleXResultColumnValue xResult = this.XResultColumnValueForCategoryValueKey(xCategoryValue.Key);
                    if (this.AggregationType.Min)
                    {
                        if (this.count == 0 || xResult.AggregatedValue > doubleValue)
                        {
                            xResult.AggregatedValue = doubleValue;
                        }
                    }
                    else if (this.AggregationType.Max)
                    {
                        if (xResult.AggregatedValue < doubleValue || this.count == 0)
                        {
                            xResult.AggregatedValue = doubleValue;
                        }
                    }
                    else
                    {
                        xResult.AggregatedValue += doubleValue;
                    }

                    xResult.Count++;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override int CountForXCategoryValueKey(string key)
        {
            return this.XResultColumnValueForCategoryValueKey(key).Count;
        }

        /// <summary>
        /// Display string from number
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns display string from number</returns>
        public override string DisplayStringFromNumber(double value)
        {
            return this.ResultColumn.DisplayStringFromNumber(value);
        }

        /// <inheritdoc/>
        public override double ResultForXCategoryValueKey(string key)
        {
            return this.XResultColumnValueForCategoryValueKey(key).AggregatedValue;
        }

        /// <summary>
        /// X result column value for category value key
        /// </summary>
        /// <param name="xCategoryValueKey">X category value key</param>
        /// <returns>Returns simple x result column value for category value key</returns>
        public AnalysisProcessingSimpleXResultColumnValue XResultColumnValueForCategoryValueKey(string xCategoryValueKey)
        {
            AnalysisProcessingSimpleXResultColumnValue resultValue;
            if (this.xResults == null)
            {
                resultValue = new AnalysisProcessingSimpleXResultColumnValue();
                this.xResults = new Dictionary<string, object> { { xCategoryValueKey, resultValue } };
            }
            else
            {
                resultValue = this.xResults.ValueOrDefault(xCategoryValueKey) as AnalysisProcessingSimpleXResultColumnValue;
                if (resultValue == null)
                {
                    resultValue = new AnalysisProcessingSimpleXResultColumnValue();
                    this.xResults.SetObjectForKey(resultValue, xCategoryValueKey);
                }
            }

            return resultValue;
        }
    }
}
