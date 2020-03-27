// <copyright file="AnalysisValueIntermediateResultWithFunction.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Value
{
    using AnalysisValueFunction;
    using Processing;

    /// <summary>
    /// Implementation of analysis value intermediate result with function
    /// </summary>
    public class AnalysisValueIntermediateResultWithFunction : AnalysisValueIntermediateResultWithFormula
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueIntermediateResultWithFunction"/> class.
        /// </summary>
        /// <param name="function">Function</param>
        /// <param name="argument">Argument</param>
        /// <param name="rowContext">Row context</param>
        /// <param name="valueContext">Value context</param>
        public AnalysisValueIntermediateResultWithFunction(AnalysisFunctionFunc function, AnalysisValueIntermediateResult argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
            : base(null, new System.Collections.Generic.List<object> { argument }, rowContext, valueContext)
        {
            this.Func = function;
        }

        /// <summary>
        /// Gets func
        /// </summary>
        public AnalysisFunctionFunc Func { get; private set; }

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ExecuteStep()
        {
            var argument = this.Arguments[0] as AnalysisValueIntermediateResult;
            AnalysisValueIntermediateResult result = argument.Complete ? argument : argument.ExecuteStep();
            return result.Complete 
                ? this.Func.ResultForArgumentRowVariableContext(result, this.RowContext, this.ValueContext) 
                : new AnalysisValueIntermediateResultWithFunction(this.Func, result, this.RowContext, this.ValueContext);
        }
    }
}
