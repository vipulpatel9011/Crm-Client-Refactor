// <copyright file="AnalysisValueIntermediateResultWithRowContext.cs" company="Aurea Software Gmbh">
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
    using Processing;

    /// <summary>
    /// Implementation of analysis value intermediate result with row context
    /// </summary>
    public class AnalysisValueIntermediateResultWithRowContext : AnalysisValueIntermediateResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueIntermediateResultWithRowContext"/> class.
        /// </summary>
        /// <param name="textResult">Text result</param>
        /// <param name="rowContext">Row context</param>
        public AnalysisValueIntermediateResultWithRowContext(string textResult, AnalysisProcessingQueryResultRowExecutionContext rowContext)
            : base(textResult)
        {
            this.RowContext = rowContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueIntermediateResultWithRowContext"/> class.
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="rowContext">Row context</param>
        public AnalysisValueIntermediateResultWithRowContext(double result, AnalysisProcessingQueryResultRowExecutionContext rowContext)
            : base(result)
        {
            this.RowContext = rowContext;
        }

        /// <summary>
        /// Gets row context
        /// </summary>
        public AnalysisProcessingQueryResultRowExecutionContext RowContext { get; private set; }

        /// <summary>
        /// Gets x category key
        /// </summary>
        public override string XCategoryKey => this.RowContext.XCategoryKey;
    }
}
