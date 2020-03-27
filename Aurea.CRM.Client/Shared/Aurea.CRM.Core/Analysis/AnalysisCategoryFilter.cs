// <copyright file="AnalysisCategoryFilter.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implementation for analysis category filter
    /// </summary>
    public class AnalysisCategoryFilter : AnalysisFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisCategoryFilter"/> class.
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="val">Value</param>
        public AnalysisCategoryFilter(AnalysisCategory category, string val)
            : base(val, val)
        {
            this.Category = category;
        }

        /// <summary>
        /// Gets category
        /// </summary>
        public AnalysisCategory Category { get; private set; }

        /// <inheritdoc />
        public override string Key => this.Category.Key;

        /// <inheritdoc />
        public override string DisplayValue()
        {
            return this.Category.ValueForRawValue(this.FilterValue);
        }

        /// <inheritdoc />
        public override string RawValueForRow(ICrmDataSourceRow row)
        {
            return this.Category.KeyForRow(row);
        }
    }
}
