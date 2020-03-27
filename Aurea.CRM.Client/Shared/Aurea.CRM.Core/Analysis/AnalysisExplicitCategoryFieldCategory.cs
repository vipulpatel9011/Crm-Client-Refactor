// <copyright file="AnalysisExplicitCategoryFieldCategory.cs" company="Aurea Software Gmbh">
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
    using Extensions;

    /// <summary>
    /// Analysis explicit category field category
    /// </summary>
    public class AnalysisExplicitCategoryFieldCategory : AnalysisCategory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExplicitCategoryFieldCategory"/> class.
        /// </summary>
        /// <param name="field">Field</param>
        public AnalysisExplicitCategoryFieldCategory(AnalysisExplicitCategoryField field)
        {
            this.AnalysisField = field;
        }

        /// <summary>
        /// Gets analysis field
        /// </summary>
        public AnalysisExplicitCategoryField AnalysisField { get; private set; }

        /// <inheritdoc/>
        public override bool ArrayCategory => this.AnalysisField.ExplicitCategory.ConfigCategory.MultiValue != 0;

        /// <inheritdoc/>
        public override bool IsExplicitCategory => true;

        /// <inheritdoc/>
        public override string Key => this.AnalysisField.Key;

        /// <inheritdoc/>
        public override string Label => this.AnalysisField.Label;

        /// <inheritdoc/>
        public override int MaxNumberOfRows => 0;

        /// <inheritdoc/>
        public override AnalysisField BaseField()
        {
            return this.AnalysisField;
        }

        /// <inheritdoc/>
        public override List<object> CategoriesForRow(ICrmDataSourceRow row)
        {
            List<object> rawValues = this.AnalysisField?.RawValueArrayForRow(row);
            if (rawValues == null || rawValues.Count == 0)
            {
                return null;
            }

            List<object> valueArray = new List<object>();
            foreach (string rawValue in rawValues)
            {
                AnalysisCategoryValue v = this.ValueDictionary.ValueOrDefault(rawValue) as AnalysisCategoryValue;
                if (v == null)
                {
                    AnalysisExplicitCategoryValue explicitCategoryValue = this.AnalysisField.ExplicitCategory.CategoryValueForKey(rawValue);
                    if (explicitCategoryValue != null)
                    {
                        v = new AnalysisCategoryValue(this, explicitCategoryValue);
                        this.AddToValueDictionary(v);
                    }
                }

                if (v != null)
                {
                    valueArray.Add(v);
                }
            }

            if (valueArray.Count == 0)
            {
                valueArray = null;
            }

            return valueArray;
        }

        /// <inheritdoc/>
        public override AnalysisCategoryValue CategoryValueForRow(ICrmDataSourceRow row)
        {
            string rawValue = this.AnalysisField.RawValueForRow(row);
            if (!string.IsNullOrEmpty(rawValue))
            {
                AnalysisCategoryValue v = this.ValueDictionary.ValueOrDefault(rawValue) as AnalysisCategoryValue;
                if (v == null)
                {
                    AnalysisExplicitCategoryValue explicitCategoryValue = this.AnalysisField.ExplicitCategory.CategoryValueForKey(rawValue);
                    if (explicitCategoryValue != null)
                    {
                        v = new AnalysisCategoryValue(this, explicitCategoryValue);
                        this.AddToValueDictionary(v);
                    }
                }

                return v;
            }

            return null;
        }

        /// <inheritdoc/>
        public override string KeyForRow(ICrmDataSourceRow row)
        {
            return this.AnalysisField.RawValueForRow(row);
        }

        /// <inheritdoc/>
        public override bool SortByFirstColumnValue()
        {
            AnalysisSourceField sourceField = (AnalysisSourceField)this.AnalysisField.ParentField;
            if (sourceField != null)
            {
                return (sourceField.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.DoNotSort) == 0;
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool SortByKey()
        {
            AnalysisSourceField sourceField = (AnalysisSourceField)this.AnalysisField?.ParentField;
            if (sourceField != null)
            {
                return (sourceField.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.DoNotSort) != 0;
            }

            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.AnalysisField?.Key}";
        }

        /// <inheritdoc/>
        public override string ValueForRawValue(string rawValue)
        {
            AnalysisExplicitCategoryValue explicitCategoryValue = this.AnalysisField?.ExplicitCategory.CategoryValueForKey(rawValue);
            return explicitCategoryValue != null ? explicitCategoryValue.Label : base.ValueForRawValue(rawValue);
        }
    }
}
