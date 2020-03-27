// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldAttribute.cs" company="Aurea Software Gmbh">
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
//   Defines the field attribute
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines the field attribute
    /// </summary>
    public class FieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldAttribute"/> class.
        /// </summary>
        /// <param name="attrdef">
        /// The attrdef.
        /// </param>
        public FieldAttribute(List<object> attrdef)
        {
            this.Attrid = JObjectExtensions.ToInt(attrdef[0]);
            this.Editmode = JObjectExtensions.ToInt(attrdef[1]);
            this.Value = attrdef[2] as string;
        }

        /// <summary>
        /// Gets the attrid.
        /// </summary>
        /// <value>
        /// The attrid.
        /// </value>
        public int Attrid { get; private set; }

        /// <summary>
        /// Gets the editmode.
        /// </summary>
        /// <value>
        /// The editmode.
        /// </value>
        public int Editmode { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the value options.
        /// </summary>
        /// <value>
        /// The value options.
        /// </value>
        public Dictionary<string, object> ValueOptions { get; private set; }

        /// <summary>
        /// Values the options for key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ValueOptionsForKey(string key)
        {
            if (this.ValueOptions != null || this.Value == null)
            {
                return this.ValueOptions?.ValueOrDefault(key);
            }

            var extOpt = this.ObjectFromJsonString(this.Value);
            if (extOpt != null && extOpt.Type == JTokenType.Object)
            {
                // Convert to dictionary
                this.ValueOptions = extOpt.ParseObject<Dictionary<string, object>>();
            }
            else
            {
                this.ValueOptions = new Dictionary<string, object>();
            }

            return this.ValueOptions?.ValueOrDefault(key);
        }

        /// <summary>
        /// Objects from json string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="JObject"/>.
        /// </returns>
        private JObject ObjectFromJsonString(string value)
        {
            return !string.IsNullOrWhiteSpace(value) ? JsonConvert.DeserializeObject<JObject>(value) : null;
        }
    }
}
