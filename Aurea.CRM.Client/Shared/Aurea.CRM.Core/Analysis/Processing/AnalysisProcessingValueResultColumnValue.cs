// <copyright file="AnalysisProcessingValueResultColumnValue.cs" company="Aurea Software Gmbh">
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
    using CRM;
    using Extensions;
    using Model;
    using Value;
    using Value.AnalysisProcessing;
    using Value.AnalysisValueFunction;

    /// <summary>
    /// Implementation of value result column value
    /// </summary>
    public class AnalysisProcessingValueResultColumnValue : AnalysisProcessingResultColumnValue
    {
        private bool complete;
        private List<object> objectValues;
        private Dictionary<string, object> significantRows;
        private AnalysisProcessingValueAggregator valueAggregator;
        private List<object> xObjectValues;
        private Dictionary<string, object> xResults;
        private AnalysisProcessingValueExecutionContext executionContext;
        private AnalysisProcessingValueExecutionContext sumExecutionContext;
        private AnalysisProcessingValueExecutionContext sumXExecutionContext;
        private AnalysisProcessingValueExecutionContext xExecutionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingValueResultColumnValue"/> class.
        /// </summary>
        /// <param name="resultColumn">Result column</param>
        /// <param name="yCategoryValue">Y category value</param>
        /// <param name="processingResultColumn">Processing result column</param>
        public AnalysisProcessingValueResultColumnValue(AnalysisValueResultColumn resultColumn, AnalysisProcessingYCategoryValue yCategoryValue, AnalysisProcessingResultColumn processingResultColumn)
            : base(processingResultColumn, yCategoryValue)
        {
            this.ResultColumn = resultColumn;
            this.ValueFunction = this.ResultColumn.ValueFunction;
            this.AggregationType = this.ResultColumn.AggregationType;
            if (this.ResultColumn.IsStatic)
            {
                this.valueAggregator = new AnalysisProcessingStaticValueAggregator();
            }
            else if (this.ResultColumn.ValueOptions.IsText)
            {
                this.valueAggregator = new AnalysisProcessingStringValueAggregator(this.ResultColumn.ValueOptions);
            }
            else if (this.AggregationType != null)
            {
                this.valueAggregator = this.AggregationType.CreateValueAggregator();
            }
            else
            {
                this.valueAggregator = new AnalysisProcessingSumValueAggregator();
            }

            this.SignificantQueryResultTableIndices = this.ResultColumn.SignificantQueryResultTableIndices;
            if (this.SignificantQueryResultTableIndices.Count == 0)
            {
                this.SignificantQueryResultTableIndices = new List<object> { 0 };
            }

            this.complete = true;
        }

        /// <summary>
        /// Gets aggregation type
        /// </summary>
        public AnalysisAggregationType AggregationType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this result is complete
        /// </summary>
        public override bool Complete => this.complete;

        /// <summary>
        /// Gets count
        /// </summary>
        public override int Count => this.valueAggregator.Count;

        /// <summary>
        /// Gets execution context
        /// </summary>
        public AnalysisProcessingValueExecutionContext ExecutionContext
            => this.executionContext ?? (this.executionContext = this.ProcessingResultColumn.ExecutionContext);

        /// <summary>
        /// Gets result
        /// </summary>
        public override double Result => this.valueAggregator.DoubleValue;

        /// <summary>
        /// Gets result column
        /// </summary>
        public AnalysisValueResultColumn ResultColumn { get; private set; }

        /// <summary>
        /// Gets significant query result table indices
        /// </summary>
        public List<object> SignificantQueryResultTableIndices { get; private set; }

        /// <summary>
        /// Gets sum execution context
        /// </summary>
        public AnalysisProcessingValueExecutionContext SumExecutionContext
            => this.sumExecutionContext ?? (this.sumExecutionContext = this.ProcessingResultColumn.SumExecutionContext);

        /// <summary>
        /// Gets sum x execution context
        /// </summary>
        public AnalysisProcessingValueExecutionContext SumXExecutionContext
            => this.sumXExecutionContext ?? (this.sumXExecutionContext = this.ProcessingResultColumn.SumXExecutionContext);

        /// <summary>
        /// Gets text result
        /// </summary>
        public override string TextResult => this.valueAggregator.StringValue;

        /// <summary>
        /// Gets value function
        /// </summary>
        public AnalysisValueFunction ValueFunction { get; private set; }

        /// <summary>
        /// Gets x execution context
        /// </summary>
        public AnalysisProcessingValueExecutionContext XExecutionContext
            => this.xExecutionContext ?? (this.xExecutionContext = this.ProcessingResultColumn.XExecutionContext);

        /// <summary>
        /// Applies row x category value
        /// </summary>
        /// <param name="dataSourceRow">Data source row</param>
        /// <param name="xCategoryValueArray">X category value array</param>
        /// <param name="sumLine">Sum line</param>
        /// <returns>Boolean value for apply result</returns>
        public override bool ApplyRowXCategoryValueArraySumLine(ICrmDataSourceRow dataSourceRow, List<object> xCategoryValueArray, bool sumLine)
        {
            string significantRowIdentifierForRow = $"{this.YCategoryValue.Key}_{this.SignificantRowIdentifierForRow(dataSourceRow)}";
            if (this.significantRows == null)
            {
                this.significantRows = new Dictionary<string, object> { { significantRowIdentifierForRow, 1 } };
            }
            else if (this.significantRows.ValueOrDefault(significantRowIdentifierForRow) != null)
            {
                return false;
            }
            else
            {
                this.significantRows.SetObjectForKey(1, significantRowIdentifierForRow);
            }

            if (this.ValueFunction.ReturnsNumber)
            {
                double r = this.ValueFunction.NumberResultForResultRow(dataSourceRow);
                this.AddDoubleValue(r);
                foreach (AnalysisProcessingXCategoryValue xCategoryValue in xCategoryValueArray)
                {
                    this.AddDoubleValueXCategoryKey(r, xCategoryValue.Key);
                }
            }
            else if (this.ValueFunction.ReturnsObject)
            {
                AnalysisProcessingQueryResultRowExecutionContext rowExecutionContext = new AnalysisProcessingQueryResultRowExecutionContext(dataSourceRow, this.YCategoryValue, null, this.ProcessingResultColumn.ProcessingContext);
                AnalysisValueIntermediateResult objectResult = this.ValueFunction.ResultForQueryRowVariableContext(rowExecutionContext, sumLine ? this.SumExecutionContext : this.ExecutionContext);
                if (objectResult != null)
                {
                    if (objectResult.Complete)
                    {
                        if (this.ResultColumn.ValueOptions.IsText)
                        {
                            this.AddStringValue(objectResult.TextResult);
                        }
                        else
                        {
                            this.AddDoubleValue(objectResult.NumberResult);
                        }
                    }
                    else
                    {
                        if (this.objectValues == null)
                        {
                            this.objectValues = new List<object> { objectResult };
                        }
                        else
                        {
                            this.objectValues.Add(objectResult);
                        }

                        this.complete = false;
                    }
                }

                foreach (AnalysisProcessingXCategoryValue xCategoryValue in xCategoryValueArray)
                {
                    rowExecutionContext = new AnalysisProcessingQueryResultRowExecutionContext(dataSourceRow, this.YCategoryValue, xCategoryValue, this.ProcessingResultColumn.ProcessingContext);
                    AnalysisValueIntermediateResult xObjectResult = this.ValueFunction.ResultForQueryRowVariableContext(rowExecutionContext, sumLine ? this.SumXExecutionContext : this.XExecutionContext);
                    if (xObjectResult != null)
                    {
                        if (xObjectResult.Complete)
                        {
                            if (this.ResultColumn.ValueOptions.IsText)
                            {
                                this.AddStringValueXCategoryKey(xObjectResult.TextResult, xCategoryValue.Key);
                            }
                            else
                            {
                                this.AddDoubleValueXCategoryKey(xObjectResult.NumberResult, xCategoryValue.Key);
                            }
                        }
                        else
                        {
                            if (this.xObjectValues == null)
                            {
                                this.xObjectValues = new List<object> { xObjectResult };
                            }
                            else
                            {
                                this.xObjectValues.Add(xObjectResult);
                            }

                            this.complete = false;
                        }

                        return objectResult.Complete && xObjectResult.Complete;
                    }
                }

                return objectResult.Complete;
            }
            else if (this.ValueFunction.ReturnsText)
            {
                string stringValue = this.ValueFunction.TextResultForResultRow(dataSourceRow);
                this.AddStringValue(stringValue);
                foreach (AnalysisProcessingXCategoryValue xCategoryValue in xCategoryValueArray)
                {
                    this.AddStringValueXCategoryKey(stringValue, xCategoryValue.Key);
                }
            }

            return true;
        }

        /// <summary>
        /// Count for x category value key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns count result for x category value key</returns>
        public override int CountForXCategoryValueKey(string key)
        {
            AnalysisProcessingValueAggregator xValueAggregator = this.xResults.ValueOrDefault(key) as AnalysisProcessingValueAggregator;
            return xValueAggregator.Count;
        }

        /// <summary>
        /// Executes computation step
        /// </summary>
        /// <returns>Boolean value for execute computation step</returns>
        public override bool ExecuteComputationStep()
        {
            this.complete = true;
            int i, objectCount = this.objectValues.Count, xObjectCount = this.xObjectValues.Count;
            for (i = 0; i < objectCount; i++)
            {
                AnalysisValueIntermediateResult result = this.objectValues[i] as AnalysisValueIntermediateResult;
                if (!result.Complete)
                {
                    result = result.ExecuteStep();
                    this.objectValues[i] = result;
                    if (!result.Complete)
                    {
                        this.complete = false;
                    }
                }
            }

            for (i = 0; i < xObjectCount; i++)
            {
                AnalysisValueIntermediateResult result = this.xObjectValues[i] as AnalysisValueIntermediateResult;
                if (!result.Complete)
                {
                    result = result.ExecuteStep();
                    this.xObjectValues[i] = result;
                    if (!result.Complete)
                    {
                        this.complete = false;
                    }
                }
            }

            if (this.complete)
            {
                AnalysisValueOptions valueOptions = this.ProcessingResultColumn.ResultColumn.ValueOptions;
                if (objectCount > 0)
                {
                    if (valueOptions.IsStatic && this.objectValues.Count > 1)
                    {
                        this.objectValues = new List<object> { this.objectValues[0] };
                    }

                    if (valueOptions.IsText)
                    {
                        foreach (AnalysisValueIntermediateResult result in this.objectValues)
                        {
                            this.AddStringValue(result.TextResult);
                        }
                    }
                    else
                    {
                        foreach (AnalysisValueIntermediateResult result in this.objectValues)
                        {
                            this.AddDoubleValue(result.NumberResult);
                        }
                    }
                }

                if (xObjectCount > 0)
                {
                    if (valueOptions.IsStatic && this.xObjectValues.Count > 1)
                    {
                        var resultPerXCategory = new Dictionary<string, object>();
                        foreach (AnalysisValueIntermediateResult res in this.xObjectValues)
                        {
                            if (res.XCategoryKey == null)
                            {
                                continue;
                            }

                            resultPerXCategory.SetObjectForKey(res, res.XCategoryKey);
                        }

                        this.xObjectValues = new List<object>(resultPerXCategory.Values);
                    }

                    if (valueOptions.IsText)
                    {
                        foreach (AnalysisValueIntermediateResult result in this.xObjectValues)
                        {
                            this.AddStringValueXCategoryKey(result.TextResult, result.XCategoryKey);
                        }
                    }
                    else
                    {
                        foreach (AnalysisValueIntermediateResult result in this.xObjectValues)
                        {
                            this.AddDoubleValueXCategoryKey(result.NumberResult, result.XCategoryKey);
                        }
                    }
                }
            }

            return this.complete;
        }

        /// <summary>
        /// Result for x category value key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns result for x category value key</returns>
        public override double ResultForXCategoryValueKey(string key)
        {
            AnalysisProcessingValueAggregator xValueAggregator = this.xResults.ValueOrDefault(key) as AnalysisProcessingValueAggregator;
            return xValueAggregator.DoubleValue;
        }

        /// <summary>
        /// Text result for x category value key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns text result for x category value key</returns>
        public override string TextResultForXCategoryValueKey(string key)
        {
            AnalysisProcessingValueAggregator xValueAggregator = this.xResults.ValueOrDefault(key) as AnalysisProcessingValueAggregator;
            return xValueAggregator.StringValue;
        }

        private void AddDoubleValue(double doubleValue)
        {
            this.valueAggregator.AddDoubleValue(doubleValue);
        }

        private void AddDoubleValueXCategoryKey(double doubleValue, string xCategoryKey)
        {
            if (string.IsNullOrEmpty(xCategoryKey))
            {
                AnalysisProcessingValueAggregator xAggregator = this.ValueAggregatorForXCategoryKey(xCategoryKey);
                xAggregator.AddDoubleValue(doubleValue);
            }
        }

        private void AddStringValue(string stringValue)
        {
            this.valueAggregator.AddStringValue(stringValue);
        }

        private void AddStringValueXCategoryKey(string stringValue, string xCategoryKey)
        {
            if (string.IsNullOrEmpty(xCategoryKey))
            {
                AnalysisProcessingValueAggregator xAggregator = this.ValueAggregatorForXCategoryKey(xCategoryKey);
                xAggregator.AddStringValue(stringValue);
            }
        }

        private string DisplayStringFromNumber(int value)
        {
            return this.ResultColumn.DisplayStringFromNumber(value);
        }

        private string SignificantRowIdentifierForRow(ICrmDataSourceRow dataSourceRow)
        {
            string sig = null;
            var builder = new System.Text.StringBuilder();
            builder.Append(sig);
            foreach (int? num in this.SignificantQueryResultTableIndices)
            {
                var rid = dataSourceRow.RecordIdentificationAtIndex(num.Value);
                if (sig == null)
                {
                    sig = rid ?? "empty_";
                }
                else
                {
                    builder.Append($"_{rid}");
                }
            }

            sig = builder.ToString();
            return sig;
        }

        private AnalysisProcessingValueAggregator ValueAggregatorForXCategoryKey(string xCategoryKey)
        {
            var aggregator = this.xResults.ValueOrDefault(xCategoryKey) as AnalysisProcessingValueAggregator;
            if (aggregator == null)
            {
                aggregator = this.valueAggregator.CreateInstance();
                if (this.xResults == null)
                {
                    this.xResults = new Dictionary<string, object> { { xCategoryKey, aggregator } };
                }
                else
                {
                    this.xResults.SetObjectForKey(aggregator, xCategoryKey);
                }
            }

            return aggregator;
        }
    }
}
