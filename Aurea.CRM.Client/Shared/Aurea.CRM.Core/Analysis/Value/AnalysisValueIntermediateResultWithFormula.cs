// <copyright file="AnalysisValueIntermediateResultWithFormula.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;
    using AnalysisValueFunction;
    using Processing;

    /// <summary>
    /// Implementation of analysis value intermediate result with formula
    /// </summary>
    public class AnalysisValueIntermediateResultWithFormula : AnalysisValueIntermediateResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueIntermediateResultWithFormula"/> class.
        /// </summary>
        /// <param name="functionFormula">Function formula</param>
        /// <param name="arguments">Arguments</param>
        /// <param name="rowContext">Row context</param>
        /// <param name="valueContext">Value context</param>
        public AnalysisValueIntermediateResultWithFormula(AnalysisFunctionFormula functionFormula, List<object> arguments, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
            : base(functionFormula.FormulaFunction)
        {
            this.Arguments = arguments;
            this.Formula = functionFormula;
            this.RowContext = rowContext;
            this.ValueContext = valueContext;
            this.Complete = false;
        }

        /// <summary>
        /// Gets arguments
        /// </summary>
        public List<object> Arguments { get; private set; }

        /// <summary>
        /// Gets formula
        /// </summary>
        public AnalysisFunctionFormula Formula { get; private set; }

        /// <summary>
        /// Gets row context
        /// </summary>
        public AnalysisProcessingQueryResultRowExecutionContext RowContext { get; private set; }

        /// <summary>
        /// Gets value context
        /// </summary>
        public AnalysisProcessingValueExecutionContext ValueContext { get; private set; }

        /// <summary>
        /// Gets x category key
        /// </summary>
        public override string XCategoryKey => this.RowContext.XCategoryKey;

        /// <summary>
        /// Execute step
        /// </summary>
        /// <returns>Intermediate result</returns>
        public override AnalysisValueIntermediateResult ExecuteStep()
        {
            bool complete = true;
            var objArray = new List<object>();
            foreach (AnalysisValueIntermediateResult arg in this.Arguments)
            {
                if (!arg.Complete)
                {
                    AnalysisValueIntermediateResult res = arg.ExecuteStep();
                    if (!res.Complete)
                    {
                        complete = false;
                    }

                    objArray.Add(res);
                }
                else
                {
                    objArray.Add(arg);
                }
            }

            if (complete)
            {
                AnalysisValueIntermediateResult res = this.Formula.ObjectResultForArguments(objArray);
                res.XCategoryKey = this.XCategoryKey;
                return res;
            }

            return new AnalysisValueIntermediateResultWithFormula(this.Formula, objArray, this.RowContext, this.ValueContext);
        }
    }
}
