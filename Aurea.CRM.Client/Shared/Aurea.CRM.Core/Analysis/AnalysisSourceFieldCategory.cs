// <copyright file="AnalysisSourceFieldCategory.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Implementation of analysis source field category
    /// </summary>
    public class AnalysisSourceFieldCategory : AnalysisCategory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisSourceFieldCategory"/> class.
        /// </summary>
        /// <param name="field">Field</param>
        public AnalysisSourceFieldCategory(AnalysisSourceField field)
        {
            this.AnalysisField = field;
        }

        /// <summary>
        /// Gets analysis field
        /// </summary>
        public AnalysisSourceField AnalysisField { get; private set; }

        /// <inheritdoc/>
        public override bool ArrayCategory => this.AnalysisField.SubFieldQueryResultFieldIndices.Count > 0;

        /// <inheritdoc/>
        public override string Key => this.AnalysisField.Key;

        /// <inheritdoc/>
        public override string Label => this.AnalysisField.Label;

        /// <inheritdoc/>
        public override int MaxNumberOfRows => this.AnalysisField.ConfigField.Slices > 0
                                                ? this.AnalysisField.ConfigField.Slices
                                                : this.AnalysisField.Analysis.Configuration.MaxBars;

        /// <inheritdoc/>
        public override AnalysisField BaseField()
        {
            return this.AnalysisField;
        }

        /// <inheritdoc/>
        public override List<object> CategoriesForRow(ICrmDataSourceRow row)
        {
            var resultArray = new List<object>();
            AnalysisCategoryValue v = this.CategoryValueForRow(row);
            if (v != null && !v.IsEmptyValue)
            {
                resultArray.Add(v);
            }

            foreach (var subIndices in this.AnalysisField.SubFieldQueryResultFieldIndices)
            {
                string rawValue = row.RawValueAtIndex(subIndices.ToInt());
                if (rawValue != null)
                {
                    v = this.ValueDictionary.ValueOrDefault(rawValue) as AnalysisCategoryValue;
                    if (v == null)
                    {
                        if (!this.AnalysisField.CrmFieldInfo.IsEmptyValue(rawValue))
                        {
                            v = new AnalysisCategoryValue(this, rawValue, row.ValueAtIndex(subIndices.ToInt()));
                            if (v != null)
                            {
                                this.AddToValueDictionary(v);
                            }
                        }
                    }

                    if (v != null)
                    {
                        resultArray.Add(v);
                    }
                }
            }

            if (resultArray.Count > 0)
            {
                return resultArray;
            }

            return new List<object> { this.EmptyValue() };
        }

        /// <inheritdoc/>
        public override AnalysisCategoryValue CategoryValueForRow(ICrmDataSourceRow row)
        {
            string rawValue = this.AnalysisField.RawValueForRow(row);
            if (rawValue != null)
            {
                AnalysisCategoryValue v = this.ValueDictionary.ValueOrDefault(rawValue) as AnalysisCategoryValue;
                if (v == null)
                {
                    v = this.AnalysisField.CrmFieldInfo.IsEmptyValue(rawValue)
                        ? this.EmptyValue()
                        : new AnalysisCategoryValue(this, rawValue, this.AnalysisField.StringValueForRow(row));

                    this.AddToValueDictionary(v);
                }

                return v;
            }

            return null;
        }

        /// <inheritdoc/>
        public override string KeyForRow(ICrmDataSourceRow row)
        {
            return this.AnalysisField.IsEmptyForRow(row) ? string.Empty : this.AnalysisField.RawValueForRow(row);
        }

        /// <inheritdoc/>
        public override bool SortByFirstColumnValue()
        {
            return (this.AnalysisField.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.DoNotSort) != 0;
        }

        /// <inheritdoc/>
        public override bool SortByKey()
        {
            if ((this.AnalysisField.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.DoNotSort) != 0)
            {
                return false;
            }

            return this.AnalysisField.CrmFieldInfo.FieldType == "D";
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.AnalysisField.Key}";
        }

        /// <inheritdoc/>
        public override string ValueForRawValue(string rawValue)
        {
            return this.AnalysisField.ValueForRawValue(rawValue);
        }
    }
}
