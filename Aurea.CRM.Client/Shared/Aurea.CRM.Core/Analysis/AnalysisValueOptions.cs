// <copyright file="AnalysisValueOptions.cs" company="Aurea Software Gmbh">
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
    using Extensions;

    /// <summary>
    /// Implementation of analysis value options
    /// </summary>
    public class AnalysisValueOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueOptions"/> class.
        /// </summary>
        /// <param name="options">Options</param>
        public AnalysisValueOptions(Dictionary<string, object> options)
        {
            this.IsCategory = (options.ValueOrDefault("IsCategory") as string).ToBoolWithDefaultValue(false);
            this.IsColumn = (options.ValueOrDefault("IsColumn") as string).ToBoolWithDefaultValue(false);
            this.IsDefaultCategory = (options.ValueOrDefault("DefaultCategory") as string).ToBoolWithDefaultValue(false);
            this.IsSortCategory = (options.ValueOrDefault("SortCategory") as string).ToBoolWithDefaultValue(false);
            this.IsStatic = (options.ValueOrDefault("IsStatic") as string).ToBoolWithDefaultValue(false);
            this.IsText = (options.ValueOrDefault("IsText") as string).ToBoolWithDefaultValue(false);
            this.HideIfMany = this.StringValueFromOptionsValue(options.ValueOrDefault("HideIfMany"));
            if (this.IsText)
            {
                this.Concatenate = ", ";
                object concatenateObject = options.ValueOrDefault("Concatenate");
                if (concatenateObject != null)
                {
                    this.Concatenate = this.StringValueFromOptionsValue(concatenateObject);
                }
            }

            this.Cumulate = (options.ValueOrDefault("Cumulate") as string).ToBoolWithDefaultValue(false);
            this.ComputeSum = (options.ValueOrDefault("ComputeSum") as string).ToBoolWithDefaultValue(false);
            var formatObject = options.ValueOrDefault("Format");
            this.Format = null;
            this.FractionDigits = 2;
            if (formatObject != null)
            {
                string str = this.StringValueFromOptionsValueArrayIndex(formatObject, 0);
                if (str?.Length > 0)
                {
                    this.Format = str;
                }

                str = this.StringValueFromOptionsValueArrayIndex(formatObject, 1);
                if (str?.Length > 0)
                {
                    this.FractionDigits = str.ToInt();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether compute sum
        /// </summary>
        public bool ComputeSum { get; private set; }

        /// <summary>
        /// Gets concatenate
        /// </summary>
        public string Concatenate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether cumulate
        /// </summary>
        public bool Cumulate { get; private set; }

        /// <summary>
        /// Gets format
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// Gets fraction digits
        /// </summary>
        public int FractionDigits { get; private set; }

        /// <summary>
        /// Gets hide if many
        /// </summary>
        public string HideIfMany { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is category
        /// </summary>
        public bool IsCategory { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is column
        /// </summary>
        public bool IsColumn { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is default category
        /// </summary>
        public bool IsDefaultCategory { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is sort category
        /// </summary>
        public bool IsSortCategory { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is static
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is text
        /// </summary>
        public bool IsText { get; private set; }

        private string StringValueFromOptionsValue(object optionsValue)
        {
            return this.StringValueFromOptionsValueArrayIndex(optionsValue, 0);
        }

        private string StringValueFromOptionsValueArrayIndex(object optionsValue, int arrayIndex)
        {
            if (optionsValue != null)
            {
                object stringValue = null;
                if (optionsValue is List<object>)
                {
                    var arr = (List<object>)optionsValue;
                    stringValue = arr.Count > arrayIndex ? arr[arrayIndex] : null;
                }
                else if (arrayIndex == 0)
                {
                    stringValue = optionsValue;
                }
                else
                {
                    stringValue = null;
                }

                if (stringValue is string)
                {
                    return (string)stringValue;
                }
                else if (stringValue != null && stringValue.HasMethod((string)stringValue))
                {
                    return stringValue.ToString();
                }
            }

            return string.Empty;
        }
    }
}
