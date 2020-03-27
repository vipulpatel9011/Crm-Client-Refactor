// <copyright file="AnalysisValueIntermediateResultDelayed.cs" company="Aurea Software Gmbh">
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
    using Extensions;
    using Processing;

    /// <summary>
    /// Implementation of analysis value intermediate result delayed
    /// </summary>
    public class AnalysisValueIntermediateResultDelayed : AnalysisValueIntermediateResult
    {
        private IAnalysisFunctionResultDelegate resultDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueIntermediateResultDelayed"/> class.
        /// </summary>
        /// <param name="resultDelegate">Result delegate</param>
        /// <param name="rowContext">Row context</param>
        public AnalysisValueIntermediateResultDelayed(IAnalysisFunctionResultDelegate resultDelegate, AnalysisProcessingQueryResultRowExecutionContext rowContext)
            : base(resultDelegate.Result)
        {
            this.resultDelegate = resultDelegate;
            this.RowContext = rowContext;
        }

        /// <inheritdoc/>
        public override bool Complete => false;

        /// <inheritdoc/>
        public override double NumberResult => this.resultDelegate.IsTextResult ? this.resultDelegate.TextResult.ToDouble() : this.resultDelegate.Result;

        /// <summary>
        /// Gets row context
        /// </summary>
        public AnalysisProcessingQueryResultRowExecutionContext RowContext { get; private set; }

        /// <inheritdoc/>
        public override string TextResult => this.resultDelegate.IsTextResult ? this.resultDelegate.TextResult : this.resultDelegate.Result.ToString("0.00");

        /// <inheritdoc/>
        public override string XCategoryKey => this.RowContext.XCategoryKey;

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ExecuteStep()
        {
            if (this.resultDelegate.IsTextResult)
            {
                return new AnalysisValueIntermediateResultWithRowContext(this.resultDelegate.TextResult, this.RowContext);
            }

            return new AnalysisValueIntermediateResultWithRowContext(this.resultDelegate.Result, this.RowContext);
        }
    }
}
