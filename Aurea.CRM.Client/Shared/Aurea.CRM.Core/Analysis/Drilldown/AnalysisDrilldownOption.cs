// <copyright file="AnalysisDrilldownOption.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Drilldown
{
    /// <summary>
    /// Implementation of analysis drilldown option class
    /// </summary>
    public class AnalysisDrilldownOption : AnalysisDrillOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisDrilldownOption"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="category">Category</param>
        public AnalysisDrilldownOption(Analysis analysis, AnalysisCategory category)
            : base(analysis)
        {
            this.Category = category;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisDrilldownOption"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="categoryName">Category name</param>
        public AnalysisDrilldownOption(Analysis analysis, AnalysisField parentField, string categoryName)
            : base(analysis)
        {
            AnalysisExplicitCategoryField explicitCategoryField = new AnalysisExplicitCategoryField(analysis, parentField, categoryName);
            if (explicitCategoryField != null)
            {
                this.ParentField = parentField;
                this.Category = new AnalysisExplicitCategoryFieldCategory(explicitCategoryField);
                if (this.Category == null)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Gets category
        /// </summary>
        public AnalysisCategory Category { get; private set; }

        /// <summary>
        /// Gets parent field
        /// </summary>
        public AnalysisField ParentField { get; private set; }

        /// <inheritdoc/>
        public override string Key => this.Category.Key;

        /// <inheritdoc/>
        public override string Label => this.Category.Label;
    }
}
