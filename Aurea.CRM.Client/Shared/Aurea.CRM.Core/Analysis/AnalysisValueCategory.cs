// <copyright file="AnalysisValueCategory.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Analysis.Model;
    using Aurea.CRM.Core.Analysis.Value;
    using Aurea.CRM.Core.Analysis.Value.AnalysisValueFunction;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Implementation of analysis value category
    /// </summary>
    public class AnalysisValueCategory : AnalysisCategory
    {
        private AnalysisValueFunction valueFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueCategory"/> class.
        /// </summary>
        public AnalysisValueCategory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueCategory"/> class.
        /// </summary>
        /// <param name="field">Field</param>
        public AnalysisValueCategory(AnalysisValueField field)
        {
            this.AnalysisValueField = field;
            this.valueFunction = new AnalysisValueResultColumn(field).ValueFunction;
        }

        /// <summary>
        /// Gets analysis value field
        /// </summary>
        public AnalysisValueField AnalysisValueField { get; private set; }

        /// <inheritdoc/>
        public override bool ArrayCategory => true;

        /// <inheritdoc/>
        public override bool IsValueCategory => true;

        /// <inheritdoc/>
        public override string Key => this.AnalysisValueField.Key;

        /// <inheritdoc/>
        public override string Label => this.AnalysisValueField.Label;

        /// <inheritdoc/>
        public override AnalysisField BaseField()
        {
            return this.AnalysisValueField;
        }

        /// <inheritdoc/>
        public override List<object> CategoriesForRow(ICrmDataSourceRow row)
        {
            List<object> stringValueArray;
            if (this.valueFunction == null)
            {
                stringValueArray = new List<object> { "invalid" };
            }
            else
            {
                AnalysisValueIntermediateResult result = this.valueFunction.ObjectResultForResultRow(row);
                if (result.IsTextResult)
                {
                    stringValueArray = new List<object> { result.TextResult };
                }
                else if (result.IsObject)
                {
                    stringValueArray = result.ArrayResult;
                    var arr = new List<object>();
                    foreach (object v in stringValueArray)
                    {
                        if (v is string)
                        {
                            arr.Add((string)v);
                        }
                    }

                    stringValueArray = arr;
                }
                else
                {
                    stringValueArray = new List<object> { result.TextResult };
                }
            }

            List<object> categoryValueArray = null;
            foreach (string rawValue in stringValueArray)
            {
                AnalysisCategoryValue categoryValue = this.ValueDictionary.ValueOrDefault(rawValue) as AnalysisCategoryValue;
                if (categoryValue == null)
                {
                    categoryValue = new AnalysisCategoryValue(this, rawValue, rawValue);
                    if (categoryValue != null)
                    {
                        this.AddToValueDictionary(categoryValue);
                    }
                }

                if (categoryValue != null)
                {
                    if (categoryValueArray == null)
                    {
                        categoryValueArray = new List<object> { categoryValue };
                    }
                    else
                    {
                        categoryValueArray.Add(categoryValue);
                    }
                }
            }

            return categoryValueArray;
        }

        /// <inheritdoc/>
        public override AnalysisCategoryValue CategoryValueForRow(ICrmDataSourceRow row)
        {
            string rawValue = this.valueFunction == null ? "invalid" : this.valueFunction.TextResultForResultRow(row);

            if (!string.IsNullOrEmpty(rawValue))
            {
                AnalysisCategoryValue categoryValue = this.ValueDictionary.ValueOrDefault(rawValue) as AnalysisCategoryValue;
                if (categoryValue == null)
                {
                    categoryValue = new AnalysisCategoryValue(this, rawValue, rawValue);
                    if (categoryValue != null)
                    {
                        this.AddToValueDictionary(categoryValue);
                    }
                }

                return categoryValue;
            }

            return null;
        }

        /// <inheritdoc/>
        public override string KeyForRow(ICrmDataSourceRow row)
        {
            return this.valueFunction == null ? "invalid" : this.valueFunction.TextResultForResultRow(row);
        }

        /// <inheritdoc/>
        public override bool SortByFirstColumnValue()
        {
            return !this.AnalysisValueField.Options.IsSortCategory;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.AnalysisValueField.Key}";
        }

        /// <inheritdoc/>
        public override string ValueForRawValue(string rawValue)
        {
            return rawValue;
        }
    }
}
