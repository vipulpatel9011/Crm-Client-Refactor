// <copyright file="AnalysisProcessingCategoryValue.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Implementation of category value
    /// </summary>
    public class AnalysisProcessingCategoryValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingCategoryValue"/> class.
        /// </summary>
        /// <param name="processing">Processing</param>
        /// <param name="category">Category</param>
        public AnalysisProcessingCategoryValue(AnalysisProcessing processing, AnalysisCategoryValue category)
        {
            this.Category = category;
            this.Processing = processing;
            this.IsSumLine = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingCategoryValue"/> class.
        /// </summary>
        /// <param name="analysisProcessing">Analysis processing</param>
        public AnalysisProcessingCategoryValue(AnalysisProcessing analysisProcessing)
            : this(analysisProcessing, new AnalysisCategoryValue(analysisProcessing.Analysis.DefaultCategory, "sum", LocalizedString.TextAnalysesSum))
        {
            this.IsSumLine = true;
        }

        /// <summary>
        /// Gets category
        /// </summary>
        public AnalysisCategoryValue Category { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is sum line
        /// </summary>
        public bool IsSumLine { get; protected set; }

        /// <summary>
        /// Gets key
        /// </summary>
        public string Key => this.Category.Key;

        /// <summary>
        /// Gets label
        /// </summary>
        public string Label => this.Category.Label;

        /// <summary>
        /// Gets procesing
        /// </summary>
        public AnalysisProcessing Processing { get; private set; }

        /// <inheritdoc/>
        public override string ToString() => this.Key;
    }
}
