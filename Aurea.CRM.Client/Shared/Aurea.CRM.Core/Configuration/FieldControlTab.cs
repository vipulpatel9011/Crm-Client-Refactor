// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldControlTab.cs" company="Aurea Software Gmbh">
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
//   Field control tab configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Field control tab configurations
    /// </summary>
    public class FieldControlTab
    {
        /// <summary>
        /// The attribute dictionary.
        /// </summary>
        private readonly Dictionary<string, List<ConfigFieldControlTabAttribute>> attributeDictionary;

        /// <summary>
        /// The attributes.
        /// </summary>
        private readonly List<object> attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControlTab"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        public FieldControlTab(List<object> definition, int offset, FieldControl fieldControl)
        {
            this.FieldControl = fieldControl;
            this.Label = (string)definition[0];
            this.Type = (string)definition[1];
            var fielddefs = (definition[2] as JArray)?.ToObject<List<object>>();
            if (fielddefs == null)
            {
                this.AllFields = null;
            }
            else
            {
                var count = fielddefs.Count;
                this.AllFields = new List<UPConfigFieldControlField>(count);
                for (var i = 0; i < count; i++)
                {
                    var def = (fielddefs[i] as JArray)?.ToObject<List<object>>();
                    if (def == null)
                    {
                        continue;
                    }

                    this.AllFields.Add(new UPConfigFieldControlField(def, offset + i, this));
                }
            }

            this.FieldOffset = offset;
            if (definition.Count > 3)
            {
                var attributeDefs = definition[3] as List<List<object>>;
                if (attributeDefs != null && attributeDefs.Count > 0)
                {
                    var attributes = new List<object>(attributeDefs.Count);
                    var attributeDictionary =
                        new Dictionary<string, List<ConfigFieldControlTabAttribute>>(attributeDefs.Count);
                    foreach (var attributeDef in attributeDefs)
                    {
                        var attribute = new ConfigFieldControlTabAttribute(attributeDef);
                        attributes.Add(attribute);
                        var arrayForKey = attributeDictionary[attribute.Key];
                        if (arrayForKey != null)
                        {
                            arrayForKey.Add(attribute);
                            attributeDictionary[attribute.Key] = arrayForKey;
                        }
                        else
                        {
                            attributeDictionary[attribute.Key] = new List<ConfigFieldControlTabAttribute> { attribute };
                        }
                    }

                    this.attributeDictionary = attributeDictionary;
                    this.attributes = attributes;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControlTab"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <param name="rootLinkId">
        /// The root link identifier.
        /// </param>
        public FieldControlTab(FieldControlTab source, int offset, FieldControl fieldControl, int rootLinkId)
        {
            this.FieldControl = fieldControl;
            this.Label = source?.Label;
            this.Type = source?.Type;
            var count = source?.NumberOfFields ?? 0;
            if (count > 0)
            {
                this.AllFields = new List<UPConfigFieldControlField>(count);
                for (var i = 0; i < count; i++)
                {
                    this.AllFields.Add(
                        new UPConfigFieldControlField(
                            source.FieldAtIndex(i),
                            offset + i,
                            this,
                            fieldControl.InfoAreaId,
                            rootLinkId));
                }
            }

            this.attributes = source?.AttributeArray;
            this.attributeDictionary = source?.AttributeDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControlTab"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        public FieldControlTab(FieldControlTab source, int mode, FieldControl fieldControl)
        {
            this.FieldControl = fieldControl;

            this.Label = source?.Label;
            this.Type = source?.Type;

            if (source != null && source.NumberOfFields > 0)
            {
                var fieldArray = new List<UPConfigFieldControlField>(source.NumberOfFields);
                fieldArray.AddRange(source.Fields.Select(field => new UPConfigFieldControlField(field, mode, this)));
                this.AllFields = fieldArray;
            }
            else
            {
                this.AllFields = new List<UPConfigFieldControlField>();
            }
        }

        /// <summary>
        /// Gets all fields.
        /// </summary>
        /// <value>
        /// All fields.
        /// </value>
        public List<UPConfigFieldControlField> AllFields { get; private set; }

        /// <summary>
        /// Gets the attribute array.
        /// </summary>
        /// <value>
        /// The attribute array.
        /// </value>
        public List<object> AttributeArray => this.attributes != null ? new List<object>(this.attributes) : null;

        /// <summary>
        /// Gets the attribute dictionary.
        /// </summary>
        /// <value>
        /// The attribute dictionary.
        /// </value>
        public Dictionary<string, List<ConfigFieldControlTabAttribute>> AttributeDictionary
            =>
                this.attributeDictionary != null
                    ? new Dictionary<string, List<ConfigFieldControlTabAttribute>>(this.attributeDictionary)
                    : null;

        /// <summary>
        /// Gets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; private set; }

        /* editModes: New=1, Update=2, Delete=4, View=8, Search=16, List=32 */

        /// <summary>
        /// Gets the field offset.
        /// </summary>
        /// <value>
        /// The field offset.
        /// </value>
        public int FieldOffset { get; private set; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public List<UPConfigFieldControlField> Fields => this.AllFields; // { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the number of fields.
        /// </summary>
        /// <value>
        /// The number of fields.
        /// </value>
        public int NumberOfFields => this.AllFields?.Count ?? 0;

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; private set; }

        /// <summary>
        /// Alls the CRM fields.
        /// </summary>
        /// <returns>list of crm fields</returns>
        public List<UPCRMField> AllCRMFields()
        {
            if (this.AllFields == null)
            {
                return null;
            }

            var count = this.AllFields.Count;
            var crmFields = new List<UPCRMField>(count);
            for (var i = 0; i < count; i++)
            {
                crmFields.Add(this.AllFields[i].Field);
            }

            return crmFields;
        }

        /// <summary>
        /// Fields at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFieldControlField"/>.
        /// </returns>
        public UPConfigFieldControlField FieldAtIndex(int index)
        {
            if (this.AllFields != null && this.AllFields.Count > index)
            {
                return (UPConfigFieldControlField)this.AllFields[index];
            }

            return null;
        }

        /// <summary>
        /// Needses the specific field control tab for mode.
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <returns>
        /// true if need secific control
        /// </returns>
        public bool NeedsSpecificFieldControlTabForMode(FieldDetailsMode mode)
        {
            if (this.AllFields == null)
            {
                return false;
            }

            foreach (var field in this.AllFields)
            {
                if (field.NeedsSpecificFieldControlFieldForMode(mode))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder("Tab: ");
            foreach (var field in this.AllFields)
            {
                builder.AppendFormat($",{field}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Values for attribute.
        /// </summary>
        /// <param name="attributeKey">
        /// The attribute key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueForAttribute(string attributeKey)
        {
            if (this.attributeDictionary == null)
            {
                return string.Empty;
            }

            ConfigFieldControlTabAttribute tabAttribute = null;
            foreach (var attr in this.attributeDictionary.ValueOrDefault(attributeKey))
            {
                if (tabAttribute == null || attr.EditMode > tabAttribute.EditMode)
                {
                    tabAttribute = attr;
                }
            }

            return tabAttribute != null ? tabAttribute.Value : string.Empty;
        }

        /* editModes: New=1, Update=2, Delete=4, View=8, Search=16, List=32 */

        /// <summary>
        /// Values for attribute.
        /// </summary>
        /// <param name="attributeKey">
        /// The attribute key.
        /// </param>
        /// <param name="editMode">
        /// The edit mode.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueForAttribute(string attributeKey, int editMode)
        {
            ConfigFieldControlTabAttribute tabAttribute = null;
            foreach (var attr in this.attributeDictionary[attributeKey])
            {
                if ((attr.EditMode & editMode) != editMode)
                {
                    continue;
                }

                if (tabAttribute == null || attr.EditMode < tabAttribute.EditMode)
                {
                    tabAttribute = attr;
                }
            }

            return tabAttribute != null ? tabAttribute.Value : string.Empty;
        }
    }

    /// <summary>
    /// Field control tab attributes
    /// </summary>
    public class ConfigFieldControlTabAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigFieldControlTabAttribute"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public ConfigFieldControlTabAttribute(List<object> definition)
        {
            this.Key = definition[0] as string;
            this.Value = definition[1] as string;
            this.EditMode = JObjectExtensions.ToInt(definition[2]);
            this.ValueType = definition[3] as string;
        }

        /// <summary>
        /// Gets the edit mode.
        /// </summary>
        /// <value>
        /// The edit mode.
        /// </value>
        public int EditMode { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        public string ValueType { get; private set; }
    }
}
