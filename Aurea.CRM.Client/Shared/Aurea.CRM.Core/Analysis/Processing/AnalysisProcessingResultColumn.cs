// <copyright file="AnalysisProcessingResultColumn.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Analysis.Model;

    /// <summary>
    /// Implementation of result column
    /// </summary>
    public class AnalysisProcessingResultColumn
    {
        private AnalysisProcessingValueExecutionContext executionContext;
        private AnalysisProcessingValueExecutionContext sumExecutionContext;
        private AnalysisProcessingValueExecutionContext sumXExecutionContext;
        private AnalysisProcessingValueExecutionContext xExecutionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingResultColumn"/> class.
        /// </summary>
        /// <param name="resultColumn">Result column</param>
        /// <param name="processingContext">Processing context</param>
        public AnalysisProcessingResultColumn(AnalysisResultColumn resultColumn, AnalysisProcessing processingContext)
        {
            this.ResultColumn = resultColumn;
            this.ProcessingContext = processingContext;
        }

        /// <summary>
        /// Gets execution context
        /// </summary>
        public AnalysisProcessingValueExecutionContext ExecutionContext => this.executionContext ?? (this.executionContext = new AnalysisProcessingValueExecutionContext());

        /// <summary>
        /// Gets processing context
        /// </summary>
        public AnalysisProcessing ProcessingContext { get; private set; }

        /// <summary>
        /// Gets result column
        /// </summary>
        public AnalysisResultColumn ResultColumn { get; private set; }

        /// <summary>
        /// Gets sum execution context
        /// </summary>
        public AnalysisProcessingValueExecutionContext SumExecutionContext => this.sumExecutionContext ??
                                                                              (this.sumExecutionContext = new AnalysisProcessingValueExecutionContext());

        /// <summary>
        /// Gets sum x execution context
        /// </summary>
        public AnalysisProcessingValueExecutionContext SumXExecutionContext => this.sumXExecutionContext ??
                                                                               (this.sumXExecutionContext = new AnalysisProcessingValueExecutionContext());

        /// <summary>
        /// Gets x execution context
        /// </summary>
        public AnalysisProcessingValueExecutionContext XExecutionContext => this.xExecutionContext ??
                                                                            (this.xExecutionContext = new AnalysisProcessingValueExecutionContext());
    }
}
