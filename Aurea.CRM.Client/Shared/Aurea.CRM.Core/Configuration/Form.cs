// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Form.cs" company="Aurea Software Gmbh">
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
// --------------------------------------------------------------------------------------------------------------------s

namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using Extensions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// defines the form configurations
    /// corresponds to UPConfigForm in CRM.Pad
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class Form : ConfigUnit
    {
        /// <summary>
        /// Gets the tabs.
        /// </summary>
        /// <value>
        /// The tabs.
        /// </value>
        public List<FormTab> Tabs { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class.
        /// </summary>
        /// <param name="defarray">The defarray.</param>
        public Form(List<object> defarray)
        {
            this.UnitName = (string)defarray[0];
            var tabdefs = (defarray[1] as JArray)?.ToObject<List<object>>();
            if (tabdefs == null)
            {
                return;
            }

            // add the tabs
            var count = tabdefs.Count;
            var tabarray = new List<FormTab>(count);
            for (var i = 0; i < count; i++)
            {
                tabarray.Add(new FormTab((tabdefs[i] as JArray)?.ToObject<List<object>>()));
            }

            this.Tabs = tabarray;
        }

        /// <summary>
        /// Gets the number of tabs.
        /// </summary>
        /// <value>
        /// The number of tabs.
        /// </value>
        public int NumberOfTabs => this.Tabs.Count;

        /// <summary>
        /// Tabs at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public FormTab TabAtIndex(int index) => this.Tabs[index];
    }

    /// <summary>
    /// defines the form tab configurations
    /// corresponds to UPConfigFormTab on CRM.Pad
    /// </summary>
    public class FormTab
    {
        /// <summary>
        /// The attributes string
        /// </summary>
        protected string AttributesString;

        /// <summary>
        /// The attributes
        /// </summary>
        private Dictionary<string, object> attributes;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public List<FormRow> Rows { get; }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public Dictionary<string, object> Attributes
        {
            get
            {
                if (this.attributes == null && !string.IsNullOrEmpty(this.AttributesString))
                {
                    var jObject = JsonConvert.DeserializeObject<JObject>(this.AttributesString);
                    this.attributes = jObject?.ParseObject<Dictionary<string, object>>();
                }

                return this.attributes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormTab"/> class.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public FormTab(List<object> definition)
        {
            this.Label = (string)definition[0];
            var rowdefs = (definition[1] as JArray)?.ToObject<List<object>>();
            if (rowdefs == null)
            {
                return;
            }

            var count = rowdefs.Count;
            var rowarray = new List<FormRow>(count);
            for (var i = 0; i < count; i++)
            {
                rowarray.Add(new FormRow((rowdefs[i] as JArray)?.ToObject<List<object>>()));
            }

            this.Rows = rowarray;
            this.AttributesString = (definition.Count > 2) ? (string)definition[2] : null;
        }

        /// <summary>
        /// Attributes for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string AttributeForKey(string key)
        {
            return this.attributes?.ContainsKey(key) == true ? this.Attributes[key] as string : null;
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        /// <value>
        /// The number of rows.
        /// </value>
        public int NumberOfRows => this.Rows.Count;

        /// <summary>
        /// Rows at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public FormRow RowAtIndex(int index) => this.Rows[index];
    }

    /// <summary>
    /// defines form row configurations
    /// corresponds to UPConfigFormRow in CRM.Pad
    /// </summary>
    public class FormRow
    {
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<FormItem> Items { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormRow"/> class.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public FormRow(List<object> definition)
        {
            var itemdefs = (definition[0] as JArray)?.ToObject<List<object>>();
            var count = itemdefs?.Count ?? 0;
            var itemarray = new List<FormItem>(count);
            for (var i = 0; i < count; i++)
            {
                itemarray.Add(new FormItem((itemdefs[i] as JArray)?.ToObject<List<object>>()));
            }

            this.Items = itemarray;
        }

        /// <summary>
        /// Gets the number of items.
        /// </summary>
        /// <value>
        /// The number of items.
        /// </value>
        public int NumberOfItems => this.Items.Count;

        /// <summary>
        /// Items at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public FormItem ItemAtIndex(int index)
        {
            return this.Items[index];
        }
    }

    /// <summary>
    /// defines form item configurations
    /// corresponds to UPConfigFormItem in CRM.Pad
    /// </summary>
    public class FormItem
    {
        /// <summary>
        /// Gets the name of the control.
        /// cell type independent display information
        /// cell type dependent display information
        /// cell type dependent control information
        /// </summary>
        /// <value>
        /// The name of the control.
        /// </value>
        public string ControlName { get; private set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the cell attributes.
        /// </summary>
        /// <value>
        /// The cell attributes.
        /// </value>
        public Dictionary<string, object> CellAttributes { get; private set; }

        /// <summary>
        /// Gets the item attributes.
        /// </summary>
        /// <value>
        /// The item attributes.
        /// </value>
        public Dictionary<string, object> ItemAttributes { get; private set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public Dictionary<string, object> Options { get; private set; }

        /// <summary>
        /// Gets the name of the value.
        /// </summary>
        /// <value>
        /// The name of the value.
        /// </value>
        public string ValueName { get; private set; }

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        public string Func { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormItem"/> class.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public FormItem(List<object> definition)
        {
            this.ControlName = (string)definition[0];
            this.ViewReference = new ViewReference((definition[1] as JArray)?.ToObject<List<object>>(), this.ControlName);
            this.Label = (string)definition[2];
            this.Options = ((string)definition[3]).JsonDictionaryFromString();
            this.CellAttributes = ((string)definition[4]).JsonDictionaryFromString();
            this.ItemAttributes = ((string)definition[5]).JsonDictionaryFromString();

            if (definition.Count > 6)
            {
                this.ValueName = (string)definition[6];
                this.Func = (string)definition[7];
            }
        }
    }
}
