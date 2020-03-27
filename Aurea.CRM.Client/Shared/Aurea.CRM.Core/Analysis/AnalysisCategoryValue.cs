// <copyright file="AnalysisCategoryValue.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis
{
    /// <summary>
    /// Implementation of analysis category value
    /// </summary>
    public class AnalysisCategoryValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisCategoryValue"/> class.
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="key">Key</param>
        /// <param name="label">Label</param>
        public AnalysisCategoryValue(AnalysisCategory category, string key, string label)
        {
            this.Key = key;
            this.Label = label?.Length > 0 ? label : this.Key;

            this.Category = category;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisCategoryValue"/> class.
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="explicitCategoryValue">Explicit category value</param>
        public AnalysisCategoryValue(AnalysisExplicitCategoryFieldCategory category, AnalysisExplicitCategoryValue explicitCategoryValue)
            : this(category, explicitCategoryValue.Key, explicitCategoryValue.Label)
        {
            this.SubCategoryName = explicitCategoryValue.SubCategoryName;
        }

        /// <summary>
        /// Gets category
        /// </summary>
        public AnalysisCategory Category { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is empty value
        /// </summary>
        public bool IsEmptyValue => string.IsNullOrEmpty(this.Key);

        /// <summary>
        /// Gets key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets label
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Gets sub category name
        /// </summary>
        public string SubCategoryName { get; private set; }
    }
}
