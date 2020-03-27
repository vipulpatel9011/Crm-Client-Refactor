// <copyright file="AnalysisExplicitCategoryField.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;
    using Configuration;
    using CRM;

    /// <summary>
    /// Implementation of analysis explicit category field
    /// </summary>
    public class AnalysisExplicitCategoryField : AnalysisField
    {
        private string categoryName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExplicitCategoryField"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="categoryName">Category name</param>
        public AnalysisExplicitCategoryField(Analysis analysis, AnalysisField parentField, string categoryName)
            : base(analysis, $"{parentField.Key}({categoryName})")
        {
            this.categoryName = categoryName;
            UPConfigAnalysisCategory configCategory = ConfigurationUnitStore.DefaultStore.AnalysisCategoryByName(this.categoryName);
            if (configCategory == null)
            {
                return;
            }

            this.ParentField = parentField;
            var field = this.ParentField as AnalysisExplicitCategoryField;
            this.SourceField = field != null ? field.SourceField : this.ParentField;

            this.ExplicitCategory = this.SourceField.IsDateValue
                                    ? new AnalysisExplicitDateCategory(analysis, configCategory)
                                    : new AnalysisExplicitCategory(analysis, configCategory);
        }

        /// <inheritdoc/>
        public override string CategoryName => this.categoryName;

        /// <summary>
        /// Gets explicit category
        /// </summary>
        public AnalysisExplicitCategory ExplicitCategory { get; private set; }

        /// <inheritdoc/>
        public override bool IsCategory => true;

        /// <inheritdoc/>
        public override bool IsDefaultCategory => this.ParentField.IsDefaultCategory;

        /// <inheritdoc/>
        public override bool IsFilter => false;

        /// <inheritdoc/>
        public override bool IsResultColumn => false;

        /// <inheritdoc/>
        public override bool IsXCategory => this.ParentField.IsXCategory;

        /// <inheritdoc/>
        public override string Label => this.ExplicitCategory.ConfigCategory.Label;

        /// <summary>
        /// Gets parent category field
        /// </summary>
        public AnalysisExplicitCategoryField ParentCategoryField => this.ParentField as AnalysisExplicitCategoryField;

        /// <summary>
        /// Gets parent field
        /// </summary>
        public AnalysisField ParentField { get; private set; }

        /// <summary>
        /// Gets source field
        /// </summary>
        public AnalysisField SourceField { get; private set; }

        /// <inheritdoc/>
        public override List<object> RawValueArrayForRow(ICrmDataSourceRow row)
        {
            string parentValue = this.SourceField.RawValueForRow(row);
            if (parentValue?.Length > 0)
            {
                List<object> values = this.ExplicitCategory.CategoryValueArrayForValue(parentValue);
                if (values.Count == 0)
                {
                    return null;
                }

                List<object> keyArray = new List<object> { values.Count };
                foreach (AnalysisExplicitCategoryValue explicitValue in values)
                {
                    keyArray.Add(explicitValue.Key);
                }

                return keyArray;
            }

            return null;
        }

        /// <inheritdoc/>
        public override string RawValueForRow(ICrmDataSourceRow row)
        {
            string parentValue = this.SourceField.RawValueForRow(row);
            if (parentValue?.Length > 0)
            {
                AnalysisExplicitCategoryValue value = this.ExplicitCategory.CategoryValueForValue(parentValue);
                return value.Key;
            }

            return null;
        }

        /// <inheritdoc/>
        public override string StringValueForRow(ICrmDataSourceRow row)
        {
            string rawValue = this.RawValueForRow(row);
            AnalysisExplicitCategoryValue categoryValue = this.ExplicitCategory.CategoryValueForValue(rawValue);
            return categoryValue.Label;
        }
    }
}
