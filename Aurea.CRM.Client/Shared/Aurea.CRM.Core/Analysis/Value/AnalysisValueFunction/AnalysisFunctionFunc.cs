// <copyright file="AnalysisFunctionFunc.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Value.AnalysisValueFunction
{
    using System.Collections.Generic;
    using AnalysisFunction;
    using CRM;
    using Processing;

    /// <summary>
    /// Implementation of analysis function func class
    /// </summary>
    public class AnalysisFunctionFunc : AnalysisValueFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionFunc"/> class.
        /// </summary>
        /// <param name="function">Function</param>
        /// <param name="argument">Argument</param>
        /// <param name="analysis">Analysis</param>
        public AnalysisFunctionFunc(AnalysisFunction function, AnalysisValueFunction argument, Analysis analysis)
            : base(analysis)
        {
            this.ArgumentFunction = argument;
            this.AnalysisFunction = function;
        }

        /// <summary>
        /// Gets analysis function
        /// </summary>
        public AnalysisFunction AnalysisFunction { get; private set; }

        /// <summary>
        /// Gets argument function
        /// </summary>
        public AnalysisValueFunction ArgumentFunction { get; private set; }

        /// <inheritdoc/>
        public override bool ReturnsObject => true;

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ObjectResultForResultRow(ICrmDataSourceRow row)
        {
            AnalysisValueIntermediateResult argumentResult = this.ArgumentFunction.ObjectResultForResultRow(row);
            if (argumentResult.Complete)
            {
                AnalysisValueIntermediateResult res = this.AnalysisFunction.ResultWithArgumentRowContextValueContext(argumentResult.JavascriptResult, null, null);
                if (res.Complete)
                {
                    return res;
                }
            }

            return new AnalysisValueIntermediateResult("configError: incomplete");
        }

        /// <summary>
        /// Result for argument row variable context
        /// </summary>
        /// <param name="result">Result row</param>
        /// <param name="row">Row</param>
        /// <param name="valueExecutionContext">Value.AnalysisValueFunction execution context</param>
        /// <returns>Returns analysis value intermediate result</returns>
        public AnalysisValueIntermediateResult ResultForArgumentRowVariableContext(AnalysisValueIntermediateResult result, AnalysisProcessingQueryResultRowExecutionContext row, AnalysisProcessingValueExecutionContext valueExecutionContext)
        {
            return this.AnalysisFunction.ResultWithArgumentRowContextValueContext(result.JavascriptResult, row, valueExecutionContext);
        }

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultForQueryRowVariableContext(AnalysisProcessingQueryResultRowExecutionContext row, AnalysisProcessingValueExecutionContext valueExecutionContext)
        {
            AnalysisProcessingValueExecutionContext childExecutionContext = valueExecutionContext.ChildExecutionContextAtIndex(0);
            AnalysisValueIntermediateResult result = this.ArgumentFunction.ResultForQueryRowVariableContext(row, childExecutionContext);
            if (result.Complete)
            {
                return this.AnalysisFunction.ResultWithArgumentRowContextValueContext(result.JavascriptResult, row, valueExecutionContext);
            }
            else
            {
                return new AnalysisValueIntermediateResultWithFunction(this, result, row, valueExecutionContext);
            }
        }

        /// <inheritdoc/>
        public override List<object> SignificantQueryResultTableIndices() => this.ArgumentFunction.SignificantQueryResultTableIndices();

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"FunctionFunc with {this.ArgumentFunction}";
        }
    }
}
