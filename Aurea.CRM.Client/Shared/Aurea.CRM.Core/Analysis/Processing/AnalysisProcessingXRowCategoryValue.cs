// <copyright file="AnalysisProcessingXRowCategoryValue.cs" company="Aurea Software Gmbh">
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
    /// <summary>
    /// Implementation of x row category value
    /// </summary>
    public class AnalysisProcessingXRowCategoryValue : AnalysisProcessingCategoryValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingXRowCategoryValue"/> class.
        /// </summary>
        /// <param name="processing">Processing</param>
        /// <param name="category">Category</param>
        public AnalysisProcessingXRowCategoryValue(AnalysisProcessing processing, AnalysisCategoryValue category)
            : base(processing, category)
        {
        }

        /// <summary>
        /// Gets x category value
        /// </summary>
        public AnalysisProcessingXCategoryValue XCategoryValue { get; private set; }

        /// <summary>
        /// Gets y category value
        /// </summary>
        public AnalysisProcessingYCategoryValue YCategoryValue { get; private set; }
    }
}
