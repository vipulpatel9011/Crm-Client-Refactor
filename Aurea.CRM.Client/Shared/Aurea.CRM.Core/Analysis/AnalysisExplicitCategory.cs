// <copyright file="AnalysisExplicitCategory.cs" company="Aurea Software Gmbh">
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
    /// Implementation of analysis explicit category
    /// </summary>
    public class AnalysisExplicitCategory
    {
        private AnalysisExplicitCategoryValue otherValue;
        private List<object> sourceValueArray;
        private Dictionary<string, object> valueDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExplicitCategory"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="configCategory">Config category</param>
        public AnalysisExplicitCategory(Analysis analysis, UPConfigAnalysisCategory configCategory)
        {
            this.Analysis = analysis;
            this.ConfigCategory = configCategory;
            this.valueDictionary = new Dictionary<string, object>();
            this.sourceValueArray = new List<object>();
            foreach (UPConfigAnalysisCategoryValue configValue in configCategory.Values)
            {
                AnalysisExplicitCategoryValue value = new AnalysisExplicitCategoryValue(this, configValue);
                this.valueDictionary.SetObjectForKey(value, value.Key);
                this.sourceValueArray.Add(value);
            }

            if (this.ConfigCategory?.OtherLabel?.Length > 0)
            {
                this.otherValue = new AnalysisExplicitCategoryValue(this, this.ConfigCategory.OtherLabel);
                if (this.otherValue != null)
                {
                    this.valueDictionary.SetObjectForKey(this.otherValue, this.otherValue.Key);
                    this.sourceValueArray.Add(this.otherValue);
                }
            }
            else
            {
                this.otherValue = null;
            }
        }

        /// <summary>
        /// Gets or sets value dictionary
        /// </summary>
        public Dictionary<string, object> ValueDictionary
        {
            get
            {
                return this.valueDictionary;
            }

            protected set
            {
                this.valueDictionary = value;
            }
        }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets config category
        /// </summary>
        public UPConfigAnalysisCategory ConfigCategory { get; private set; }

        /// <summary>
        /// Gets category value array for value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Category value array</returns>
        public virtual List<object> CategoryValueArrayForValue(string value)
        {
            List<object> v = new List<object>();
            foreach (AnalysisExplicitCategoryValue categoryValue in this.sourceValueArray)
            {
                if (categoryValue.MatchesValue(value))
                {
                    v.Add(categoryValue);
                }
            }

            if (v.Count > 0)
            {
                return v;
            }

            if (this.otherValue != null && !string.IsNullOrEmpty(value) && value != "0")
            {
                return new List<object> { this.otherValue };
            }

            return null;
        }

        /// <summary>
        /// Category value for key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns category value for key</returns>
        public virtual AnalysisExplicitCategoryValue CategoryValueForKey(string key)
        {
            return this.valueDictionary[key] as AnalysisExplicitCategoryValue;
        }

        /// <summary>
        /// Category value for value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns category value for value</returns>
        public virtual AnalysisExplicitCategoryValue CategoryValueForValue(string value)
        {
            foreach (AnalysisExplicitCategoryValue categoryValue in this.sourceValueArray)
            {
                if (categoryValue.MatchesValue(value))
                {
                    return categoryValue;
                }
            }

            return !string.IsNullOrEmpty(value) && value != "0" ? this.otherValue : null;
        }

        /// <summary>
        /// All values
        /// </summary>
        /// <returns>Returns all values</returns>
        private List<object> AllValues()
        {
            return this.sourceValueArray;
        }
    }
}
