// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldControl.cs" company="Aurea Software Gmbh">
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
//   The field detail modes
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The field detail modes
    /// </summary>
    [Flags]
    public enum FieldDetailsMode
    {
        /// <summary>
        /// The new.
        /// </summary>
        New = 1,

        /// <summary>
        /// The update.
        /// </summary>
        Update = 2,

        /// <summary>
        /// The delete.
        /// </summary>
        Delete = 4,

        /// <summary>
        /// The view.
        /// </summary>
        View = 8,

        /// <summary>
        /// The search.
        /// </summary>
        Search = 16,

        /// <summary>
        /// The list.
        /// </summary>
        List = 32
    }

    /// <summary>
    /// Configurations related to field control
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class FieldControl : ConfigUnit
    {
        /// <summary>
        /// The sort fields
        /// </summary>
        private List<FieldControlSortField> sortFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControl"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public FieldControl(List<object> definition)
        {
            if (definition == null || definition.Count < 6)
            {
                return;
            }

            this.UnitName = (string)definition[0];
            this.InfoAreaId = (string)definition[1];
            this.ControlName = (string)definition[2];

            var tabdefs = (definition[3] as JArray)?.ToObject<List<object>>();
            if (tabdefs == null)
            {
                return;
            }

            var sortFieldDef = (definition[4] as JArray)?.ToObject<List<object>>();
            var attributeDef = (definition[5] as JArray)?.ToObject<List<object>>();

            // empty minidetails
            int i, count = tabdefs.Count;
            if (count > 0)
            {
                var tabs = new List<FieldControlTab>(count);
                var fieldOffset = 0;
                for (i = 0; i < count; i++)
                {
                    var tabDef = (tabdefs[i] as JArray)?.ToObject<List<object>>();
                    if (tabDef == null)
                    {
                        continue;
                    }

                    var tab = new FieldControlTab(tabDef, fieldOffset, this);
                    tabs.Add(tab);
                    fieldOffset += tab.NumberOfFields;
                }

                this.Tabs = tabs;
            }

            if (sortFieldDef != null)
            {
                count = sortFieldDef.Count;
                var sortFields = new List<FieldControlSortField>(count);
                for (i = 0; i < count; i++)
                {
                    var sdef = (sortFieldDef[i] as JArray)?.ToObject<List<object>>();
                    if (sdef == null)
                    {
                        return;
                    }

                    sortFields.Add(new FieldControlSortField(sdef, this.InfoAreaId));
                }

                this.sortFields = sortFields;
            }

            if (attributeDef != null)
            {
                var attributes = new Dictionary<string, string>();
                foreach (JArray keyval in attributeDef)
                {
                    attributes[keyval[0].ToObject<string>()] = keyval[1]?.ToObject<string>();
                }

                this.Attributes = attributes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControl"/> class.
        /// </summary>
        /// <param name="fieldControlArray">
        /// The field control array.
        /// </param>
        public FieldControl(List<FieldControl> fieldControlArray)
        {
            this.UnitName = "Combined";

            var first = fieldControlArray[0];
            this.InfoAreaId = first.InfoAreaId;
            this.ControlName = first.ControlName;

            this.sortFields = first.SortFields();

            this.Attributes = first.Attributes;
            var tabs = new List<FieldControlTab>();

            var offset = 0;
            foreach (var control in fieldControlArray)
            {
                for (var i = 0; i < control.NumberOfTabs; i++)
                {
                    var currentTab = control.TabAtIndex(i);
                    tabs.Add(new FieldControlTab(currentTab, offset, this, -1));
                    offset += currentTab.NumberOfFields;
                }
            }

            this.Tabs = tabs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControl"/> class.
        /// </summary>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <param name="tabNr">
        /// The tab nr.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="rootLinkId">
        /// The root link identifier.
        /// </param>
        public FieldControl(FieldControl fieldControl, int tabNr, string infoAreaId, int rootLinkId)
        {
            this.UnitName = fieldControl.UnitName;

            this.InfoAreaId = string.IsNullOrWhiteSpace(infoAreaId) ? fieldControl.InfoAreaId : infoAreaId;
            this.ControlName = fieldControl.ControlName;

            this.Tabs = new List<FieldControlTab>
                            {
                                new FieldControlTab(
                                    fieldControl.TabAtIndex(tabNr),
                                    0,
                                    this,
                                    rootLinkId)
                            };

            this.sortFields = fieldControl.SortFields();

            this.Attributes = fieldControl.Attributes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControl"/> class.
        /// </summary>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <param name="tabNr">
        /// The tab nr.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public FieldControl(FieldControl fieldControl, int tabNr, string infoAreaId)
            : this(fieldControl, tabNr, infoAreaId, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControl"/> class.
        /// </summary>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <param name="mode">
        /// The mode.
        /// </param>
        public FieldControl(FieldControl fieldControl, FieldDetailsMode mode)
        {
            this.UnitName = fieldControl.UnitName;
            this.InfoAreaId = fieldControl.InfoAreaId;
            this.ControlName = fieldControl.ControlName;

            var _tabArray = new List<FieldControlTab>(fieldControl.NumberOfTabs);
            _tabArray.AddRange(fieldControl.Tabs.Select(tab => new FieldControlTab(tab, (int)mode, this)));

            this.Tabs = _tabArray;
            this.sortFields = fieldControl.SortFields();

            this.Attributes = fieldControl.Attributes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControl"/> class.
        /// </summary>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <param name="sortConfiguration">
        /// The sort configuration.
        /// </param>
        public FieldControl(FieldControl fieldControl, FieldControl sortConfiguration)
        {
            this.UnitName = "CombinedWithSort";
            this.InfoAreaId = fieldControl.InfoAreaId;
            this.ControlName = fieldControl.ControlName;
            this.Tabs = new List<FieldControlTab>(fieldControl.Tabs);
            this.sortFields = new List<FieldControlSortField>(sortConfiguration.SortFields());
            if (fieldControl?.Attributes != null)
            {
                this.Attributes = new Dictionary<string, string>(fieldControl.Attributes);
            }
            else
            {
                this.Attributes = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Gets all CRM fields.
        /// </summary>
        /// <value>
        /// All CRM fields.
        /// </value>
        public List<UPCRMField> AllCRMFields
        {
            get
            {
                var tabCount = this.Tabs?.Count ?? 0;
                switch (tabCount)
                {
                    case 0:
                        return null;
                    case 1:
                        return this.Tabs?[0].AllCRMFields();
                }

                var fieldArray = new List<UPCRMField>();
                for (var i = 0; i < tabCount; i++)
                {

                    var tabFields = this.Tabs?[i].AllCRMFields();
                    if (tabFields?.Count > 0)
                    {
                        fieldArray.AddRange(tabFields);
                    }
                }

                return fieldArray;
            }
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public Dictionary<string, string> Attributes { get; private set; }

        /// <summary>
        /// Gets the name of the control.
        /// </summary>
        /// <value>
        /// The name of the control.
        /// </value>
        public string ControlName { get; private set; }

        /// <summary>
        /// Gets the CRM sort fields.
        /// </summary>
        /// <value>
        /// The CRM sort fields.
        /// </value>
        public List<UPCRMSortField> CrmSortFields
        {
            get
            {
                if (this.sortFields == null)
                {
                    return null;
                }

                var crmFields = new List<UPCRMSortField>(this.sortFields.Count);
                foreach (var sortField in this.sortFields)
                {
                    crmFields.Add(sortField.CrmSortField);
                }

                return crmFields;
            }
        }

        /// <summary>
        /// Gets the name of the field group.
        /// </summary>
        /// <value>
        /// The name of the field group.
        /// </value>
        public string FieldGroupName
            => !string.IsNullOrWhiteSpace(this.UnitName) ? this.UnitName.Split('.')[0] : string.Empty;

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public List<UPConfigFieldControlField> Fields
        {
            get
            {
                if (this.Tabs == null)
                {
                    return null;
                }

                List<UPConfigFieldControlField> fieldArray = null;
                foreach (var tab in this.Tabs)
                {
                    if (tab.Fields == null)
                    {
                        continue;
                    }

                    if (fieldArray == null)
                    {
                        fieldArray = new List<UPConfigFieldControlField>(tab.Fields);
                    }
                    else
                    {
                        fieldArray.AddRange(tab.Fields);
                    }
                }

                return fieldArray;
            }
        }

        /// <summary>
        /// Gets the fields on first tab.
        /// </summary>
        /// <value>
        /// The fields on first tab.
        /// </value>
        public List<UPConfigFieldControlField> FieldsOnFirstTab => this.Tabs?.FirstOrDefault()?.Fields;

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the number of fields.
        /// </summary>
        /// <value>
        /// The number of fields.
        /// </value>
        public int NumberOfFields
            => this.Tabs?.Aggregate(0, (current, currentTab) => current + currentTab.NumberOfFields) ?? 0;

        /// <summary>
        /// Gets the number of tabs.
        /// </summary>
        /// <value>
        /// The number of tabs.
        /// </value>
        public int NumberOfTabs => this.Tabs?.Count ?? 0;

        /// <summary>
        /// Gets the tabs.
        /// </summary>
        /// <value>
        /// The tabs.
        /// </value>
        public List<FieldControlTab> Tabs { get; private set; }

        /// <summary>
        /// Fields at index.
        /// </summary>
        /// <param name="fieldNr">
        /// The field nr.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFieldControlField"/>.
        /// </returns>
        public UPConfigFieldControlField FieldAtIndex(int fieldNr)
        {
            if (this.Tabs != null)
            {
                foreach (var currentTab in this.Tabs)
                {
                    if (fieldNr >= currentTab.NumberOfFields)
                    {
                        fieldNr -= currentTab.NumberOfFields;
                    }
                    else
                    {
                        return currentTab.FieldAtIndex(fieldNr);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Fields the control with mode.
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <returns>
        /// Fied control instance
        /// </returns>
        public FieldControl FieldControlWithMode(FieldDetailsMode mode)
        {
            return this.NeedsSpecificFieldControlForMode(mode) ? new FieldControl(this, mode) : this;
        }

        /// <summary>
        /// Fields the control with single tab.
        /// </summary>
        /// <param name="tabNr">
        /// The tab nr.
        /// </param>
        /// <returns>
        /// Field control instance
        /// </returns>
        public FieldControl FieldControlWithSingleTab(int tabNr)
        {
            return new FieldControl(this, tabNr, null);
        }

        /// <summary>
        /// Fields the control with single tab and root information area identifier.
        /// </summary>
        /// <param name="tabNr">
        /// The tab nr.
        /// </param>
        /// <param name="rootInfoAreaid">
        /// The root information areaid.
        /// </param>
        /// <returns>
        /// Field control instance
        /// </returns>
        public FieldControl FieldControlWithSingleTab(int tabNr, string rootInfoAreaid)
        {
            return new FieldControl(this, tabNr, rootInfoAreaid);
        }

        /// <summary>
        /// Fields the control with single tab root information area identifier root link identifier.
        /// </summary>
        /// <param name="tabNr">
        /// The tab nr.
        /// </param>
        /// <param name="rootInfoAreaid">
        /// The root information areaid.
        /// </param>
        /// <param name="rootLinkId">
        /// The root link identifier.
        /// </param>
        /// <returns>
        /// Field control instance
        /// </returns>
        public FieldControl FieldControlWithSingleTabRootInfoAreaIdRootLinkId(
            int tabNr,
            string rootInfoAreaid,
            int rootLinkId)
        {
            return new FieldControl(this, tabNr, rootInfoAreaid, rootLinkId);
        }

        /// <summary>
        /// Fields the with function.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <returns>
        /// Field control field configurations
        /// </returns>
        public UPConfigFieldControlField FieldWithFunction(string functionName)
        {
            foreach (var tab in this.Tabs)
            {
                if (tab.Fields == null)
                {
                    continue;
                }

                foreach (var field in tab.Fields)
                {
                    if (field.Function == functionName)
                    {
                        return field;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Fulls the name of the label text for function.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <returns>
        /// Full label text
        /// </returns>
        public string FullLabelTextForFunctionName(string functionName)
        {
            foreach (var tab in this.Tabs)
            {
                if (tab?.Fields == null)
                {
                    continue;
                }

                foreach (var field in tab.Fields)
                {
                    if (field.Function == functionName)
                    {
                        return field.FullLabel;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Functions the names.
        /// </summary>
        /// <returns>Function names vs field control field lookup</returns>
        public Dictionary<string, UPConfigFieldControlField> FunctionNames()
        {
            var mapping = new Dictionary<string, UPConfigFieldControlField>();
            foreach (var tab in this.Tabs)
            {
                if (tab?.Fields == null)
                {
                    continue;
                }

                foreach (var field in tab.Fields)
                {
                    if (!string.IsNullOrEmpty(field.Function))
                    {
                        mapping[field.Function] = field;
                    }
                }
            }

            return mapping;
        }

        /// <summary>
        /// Functions the names.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="fieldOffset">
        /// The field offset.
        /// </param>
        /// <param name="displayPrefix">
        /// The display prefix.
        /// </param>
        /// <returns>
        /// Function names lookup
        /// </returns>
        public Dictionary<string, object> FunctionNames(UPCRMResultRow row, int fieldOffset = 0, string displayPrefix = null)
        {
            if (row == null)
            {
                return null;
            }

            var dictionary = new Dictionary<string, object>();
            foreach (var tab in this.Tabs)
            {
                if (tab?.Fields == null)
                {
                    continue;
                }

                foreach (var field in tab.Fields)
                {
                    if (string.IsNullOrEmpty(field.Function))
                    {
                        continue;
                    }

                    dictionary[field.Function] = row.RawValueAtIndex(field.TabIndependentFieldIndex + fieldOffset);
                    if (!string.IsNullOrWhiteSpace(displayPrefix))
                    {
                        dictionary[$"{displayPrefix}{field.Function}"] =
                            row.ValueAtIndex(field.TabIndependentFieldIndex + fieldOffset);
                    }
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Functions the names.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="displayPrefix">
        /// The display prefix.
        /// </param>
        /// <returns>
        /// Function names lookup
        /// </returns>
        public Dictionary<string, object> FunctionNames(UPCRMResultRow row, string displayPrefix)
        {
            return this.FunctionNames(row, 0, displayPrefix);
        }

        /// <summary>
        /// Labels the name of the text for function.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <returns>
        /// Lavel for given function
        /// </returns>
        public string LabelTextForFunctionName(string functionName)
        {
            foreach (var tab in this.Tabs)
            {
                if (tab?.Fields == null)
                {
                    continue;
                }

                foreach (var field in tab.Fields)
                {
                    if (field.Function.Equals(functionName))
                    {
                        return field.Label;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Needses the specific field control for mode.
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <returns>
        /// true if specific control required; else false
        /// </returns>
        public bool NeedsSpecificFieldControlForMode(FieldDetailsMode mode)
        {
            return this.Tabs != null && this.Tabs.Any(tab => tab.NeedsSpecificFieldControlTabForMode(mode));
        }

        /// <summary>
        /// Results the name of the index of function.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <returns>
        /// Index of a given function name
        /// </returns>
        public int ResultIndexOfFunctionName(string functionName)
        {
            var field = this.FieldWithFunction(functionName);
            if (field == null)
            {
                return -1;
            }

            return field.TabIndependentFieldIndex;
        }

        /// <summary>
        /// Sorts the fields.
        /// </summary>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public List<FieldControlSortField> SortFields()
        {
            if (this.sortFields != null || this.Fields == null || this.Fields.Count == 0)
            {
                return this.sortFields;
            }

            var crmField = this.Fields[0];
            var def = new List<object> { crmField.FieldId, 0, crmField.InfoAreaId };

            var configFieldControlSortField = new FieldControlSortField(def, crmField.InfoAreaId);
            this.sortFields = new List<FieldControlSortField> { configFieldControlSortField };

            return this.sortFields;
        }

        /// <summary>
        /// Tabs at index.
        /// </summary>
        /// <param name="tabNr">
        /// The tab nr.
        /// </param>
        /// <returns>
        /// Field control tab instance
        /// </returns>
        public FieldControlTab TabAtIndex(int tabNr)
        {
            if (this.Tabs != null && this.Tabs.Count > tabNr)
            {
                return this.Tabs[tabNr];
            }

            return null;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder($"FieldControl {this.UnitName} Control {this.ControlName}:");
            foreach (var tab in this.Tabs)
            {
                builder.AppendFormat($"{Environment.NewLine}{tab}");
            }

            if (this.sortFields != null)
            {
                builder.Append($"{Environment.NewLine}Sort: ");
                foreach (var sortField in this.sortFields)
                {
                    builder.AppendFormat($",{sortField}");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Values for attribute.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// attribute value
        /// </returns>
        public string ValueForAttribute(string key)
        {
            return this.Attributes?.ValueOrDefault(key);
        }
    }
}
