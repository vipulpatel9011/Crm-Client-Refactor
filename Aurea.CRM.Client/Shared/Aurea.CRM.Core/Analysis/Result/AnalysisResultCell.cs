// <copyright file="AnalysisResultCell.cs" company="Aurea Software Gmbh">
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
    using System.Linq;
    using Aurea.CRM.Core.Analysis.Model;
    using Aurea.CRM.Core.Analysis.Processing;

    /// <summary>
    /// Implementation of analysis result cell
    /// </summary>
    public class AnalysisResultCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisResultCell"/> class.
        /// </summary>
        /// <param name="value">Value.AnalysisValueFunction</param>
        /// <param name="row">Row</param>
        /// <param name="column">Column</param>
        public AnalysisResultCell(AnalysisProcessingResultColumnValue value, AnalysisRow row, AnalysisColumn column)
        {
            this.Row = row;
            this.Column = column;
            if (this.Column.ResultColumn.ValueOptions?.IsText ?? false)
            {
                this.StringValue = value.TextResult;
                this.RawValue = this.StringValue;
                this.Value = 0;
                var xCategoryValues = column.XCategoryValues;
                if (xCategoryValues.Count > 0)
                {
                    var xResultCells = new List<AnalysisXResultCell>();
                    foreach (AnalysisXColumn xCategoryValue in xCategoryValues)
                    {
                        xResultCells.Add(new AnalysisXResultCell(value.TextResultForXCategoryValueKey(xCategoryValue.Key)));
                    }

                    this.XResultCells = xResultCells;
                }
            }
            else
            {
                this.Value = value.Result;
                this.StringValue = column.ResultColumn.DisplayStringFromNumber(this.Value);
                this.RawValue = this.Value.ToString();
                var xCategoryValues = column.XCategoryValues;
                if (xCategoryValues?.Count > 0)
                {
                    var xResultCells = new List<AnalysisXResultCell>();
                    foreach (AnalysisXColumn xCategoryValue in xCategoryValues)
                    {
                        var num = value.ResultForXCategoryValueKey(xCategoryValue.Key);
                        xResultCells.Add(new AnalysisXResultCell(num, column.ResultColumn.DisplayStringFromNumber(num)));
                    }

                    this.XResultCells = xResultCells;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisResultCell"/> class.
        /// </summary>
        /// <param name="values">Values</param>
        /// <param name="row">Row</param>
        /// <param name="column">Column</param>
        public AnalysisResultCell(List<object> values, AnalysisRow row, AnalysisColumn column)
        {
            Row = row;
            Column = column;
            var result = 0.0;
            if (column.IsTextColumn || values?.Any() == false)
            {
                Value = 0;
                StringValue = string.Empty;
            }

            var collection = values?.OfType<AnalysisProcessingResultColumnValue>();
            var lstAnalysis = collection?.ToList();
            var aggregationType = column.ResultColumn.AggregationType;
            if (lstAnalysis?.Any() == true)
            {
                result = GetAggregatedResult(aggregationType, lstAnalysis);
            }

            Value = result;
            StringValue = column.ResultColumn.DisplayStringFromNumber(Value);
            var xCategoryValues = column.XCategoryValues;
            if (xCategoryValues?.Any() == true)
            {
                XResultCells = GetAnalysisXResultCells(lstAnalysis, aggregationType, column);
            }
        }

        /// <summary>
        /// Gets column
        /// </summary>
        public AnalysisColumn Column { get; private set; }

        /// <summary>
        /// Gets raw value
        /// </summary>
        public string RawValue { get; private set; }

        /// <summary>
        /// Gets row
        /// </summary>
        public AnalysisRow Row { get; private set; }

        /// <summary>
        /// Gets string value
        /// </summary>
        public string StringValue { get; private set; }

        /// <summary>
        /// Gets value
        /// </summary>
        public double Value { get; private set; }

        /// <summary>
        /// Gets x result cells
        /// </summary>
        public List<AnalysisXResultCell> XResultCells { get; private set; }

        /// <summary>
        /// Raw string x resutl at index
        /// </summary>
        /// <param name="xIndex">X index</param>
        /// <returns>Returns raw string result at index</returns>
        public string RawStringXResultAtIndex(int xIndex)
        {
            AnalysisXResultCell cell = this.XResultAtIndex(xIndex);
            return cell != null ? cell.RawValue : string.Empty;
        }

        /// <summary>
        /// String result at index
        /// </summary>
        /// <param name="xIndex">X index</param>
        /// <returns>Returns string result at x index</returns>
        public string StringXResultAtIndex(int xIndex)
        {
            AnalysisXResultCell cell = this.XResultAtIndex(xIndex);
            return cell != null ? cell.StringValue : string.Empty;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Value} {this.XResultCells}";
        }

        /// <summary>
        /// Computes AnalysisXResultCell aggregated results
        /// </summary>
        /// <param name="lstAnalysis">
        /// List of <see cref="AnalysisProcessingResultColumnValue"/>
        /// </param>
        /// <param name="aggregationType">
        /// <see cref="AnalysisAggregationType"/> object
        /// </param>
        /// <param name="column">
        /// <see cref="AnalysisColumn"/> object.
        /// </param>
        /// <returns>
        /// List of <see cref="AnalysisXResultCell"/>
        /// </returns>
        private static List<AnalysisXResultCell> GetAnalysisXResultCells(
            List<AnalysisProcessingResultColumnValue> lstAnalysis,
            AnalysisAggregationType aggregationType,
            AnalysisColumn column)
        {
            var xResultCells = new List<AnalysisXResultCell>();
            var xCategoryValues = column.XCategoryValues;
            foreach (var xCategoryValue in xCategoryValues)
            {
                var result = 0.0;
                if (aggregationType.Sum)
                {
                    result = lstAnalysis?.Sum(x => x.ResultForXCategoryValueKey(xCategoryValue.Key)) ?? result;
                }
                else if (aggregationType.Min)
                {
                    if (lstAnalysis?.Any() == true)
                    {
                        result = lstAnalysis.Min(x => x.ResultForXCategoryValueKey(xCategoryValue.Key));
                    }
                }
                else if (aggregationType.Max)
                {
                    result = lstAnalysis?.Max(x => x.ResultForXCategoryValueKey(xCategoryValue.Key)) ?? result;
                }
                else
                {
                    var count = 0;
                    var currentCount = 0;
                    if (lstAnalysis?.Any() == true)
                    {
                        foreach (var colVal in lstAnalysis)
                        {
                            currentCount = colVal.CountForXCategoryValueKey(xCategoryValue.Key);
                            if (currentCount > 0)
                            {
                                result += colVal.ResultForXCategoryValueKey(xCategoryValue.Key) * currentCount;
                                count += currentCount;
                            }
                        }
                    }

                    result = count > 0
                        ? result / count
                        : 0;
                }

                xResultCells.Add(new AnalysisXResultCell(result, column.ResultColumn.DisplayStringFromNumber(result)));
            }

            return xResultCells;
        }

        /// <summary>
        /// Returns aggregated result from list basing on aggregationType
        /// </summary>
        /// <param name="aggregationType">
        /// <see cref="AnalysisAggregationType"/> object
        /// </param>
        /// <param name="lstAnalysis">
        /// List of <see cref="AnalysisProcessingResultColumnValue"/>
        /// </param>
        /// <returns>Aggregated Value</returns>
        private static double GetAggregatedResult(AnalysisAggregationType aggregationType, List<AnalysisProcessingResultColumnValue> lstAnalysis)
        {
            var result = 0.0;
            if (aggregationType.Sum)
            {
                result = lstAnalysis.Sum(x => x.Result);
            }
            else if (aggregationType.Min)
            {
                result = lstAnalysis.Min(x => x.Result);
            }
            else if (aggregationType.Max)
            {
                result = lstAnalysis.Max(x => x.Result);
            }
            else if (aggregationType.StaticAggregator)
            {
                result = lstAnalysis[0]?.Result ?? result;
            }
            else
            {
                var count = 0;
                foreach (var colVal in lstAnalysis)
                {
                    result += colVal.Result * colVal.Count;
                    count += colVal.Count;
                }

                result = count > 0
                    ? result / count
                    : 0;
            }

            return result;
        }

        private AnalysisXResultCell XResultAtIndex(int xIndex)
        {
            return this.XResultCells.Count > xIndex ? this.XResultCells[xIndex] : null;
        }
    }
}
