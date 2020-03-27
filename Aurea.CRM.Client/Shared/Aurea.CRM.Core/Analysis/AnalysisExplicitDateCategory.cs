// <copyright file="AnalysisExplicitDateCategory.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Implementation of analysis explicit date category
    /// </summary>
    public class AnalysisExplicitDateCategory : AnalysisExplicitCategory
    {
        private Dictionary<string, object> categoriesPerYear;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExplicitDateCategory"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="category">Category</param>
        public AnalysisExplicitDateCategory(Analysis analysis, UPConfigAnalysisCategory category)
            : base(analysis, category)
        {
        }

        /// <inheritdoc/>
        public override List<object> CategoryValueArrayForValue(string value)
        {
            AnalysisExplicitCategoryValue catVal = this.CategoryValueForValue(value);
            return catVal != null ? new List<object> { catVal } : null;
        }

        /// <inheritdoc/>
        public override AnalysisExplicitCategoryValue CategoryValueForValue(string value)
        {
            if (value?.Length < 4)
            {
                return base.CategoryValueForValue(value);
            }

            string year = value.Substring(0, 4);
            var categoryValues = this.ValuesForYear(year);
            foreach (AnalysisExplicitCategoryValue categoryValue in categoryValues)
            {
                if (categoryValue.MatchesValue(value))
                {
                    return categoryValue;
                }
            }

            return null;
        }

        private static List<object> DateParameterSetForYear(string year)
        {
            return new List<object>
            {
                new List<object> { "YYYY", year },
                new List<object> { "YY", year.Substring(0, 2) },
                new List<object> { "%VAR:YEAR;", year },
                new List<object> { "{YEAR}", year }
            };
        }

        private List<object> ValuesForYear(string year)
        {
            var values = this.categoriesPerYear[year] as List<object>;
            if (values != null)
            {
                return values;
            }

            List<object> parameters = DateParameterSetForYear(year);
            List<object> arr = new List<object>();
            foreach (UPConfigAnalysisCategoryValue configValue in this.ConfigCategory.Values)
            {
                AnalysisExplicitCategoryValue categoryValue = new AnalysisExplicitCategoryValue(this, configValue, parameters);
                arr.Add(categoryValue);
                this.ValueDictionary.SetObjectForKey(categoryValue, categoryValue.Key);
            }

            if (this.categoriesPerYear == null)
            {
                this.categoriesPerYear = new Dictionary<string, object> { { year, arr } };
            }
            else
            {
                this.categoriesPerYear.SetObjectForKey(arr, year);
            }

            return arr;
        }
    }
}
