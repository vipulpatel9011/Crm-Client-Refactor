// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigAnalysisCategory.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Configurations related to analysis category
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Configurations related to analysis category
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigAnalysisCategory : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigAnalysisCategory"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public UPConfigAnalysisCategory(List<object> definition)
        {
            this.Name = definition[0] as string;
            this.MultiValue = JObjectExtensions.ToInt(definition[1]);
            this.OtherMode = JObjectExtensions.ToInt(definition[2]);
            this.Roll = definition[3] as string;
            this.Label = definition[4] as string;
            this.OtherLabel = definition[5] as string;
            if (string.IsNullOrEmpty(this.Label))
            {
                this.Label = this.Name;
            }

            var valueDefs = (definition[6] as JArray)?.ToObject<List<object>>();
            if (valueDefs == null || !valueDefs.Any())
            {
                return;
            }

            var values = new List<UPConfigAnalysisCategoryValue>(valueDefs.Count);
            foreach (JArray valueDef in valueDefs)
            {
                var value = new UPConfigAnalysisCategoryValue(valueDef?.ToObject<List<object>>());
                values.Add(value);
            }

            this.Values = values;
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the multi value.
        /// </summary>
        /// <value>
        /// The multi value.
        /// </value>
        public int MultiValue { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the other label.
        /// </summary>
        /// <value>
        /// The other label.
        /// </value>
        public string OtherLabel { get; private set; }

        /// <summary>
        /// Gets the other mode.
        /// </summary>
        /// <value>
        /// The other mode.
        /// </value>
        public int OtherMode { get; private set; }

        /// <summary>
        /// Gets the roll.
        /// </summary>
        /// <value>
        /// The roll.
        /// </value>
        public string Roll { get; private set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<UPConfigAnalysisCategoryValue> Values { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Category={this.Name}, values={this.Values}";
        }
    }

    /// <summary>
    /// Analysis category value
    /// </summary>
    public class UPConfigAnalysisCategoryValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigAnalysisCategoryValue"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public UPConfigAnalysisCategoryValue(List<object> definition)
        {
            this.ValueNumber = JObjectExtensions.ToInt(definition[0]);
            this.RefValue = definition[1] as string;
            this.SubCategoryName = definition[2] as string;
            this.Label = definition[3] as string;

            var conditionDefs = (definition[4] as JArray)?.ToObject<List<object>>();
            if (conditionDefs != null && conditionDefs.Any())
            {
                var _conditions = new List<UPConfigAnalysisCategoryCondition>(conditionDefs.Count);
                foreach (JArray conditionDef in conditionDefs)
                {
                    if (conditionDef == null)
                    {
                        continue;
                    }

                    var condition = new UPConfigAnalysisCategoryCondition(conditionDef?.ToObject<List<object>>());
                    _conditions.Add(condition);
                }

                this.Conditions = _conditions;
            }

            if (string.IsNullOrEmpty(this.Label))
            {
                this.Label = this.RefValue;
            }
        }

        /// <summary>
        /// Gets the conditions.
        /// </summary>
        /// <value>
        /// The conditions.
        /// </value>
        public List<UPConfigAnalysisCategoryCondition> Conditions { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the reference value.
        /// </summary>
        /// <value>
        /// The reference value.
        /// </value>
        public string RefValue { get; private set; }

        /// <summary>
        /// Gets the name of the sub category.
        /// </summary>
        /// <value>
        /// The name of the sub category.
        /// </value>
        public string SubCategoryName { get; private set; }

        /// <summary>
        /// Gets the value number.
        /// </summary>
        /// <value>
        /// The value number.
        /// </value>
        public int ValueNumber { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"name={this.RefValue}, subCat={this.SubCategoryName}, cond={this.Conditions}";
        }
    }

    /// <summary>
    /// Analysis category condition
    /// </summary>
    public class UPConfigAnalysisCategoryCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigAnalysisCategoryCondition"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public UPConfigAnalysisCategoryCondition(List<object> definition)
        {
            this.Type = definition[1] as string;
            this.Value = definition[2] as string;
            this.ValueTo = definition[3] as string;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the value to.
        /// </summary>
        /// <value>
        /// The value to.
        /// </value>
        public string ValueTo { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.ValueTo)
                       ? $"value {this.Type} [{this.Value},{this.ValueTo}]"
                       : $"value {this.Type} {this.Value}";
        }
    }
}
