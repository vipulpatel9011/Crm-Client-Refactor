// <copyright file="AnalysisFilter.cs" company="Aurea Software Gmbh">
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
    using Utilities;

    /// <summary>
    /// Implementation for analysis filter class
    /// </summary>
    public class AnalysisFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFilter"/> class.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="valueTo">Value to</param>
        public AnalysisFilter(string value, string valueTo)
        {
            this.FilterValue = value;
            this.FilterValueTo = valueTo;
            ConditionCheckOperator compareOperator = valueTo?.Length > 0 ? ConditionCheckOperator.Between : ConditionCheckOperator.Equal;
            this.ConditionChecker = new ConditionChecker(compareOperator, value, valueTo);
        }

        /// <summary>
        /// Gets condition checker
        /// </summary>
        public ConditionChecker ConditionChecker { get; private set; }

        /// <summary>
        /// Gets or sets filter value
        /// </summary>
        public string FilterValue { get; set; }

        /// <summary>
        /// Gets or sets filter value to
        /// </summary>
        public string FilterValueTo { get; set; }

        /// <summary>
        /// Gets a value indicating whether has filter values
        /// </summary>
        public bool HasFilterValues => this.ConditionChecker.Value?.Length > 0 || this.ConditionChecker.ValueTo?.Length > 0;

        /// <summary>
        /// Gets key
        /// </summary>
        public virtual string Key => null;

        /// <summary>
        /// Display value
        /// </summary>
        /// <returns>Returns display value</returns>
        public virtual string DisplayValue()
        {
            if (this.FilterValueTo?.Length > 0)
            {
                return $"{this.ValueForRawValue(this.FilterValue)}-{this.ValueForRawValue(this.FilterValueTo)}";
            }

            return this.ValueForRawValue(this.FilterValue);
        }

        /// <summary>
        /// Checks if row matches
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>True if matches</returns>
        public bool MatchesRow(ICrmDataSourceRow row)
        {
            return this.ConditionChecker.MatchesString(this.RawValueForRow(row));
        }

        /// <summary>
        /// Raw value for row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Raw value</returns>
        public virtual string RawValueForRow(ICrmDataSourceRow row)
        {
            return null;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.ConditionChecker.ToString();
        }

        /// <summary>
        /// Value for category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Value</returns>
        public string ValueForCategory(AnalysisCategory category)
        {
            return (this.ConditionChecker.ConditionOperator == ConditionCheckOperator.Equal && this.Key == category.Key) ? this.ConditionChecker.Value : null;
        }

        /// <summary>
        /// Value for raw value
        /// </summary>
        /// <param name="rawValue">Raw value</param>
        /// <returns>Value</returns>
        public virtual string ValueForRawValue(string rawValue)
        {
            return rawValue;
        }
    }
}
