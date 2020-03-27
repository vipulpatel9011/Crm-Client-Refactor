// <copyright file="AnalysisProcessingTableResultColumnValue.cs" company="Aurea Software Gmbh">
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
    /// Implementation of table result column value
    /// </summary>
    public class AnalysisProcessingTableResultColumnValue : AnalysisProcessingResultColumnValue
    {
        private int count;
        private int queryTableIndex;
        private Dictionary<string, object> xResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingTableResultColumnValue"/> class.
        /// </summary>
        /// <param name="resultColumn">Result column</param>
        /// <param name="yCategory">Y category</param>
        /// <param name="processingResultColumn">Processing result column</param>
        public AnalysisProcessingTableResultColumnValue(AnalysisTableResultColumn resultColumn, AnalysisProcessingYCategoryValue yCategory, AnalysisProcessingResultColumn processingResultColumn)
            : base(processingResultColumn, yCategory)
        {
            this.ResultColumn = resultColumn;
            this.count = 0;
            this.queryTableIndex = this.ResultColumn.AnalysisTable.QueryTableIndex;
        }

        /// <summary>
        /// Gets result column
        /// </summary>
        public AnalysisTableResultColumn ResultColumn { get; private set; }

        /// <summary>
        /// Gets count
        /// </summary>
        public override int Count => this.count;

        /// <summary>
        /// Gets result
        /// </summary>
        public override double Result => this.count;

        /// <inheritdoc/>
        public override bool ApplyRowXCategoryValueArraySumLine(ICrmDataSourceRow dataSourceRow, List<object> xCategoryValueArray, bool sumLine)
        {
            string rid = $"{this.YCategoryValue.Key}:{dataSourceRow.RecordIdentificationAtIndex(this.queryTableIndex)}";
            if (string.IsNullOrEmpty(rid))
            {
                return false;
            }

            if (this.OccurrenceCountForKey(rid) == 1)
            {
                ++this.count;
            }

            foreach (AnalysisProcessingXCategoryValue xCategoryValue in xCategoryValueArray)
            {
                if (this.OccurrenceCountForKey($"{xCategoryValue.Key}:{rid}") == 1)
                {
                    AnalysisProcessingTableXResultColumnValue num = this.XResultColumnValueForCategoryValueKey(xCategoryValue.Key);
                    num.Count++;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override int CountForXCategoryValueKey(string key)
        {
            return this.XResultColumnValueForCategoryValueKey(key).Count;
        }

        /// <inheritdoc/>
        public override string DisplayStringFromNumber(double value)
        {
            return this.ResultColumn.DisplayStringFromNumber(value);
        }

        /// <inheritdoc/>
        public override double ResultForXCategoryValueKey(string key)
        {
            return this.XResultColumnValueForCategoryValueKey(key).Count;
        }

        /// <summary>
        /// X result column value for category value key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns x result column value for category value key</returns>
        public AnalysisProcessingTableXResultColumnValue XResultColumnValueForCategoryValueKey(string key)
        {
            AnalysisProcessingTableXResultColumnValue resultValue;
            if (this.xResults == null)
            {
                resultValue = new AnalysisProcessingTableXResultColumnValue();
                this.xResults = new Dictionary<string, object> { { key, resultValue } };
            }
            else
            {
                resultValue = this.xResults.ValueOrDefault(key) as AnalysisProcessingTableXResultColumnValue;
                if (resultValue == null)
                {
                    resultValue = new AnalysisProcessingTableXResultColumnValue();
                    this.xResults.SetObjectForKey(resultValue, key);
                }
            }

            return resultValue;
        }
    }
}
