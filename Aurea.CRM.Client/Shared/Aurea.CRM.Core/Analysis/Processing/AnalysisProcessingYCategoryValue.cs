// <copyright file="AnalysisProcessingYCategoryValue.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM;
    using Configuration;

    /// <summary>
    /// Implementation of y category value
    /// </summary>
    public class AnalysisProcessingYCategoryValue : AnalysisProcessingCategoryValue
    {
        private List<AnalysisProcessingResultColumnValue> resultColumns;
        private List<object> resultRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingYCategoryValue"/> class.
        /// </summary>
        /// <param name="processing">Processing</param>
        /// <param name="category">Category</param>
        public AnalysisProcessingYCategoryValue(AnalysisProcessing processing, AnalysisCategoryValue category)
            : base(processing, category)
        {
            this.resultColumns = new List<AnalysisProcessingResultColumnValue>();
            foreach (AnalysisProcessingResultColumn resultColumn in this.Processing.ResultColumns)
            {
                var processingResultColumnValue = AnalysisProcessingResultColumnValue.ColumnValueForAPResultColumnYCategory(resultColumn, this);
                if (processingResultColumnValue != null)
                {
                    this.resultColumns.Add(processingResultColumnValue);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingYCategoryValue"/> class.
        /// </summary>
        /// <param name="processing">Analysis processing</param>
        public AnalysisProcessingYCategoryValue(AnalysisProcessing processing)
            : this(processing, new AnalysisCategoryValue(processing.Analysis.DefaultCategory, "sum", LocalizedString.TextAnalysesSum))
        {
                this.IsSumLine = true;
        }

        /// <summary>
        /// Gets result column values
        /// </summary>
        public List<AnalysisProcessingResultColumnValue> ResultColumnValues => this.resultColumns;

        /// <summary>
        /// Gets result row
        /// </summary>
        public List<object> ResultRows => this.resultRows;

        /// <summary>
        /// Gets values
        /// </summary>
        public List<object> Values { get; private set; }

        /// <summary>
        /// Add row x category
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="xCategoryValue">X category value</param>
        /// <returns>Returns add row x category</returns>
        public bool AddRowXCategory(ICrmDataSourceRow row, AnalysisProcessingXCategoryValue xCategoryValue)
        {
            bool complete = true;
            foreach (AnalysisProcessingResultColumnValue processingResultColumnValue in this.resultColumns)
            {
                if (!processingResultColumnValue.ApplyRowXCategoryValueSumLine(row, xCategoryValue, this.IsSumLine))
                {
                    complete = false;
                }
            }

            if (this.resultRows == null)
            {
                this.resultRows = new List<object> { row };
            }
            else
            {
                this.resultRows.Add(row);
            }

            return complete;
        }

        /// <summary>
        /// Add row x category value array
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="xCategoryValues">X category values</param>
        /// <returns>Return add row x category value array</returns>
        public bool AddRowXCategoryValueArray(ICrmDataSourceRow row, List<object> xCategoryValues)
        {
            bool complete = true;
            foreach (AnalysisProcessingResultColumnValue processingResultColumnValue in this.resultColumns)
            {
                if (!processingResultColumnValue.ApplyRowXCategoryValueArraySumLine(row, xCategoryValues, this.IsSumLine))
                {
                    complete = false;
                }
            }

            if (this.resultRows == null)
            {
                this.resultRows = new List<object> { row };
            }
            else
            {
                this.resultRows.Add(row);
            }

            return complete;
        }

        /// <summary>
        /// Execute computation step
        /// </summary>
        /// <returns>Returns boolean value</returns>
        public bool ExecuteComputationStep()
        {
            bool complete = true;
            foreach (AnalysisProcessingResultColumnValue resultColumnValue in this.resultColumns)
            {
                if (!resultColumnValue.Complete)
                {
                    if (!resultColumnValue.ExecuteComputationStep())
                    {
                        complete = false;
                    }
                }
            }

            if (!complete)
            {
                this.ExecuteComputationStep();
            }

            return complete;
        }

        /// <summary>
        /// Number result at index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Returns number result at index</returns>
        public double NumberResultAtIndex(int index)
        {
            var v = this.resultColumns[index];
            return v.Result;
        }
    }
}
