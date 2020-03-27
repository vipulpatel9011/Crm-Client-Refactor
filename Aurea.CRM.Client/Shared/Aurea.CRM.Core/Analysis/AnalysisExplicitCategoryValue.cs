// <copyright file="AnalysisExplicitCategoryValue.cs" company="Aurea Software Gmbh">
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

    /// <summary>
    /// Implementation of analysis explicit category value
    /// </summary>
    public class AnalysisExplicitCategoryValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExplicitCategoryValue"/> class.
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="configCategoryValue">Config category value</param>
        /// <param name="parameters">Parameters</param>
        public AnalysisExplicitCategoryValue(AnalysisExplicitCategory category, UPConfigAnalysisCategoryValue configCategoryValue, List<object> parameters = null)
        {
            this.Category = category;
            this.ConfigCategoryValue = configCategoryValue;
            this.Key = this.ConfigCategoryValue.RefValue;
            this.Label = this.ConfigCategoryValue.Label;
            if (parameters != null)
            {
                foreach (List<object> param in parameters)
                {
                    var paramKey = param[0] as string;
                    var value = param[1] as string;
                    this.Key = this.Key.Replace(paramKey, value);
                    this.Label = this.Label.Replace(paramKey, value);
                }
            }

            var arr = new List<object>();
            foreach (UPConfigAnalysisCategoryCondition configCondition in configCategoryValue.Conditions)
            {
                arr.Add(new AnalysisExplicitCategoryCondition(configCondition, parameters));
            }

            this.Conditions = arr;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExplicitCategoryValue"/> class.
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="otherLabel">Other label</param>
        public AnalysisExplicitCategoryValue(AnalysisExplicitCategory category, string otherLabel)
        {
            this.Category = category;
            this.Key = "OTHER";
            this.Label = otherLabel;
        }

        /// <summary>
        /// Gets category
        /// </summary>
        public AnalysisExplicitCategory Category { get; private set; }

        /// <summary>
        /// Gets conditions
        /// </summary>
        public List<object> Conditions { get; private set; }

        /// <summary>
        /// Gets config category value
        /// </summary>
        public UPConfigAnalysisCategoryValue ConfigCategoryValue { get; private set; }

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
        public string SubCategoryName => this.ConfigCategoryValue.SubCategoryName;

        /// <summary>
        /// Checks matches given value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Boolean true if matches</returns>
        public bool MatchesValue(string value)
        {
            foreach (AnalysisExplicitCategoryCondition condition in this.Conditions)
            {
                if (condition.MatchesValue(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Key}: {this.Conditions}";
        }
    }
}
