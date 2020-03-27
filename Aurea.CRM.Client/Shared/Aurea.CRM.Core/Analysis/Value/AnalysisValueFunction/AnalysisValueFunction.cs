// <copyright file="AnalysisValueFunction.cs" company="Aurea Software Gmbh">
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
    /// Implementation of analysis value function class
    /// </summary>
    public class AnalysisValueFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueFunction"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        public AnalysisValueFunction(Analysis analysis)
        {
            this.Analysis = analysis;
        }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets a value indicating whether returns nubmer
        /// </summary>
        public virtual bool ReturnsNumber => false;

        /// <summary>
        /// Gets a value indicating whether returns object
        /// </summary>
        public virtual bool ReturnsObject => false;

        /// <summary>
        /// Gets a value indicating whether returns text
        /// </summary>
        public virtual bool ReturnsText => false;

        /// <summary>
        /// Value.AnalysisValueFunction function for formula analysis
        /// </summary>
        /// <param name="formula">Formula</param>
        /// <param name="analysis">Analysis</param>
        /// <returns>Value.AnalysisValueFunction function</returns>
        public static AnalysisValueFunction ValueFunctionForFormulaAnalysis(string formula, Analysis analysis)
        {
            AnalysisFunctionParser parser = new AnalysisFunctionParser(analysis, formula);
            if (parser?.Function != null && parser?.Unparsed?.Length == 0)
            {
                return parser.Function;
            }

            return null;
        }

        /// <summary>
        /// Number result for result row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Number result</returns>
        public virtual double NumberResultForResultRow(ICrmDataSourceRow row) => 0;

        /// <summary>
        /// Object result for result row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Object resutl</returns>
        public virtual AnalysisValueIntermediateResult ObjectResultForResultRow(ICrmDataSourceRow row) => null;

        /// <summary>
        /// Result for query row variable context
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="valueExecutionContext">Value execution context</param>
        /// <returns>Result for query row</returns>
        public virtual AnalysisValueIntermediateResult ResultForQueryRowVariableContext(AnalysisProcessingQueryResultRowExecutionContext row, AnalysisProcessingValueExecutionContext valueExecutionContext) => this.ObjectResultForResultRow(row.Row);

        /// <summary>
        /// Significant query result
        /// </summary>
        /// <returns>List of table indices</returns>
        public virtual List<object> SignificantQueryResultTableIndices() => null;

        /// <summary>
        /// Text result for result row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Text result</returns>
        public virtual string TextResultForResultRow(ICrmDataSourceRow row)
        {
            AnalysisValueIntermediateResult res = this.ObjectResultForResultRow(row);
            if (res != null)
            {
                return res.TextResult;
            }

            return string.Empty;
        }
    }
}
