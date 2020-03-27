// <copyright file="AnalysisProcessingQueryResultRowExecutionContext.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implementation of query result row execution context
    /// </summary>
    public class AnalysisProcessingQueryResultRowExecutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingQueryResultRowExecutionContext"/> class.
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <param name="yCategoryValue">Y category value</param>
        /// <param name="xCategoryValue">X category value</param>
        /// <param name="processingContext">Processing context</param>
        public AnalysisProcessingQueryResultRowExecutionContext(ICrmDataSourceRow dataSource, AnalysisProcessingYCategoryValue yCategoryValue, AnalysisProcessingXCategoryValue xCategoryValue, AnalysisProcessing processingContext)
        {
            this.Row = dataSource;
            this.YCategoryValue = yCategoryValue;
            this.XCategoryValue = xCategoryValue;
            this.ProcessingContext = processingContext;
        }

        /// <summary>
        /// Gets y category key
        /// </summary>
        public string YCategoryKey => this.YCategoryValue.Key;

        /// <summary>
        /// Gets x category key
        /// </summary>
        public string XCategoryKey => this.XCategoryValue.Key;

        /// <summary>
        /// Gets x category value
        /// </summary>
        public AnalysisProcessingXCategoryValue XCategoryValue { get; private set; }

        /// <summary>
        /// Gets y category value
        /// </summary>
        public AnalysisProcessingYCategoryValue YCategoryValue { get; private set; }

        /// <summary>
        /// Gets row
        /// </summary>
        public ICrmDataSourceRow Row { get; private set; }

        /// <summary>
        /// Gets processing context
        /// </summary>
        public AnalysisProcessing ProcessingContext { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Y:{this.YCategoryValue.Key}, X:{this.XCategoryValue.Key}";
        }
    }
}
