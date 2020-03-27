// <copyright file="AnalysisProcessingResultColumnValue.cs" company="Aurea Software Gmbh">
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
    /// Implementation of result column value
    /// </summary>
    public class AnalysisProcessingResultColumnValue
    {
        private Dictionary<string, object> occurrenceCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingResultColumnValue"/> class.
        /// </summary>
        /// <param name="processingResultColumn">Processing result column</param>
        /// <param name="yCategory">Y category</param>
        public AnalysisProcessingResultColumnValue(AnalysisProcessingResultColumn processingResultColumn, AnalysisProcessingYCategoryValue yCategory)
        {
            this.ProcessingResultColumn = processingResultColumn;
            this.YCategoryValue = yCategory;
        }

        /// <summary>
        /// Gets a value indicating whether this is complete
        /// </summary>
        public virtual bool Complete => true;

        /// <summary>
        /// Gets count
        /// </summary>
        public virtual int Count => 0;

        /// <summary>
        /// Gets processing result column
        /// </summary>
        public AnalysisProcessingResultColumn ProcessingResultColumn { get; private set; }

        /// <summary>
        /// Gets result
        /// </summary>
        public virtual double Result => 0;

        /// <summary>
        /// Gets text result
        /// </summary>
        public virtual string TextResult => string.Empty;

        /// <summary>
        /// Gets y category value
        /// </summary>
        public AnalysisProcessingYCategoryValue YCategoryValue { get; private set; }

        /// <summary>
        /// Gets column value for analysis processing result column
        /// </summary>
        /// <param name="processingResultColumn">Processing result column</param>
        /// <param name="yCategory">Y category</param>
        /// <returns>Returns result column value</returns>
        public static AnalysisProcessingResultColumnValue ColumnValueForAPResultColumnYCategory(AnalysisProcessingResultColumn processingResultColumn, AnalysisProcessingYCategoryValue yCategory)
        {
            AnalysisResultColumn resultColumn = processingResultColumn.ResultColumn;
            if (resultColumn is AnalysisSourceFieldResultColumn)
            {
                return new AnalysisProcessingSimpleResultColumnValue((AnalysisSourceFieldResultColumn)resultColumn, yCategory, processingResultColumn);
            }

            if (resultColumn is AnalysisTableResultColumn)
            {
                return new AnalysisProcessingTableResultColumnValue((AnalysisTableResultColumn)resultColumn, yCategory, processingResultColumn);
            }

            if (resultColumn is AnalysisValueResultColumn)
            {
                return new AnalysisProcessingValueResultColumnValue((AnalysisValueResultColumn)resultColumn, yCategory, processingResultColumn);
            }

            return null;
        }

        /// <summary>
        /// Applies row x category value array
        /// </summary>
        /// <param name="dataSourceRow">Data source row</param>
        /// <param name="xCategoryValueArray">X category value array</param>
        /// <param name="sumLine">Sum line</param>
        /// <returns>Boolean value for result</returns>
        public virtual bool ApplyRowXCategoryValueArraySumLine(ICrmDataSourceRow dataSourceRow, List<object> xCategoryValueArray, bool sumLine)
        {
            return true;
        }

        /// <summary>
        /// Applies row x category value array
        /// </summary>
        /// <param name="dataSourceRow">Data source row</param>
        /// <param name="xCategoryValue">X Category value array</param>
        /// <param name="sumLine">Sum line</param>
        /// <returns>Boolean value for result</returns>
        public bool ApplyRowXCategoryValueSumLine(ICrmDataSourceRow dataSourceRow, AnalysisProcessingXCategoryValue xCategoryValue, bool sumLine)
        {
            return this.ApplyRowXCategoryValueArraySumLine(dataSourceRow, xCategoryValue != null ? new List<object> { xCategoryValue } : null, sumLine);
        }

        /// <summary>
        /// Count for x category value key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns count for x category value key</returns>
        public virtual int CountForXCategoryValueKey(string key)
        {
            return 0;
        }

        /// <summary>
        /// Gets display string from number
        /// </summary>
        /// <param name="value">Value.AnalysisValueFunction</param>
        /// <returns>Returns display string from number</returns>
        public virtual string DisplayStringFromNumber(double value)
        {
            return value.ToString("0.00");
        }

        /// <summary>
        /// Executes computation step
        /// </summary>
        /// <returns>Boolean for result</returns>
        public virtual bool ExecuteComputationStep()
        {
            return true;
        }

        /// <summary>
        /// Calculates occurrence count for key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns count for occurrence</returns>
        public virtual int OccurrenceCountForKey(string key)
        {
            var occ = this.occurrenceCount.ValueOrDefault(key) as int?;
            if (occ != null)
            {
                int count = occ.Value + 1;
                this.occurrenceCount.SetObjectForKey(new int?(count), key);
                return count;
            }

            if (this.occurrenceCount == null)
            {
                this.occurrenceCount = new Dictionary<string, object> { { key, 1 } };
            }
            else
            {
                this.occurrenceCount.SetObjectForKey(new int?(1), key);
            }

            return 1;
        }

        /// <summary>
        /// Result for x category value key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns result for x category value key</returns>
        public virtual double ResultForXCategoryValueKey(string key)
        {
            return 0;
        }

        /// <summary>
        /// Text result for x category value key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns text result for x category value key</returns>
        public virtual string TextResultForXCategoryValueKey(string key)
        {
            return string.Empty;
        }
    }
}
