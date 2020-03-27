// <copyright file="AnalysisCategory.cs" company="Aurea Software Gmbh">
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
    /// Implementation of analysis category
    /// </summary>
    public class AnalysisCategory
    {
        private AnalysisCategoryValue emptyValue;
        private Dictionary<string, object> valueDictionary;

        /// <summary>
        /// Gets a value indicating whether is array category
        /// </summary>
        public virtual bool ArrayCategory => false;

        /// <summary>
        /// Gets a value indicating whether is explicit category
        /// </summary>
        public virtual bool IsExplicitCategory => false;

        /// <summary>
        /// Gets a value indicating whether is value category
        /// </summary>
        public virtual bool IsValueCategory => false;

        /// <summary>
        /// Gets key
        /// </summary>
        public virtual string Key => null;

        /// <summary>
        /// Gets label
        /// </summary>
        public virtual string Label => null;

        /// <summary>
        /// Gets max number of rows
        /// </summary>
        public virtual int MaxNumberOfRows => -1;

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
        /// Adds to value dictionary
        /// </summary>
        /// <param name="value">Value</param>
        public void AddToValueDictionary(AnalysisCategoryValue value)
        {
            if (this.valueDictionary == null)
            {
                this.valueDictionary = new Dictionary<string, object>();
                this.valueDictionary.Add(value.Key, value);
            }
            else
            {
                this.valueDictionary.SetObjectForKey(value, value.Key);
            }
        }

        /// <summary>
        /// Base field
        /// </summary>
        /// <returns>Returns base field</returns>
        public virtual AnalysisField BaseField()
        {
            return null;
        }

        /// <summary>
        /// Categories for row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Returns categories for row</returns>
        public virtual List<object> CategoriesForRow(ICrmDataSourceRow row)
        {
            AnalysisCategoryValue v = this.CategoryValueForRow(row);
            if (v == null || v.IsEmptyValue)
            {
                return new List<object>();
            }

            return new List<object> { v };
        }

        /// <summary>
        /// Category value for row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Returns category value for row</returns>
        public virtual AnalysisCategoryValue CategoryValueForRow(ICrmDataSourceRow row)
        {
            return null;
        }

        /// <summary>
        /// Empty value
        /// </summary>
        /// <returns>Returns empty value</returns>
        public AnalysisCategoryValue EmptyValue()
        {
            if (this.emptyValue == null)
            {
                this.emptyValue = new AnalysisCategoryValue(this, string.Empty, LocalizedString.TextAnalysesNoValue);
            }

            return this.emptyValue;
        }

        /// <summary>
        /// Key for row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Returns key for row</returns>
        public virtual string KeyForRow(ICrmDataSourceRow row)
        {
            return null;
        }

        /// <summary>
        /// Sorts by first column value
        /// </summary>
        /// <returns>Returns true if successful</returns>
        public virtual bool SortByFirstColumnValue()
        {
            return false;
        }

        /// <summary>
        /// Sorts by key
        /// </summary>
        /// <returns>Returns true if successful</returns>
        public virtual bool SortByKey()
        {
            return false;
        }

        /// <summary>
        /// Value for key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Analysis category value</returns>
        public virtual AnalysisCategoryValue ValueForKey(string key)
        {
            return this.valueDictionary.ValueOrDefault(key) as AnalysisCategoryValue;
        }

        /// <summary>
        /// Value for raw value
        /// </summary>
        /// <param name="rawValue">Raw value</param>
        /// <returns>Value</returns>
        public virtual string ValueForRawValue(string rawValue)
        {
            var cv = this.valueDictionary.ValueOrDefault(rawValue) as AnalysisCategoryValue;
            return cv?.Label;
        }
    }
}
