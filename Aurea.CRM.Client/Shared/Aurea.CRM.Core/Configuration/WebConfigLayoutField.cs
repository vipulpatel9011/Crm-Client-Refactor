// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebConfigLayoutField.cs" company="Aurea Software Gmbh">
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
//   defines the Web configuration layout fields
//   Coresponds to the UPConfigWebConfigLayoutField CRM.Pad implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// defines the Web configuration layout fields
    /// Coresponds to the UPConfigWebConfigLayoutField CRM.Pad implementation
    /// </summary>
    public class WebConfigLayoutField
    {
        /// <summary>
        /// The option dictionary
        /// </summary>
        private Dictionary<string, string> optionDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebConfigLayoutField"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public WebConfigLayoutField(List<object> definition)
        {
            this.ValueName = (string)definition[0];
            this.Label = (string)definition[1];
            if (string.IsNullOrEmpty(this.Label))
            {
                this.Label = this.ValueName;
            }

            this.FieldType = (string)definition[2];

            List<object> optionArrayDef = null;
            if (definition.Count() > 3)
            {
                optionArrayDef = definition[3] as List<object>;
            }

            if (optionArrayDef?.Count > 0)
            {
                var optionArray = new List<WebConfigOption>(optionArrayDef.Count);
                foreach (List<object> optionDef in optionArrayDef)
                {
                    optionArray.Add(new WebConfigOption(optionDef));
                }

                this.Options = optionArray;
            }
            else
            {
                this.Options = null;
            }
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public string FieldType { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the option dictionary.
        /// </summary>
        /// <value>
        /// The option dictionary.
        /// </value>
        public Dictionary<string, string> OptionDictionary
        {
            get
            {
                if (this.optionDictionary != null || this.Options.Count == 0)
                {
                    return this.optionDictionary;
                }

                var dict = new Dictionary<string, string>(this.Options.Count);
                foreach (var option in this.Options)
                {
                    dict[option.Value] = option.Label;
                }

                this.optionDictionary = dict;
                return this.optionDictionary;
            }
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public List<WebConfigOption> Options { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value => ConfigurationUnitStore.DefaultStore.ConfigValue(this.ValueName);

        /// <summary>
        /// Gets the name of the value.
        /// </summary>
        /// <value>
        /// The name of the value.
        /// </value>
        public string ValueName { get; private set; }

        /// <summary>
        /// Displays the value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DisplayValue()
        {
            return this.DisplayValueForValue(this.Value);
        }

        /// <summary>
        /// Displays the value for value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DisplayValueForValue(string value)
        {
            if (this.Options != null)
            {
                foreach (var option in this.Options)
                {
                    if (string.IsNullOrEmpty(option?.Value))
                    {
                        continue;
                    }

                    if (option.Value.Equals(value))
                    {
                        return option.Label;
                    }
                }

                return $"{value} ???";
            }

            if (this.FieldType == "Checkbox")
            {
                return string.IsNullOrEmpty(value) || value == "false" || value == "0"
                           ? Core.Constants.UpTextNo
                           : Core.Constants.UpTextYes;
            }

            return value;
        }
    }

    /// <summary>
    /// defines the web configuration options
    /// corresponds to UPConfigWebConfigOption CRM.Pad implementation
    /// </summary>
    public class WebConfigOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebConfigOption"/> class.
        /// </summary>
        /// <param name="value">
        /// The _value.
        /// </param>
        /// <param name="label">
        /// The _label.
        /// </param>
        public WebConfigOption(string value, string label)
        {
            this.Value = value;
            this.Label = string.IsNullOrEmpty(label) ? this.Value : label;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebConfigOption"/> class.
        /// </summary>
        /// <param name="def">
        /// The definition.
        /// </param>
        public WebConfigOption(List<object> def)
            : this(def[0] as string, def[1] as string)
        {
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
