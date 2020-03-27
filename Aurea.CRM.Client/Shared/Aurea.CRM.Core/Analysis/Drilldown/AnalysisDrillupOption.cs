// <copyright file="AnalysisDrillupOption.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Implementation of analysis drillup option
    /// </summary>
    public class AnalysisDrillupOption : AnalysisDrillOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisDrillupOption"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="removeCategory">Remove category</param>
        /// <param name="isBack">Is back</param>
        public AnalysisDrillupOption(Analysis analysis, AnalysisCategory removeCategory, bool isBack)
            : base(analysis)
        {
            this.IsBack = isBack;
            this.RemoveCategory = removeCategory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisDrillupOption"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="removeFilter">Remove filter</param>
        public AnalysisDrillupOption(Analysis analysis, AnalysisCategoryFilter removeFilter)
            : this(analysis, removeFilter.Category, false)
        {
            this.RemoveFilter = removeFilter;
        }

        /// <summary>
        /// Gets a value indicating whether is back
        /// </summary>
        public bool IsBack { get; private set; }

        /// <summary>
        /// Gets remove category
        /// </summary>
        public AnalysisCategory RemoveCategory { get; private set; }

        /// <summary>
        /// Gets remove filter
        /// </summary>
        public AnalysisCategoryFilter RemoveFilter { get; private set; }

        /// <summary>
        /// Gets key
        /// </summary>
        public override string Key => this.RemoveCategory.Key;

        /// <summary>
        /// Gets label
        /// </summary>
        public override string Label => this.RemoveCategory.Label;

        /// <summary>
        /// Drillup display string
        /// </summary>
        /// <returns>Returns drillup display string</returns>
        public string DrillupDisplayString()
        {
            if (this.RemoveFilter == null)
            {
                return this.Label;
            }

            string displayValue = this.RemoveFilter.DisplayValue() ?? LocalizedString.TextAnalysesNoValue;

            return $"{this.Label}:{displayValue}";
        }
    }
}
