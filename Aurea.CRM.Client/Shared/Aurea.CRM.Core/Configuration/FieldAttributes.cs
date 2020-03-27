// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldAttributes.cs" company="Aurea Software Gmbh">
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
//   The field attr.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Ioc;
    using GalaSoft.MvvmLight.Views;

    /// <summary>
    /// The field attr.
    /// </summary>
    public enum FieldAttr
    {
        /// <summary>
        /// The read only.
        /// </summary>
        ReadOnly = 1,

        /// <summary>
        /// The bold.
        /// </summary>
        Bold = 2,

        /// <summary>
        /// The italic.
        /// </summary>
        Italic = 3,

        /// <summary>
        /// The hyperlink.
        /// </summary>
        Hyperlink = 5,

        /// <summary>
        /// The mail.
        /// </summary>
        Mail = 6,

        /// <summary>
        /// The dont save.
        /// </summary>
        DontSave = 7,

        /// <summary>
        /// The hide.
        /// </summary>
        Hide = 10,

        /// <summary>
        /// The color.
        /// </summary>
        Color = 11,

        /// <summary>
        /// The password.
        /// </summary>
        Password = 12,

        /// <summary>
        /// The must.
        /// </summary>
        Must = 13,

        /// <summary>
        /// The no multi line.
        /// </summary>
        NoMultiLine = 26,

        /// <summary>
        /// The label bold.
        /// </summary>
        LabelBold = 28,

        /// <summary>
        /// The label italic.
        /// </summary>
        LabelItalic = 29,

        /// <summary>
        /// The label color.
        /// </summary>
        LabelColor = 31,

        /// <summary>
        /// The phone.
        /// </summary>
        Phone = 33,

        /// <summary>
        /// The no label.
        /// </summary>
        NoLabel = 18,

        /// <summary>
        /// The render hook.
        /// </summary>
        RenderHook = 62,

        /// <summary>
        /// The options.
        /// </summary>
        Options = 60,

        /// <summary>
        /// The col span.
        /// </summary>
        ColSpan = 40,

        /// <summary>
        /// The row span.
        /// </summary>
        RowSpan = 81,

        /// <summary>
        /// The multi line.
        /// </summary>
        MultiLine = 25,

        /// <summary>
        /// The image.
        /// </summary>
        Image = 53,

        /// <summary>
        /// The record select.
        /// </summary>
        RecordSelect = 32,

        /// <summary>
        /// The place holder.
        /// </summary>
        PlaceHolder = 52,

        /// <summary>
        /// The extended options.
        /// </summary>
        ExtendedOptions = 60,

        /// <summary>
        /// The field style.
        /// </summary>
        FieldStyle = 47,

        /// <summary>
        /// The empty.
        /// </summary>
        Empty = 19,

        /// <summary>
        /// The dont cache offline.
        /// </summary>
        DontCacheOffline = 72,
    }

    /// <summary>
    /// Field attributes
    /// </summary>
    public class FieldAttributes
    {
        /// <summary>
        /// The attributes.
        /// </summary>
        protected Dictionary<int, FieldAttribute> attributes;

        /// <summary>
        /// The field count.
        /// </summary>
        private int fieldCount;

        /// <summary>
        /// The line field counts.
        /// </summary>
        protected List<int> lineFieldCounts;

        /// <summary>
        /// The combine string.
        /// </summary>
        private string combineString;

        /// <summary>
        /// The extended options.
        /// </summary>
        private Dictionary<string, string> extendedOptions;

        /// <summary>
        /// The render hooks.
        /// </summary>
        private Dictionary<string, string> renderHooks;

        /// <summary>
        /// Gets the extended options.
        /// </summary>
        /// <value>
        /// The extended options.
        /// </value>
        public Dictionary<string, string> ExtendedOptions
            => this.extendedOptions ?? (this.extendedOptions = this.JsonForAttribute((int)FieldAttr.ExtendedOptions) as Dictionary<string, string>);

        /// <summary>
        /// Gets the render hooks options.
        /// </summary>
        /// <value>
        /// The render hooks.
        /// </value>
        public Dictionary<string, string> RenderHooks
            => this.renderHooks ?? (this.renderHooks = this.JsonForAttribute((int)FieldAttr.RenderHook) as Dictionary<string, string>);

        /// <summary>
        /// Gets a value indicating whether [read only].
        /// </summary>
        /// <value>
        /// <c>true</c> if [read only]; otherwise, <c>false</c>.
        /// </value>
        public bool ReadOnly => this.AttributeIsSet((int)FieldAttr.ReadOnly);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is bold.
        /// </summary>
        /// <value>
        /// <c>true</c> if bold; otherwise, <c>false</c>.
        /// </value>
        public bool Bold => this.AttributeIsSet((int)FieldAttr.Bold);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is italic.
        /// </summary>
        /// <value>
        /// <c>true</c> if italic; otherwise, <c>false</c>.
        /// </value>
        public bool Italic => this.AttributeIsSet((int)FieldAttr.Italic);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is httplink.
        /// </summary>
        /// <value>
        /// <c>true</c> if httplink; otherwise, <c>false</c>.
        /// </value>
        public bool Httplink => this.AttributeIsSet((int)FieldAttr.Hyperlink);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is email.
        /// </summary>
        /// <value>
        /// <c>true</c> if email; otherwise, <c>false</c>.
        /// </value>
        public bool Email => this.AttributeIsSet((int)FieldAttr.Mail);

        /// <summary>
        /// Gets a value indicating whether [dontcache offline].
        /// </summary>
        /// <value>
        /// <c>true</c> if [dontcache offline]; otherwise, <c>false</c>.
        /// </value>
        public bool DontcacheOffline => this.AttributeIsSet((int)FieldAttr.DontCacheOffline);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is dontsave.
        /// </summary>
        /// <value>
        /// <c>true</c> if dontsave; otherwise, <c>false</c>.
        /// </value>
        public bool Dontsave => this.AttributeIsSet((int)FieldAttr.DontSave);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is hide.
        /// </summary>
        /// <value>
        /// <c>true</c> if hide; otherwise, <c>false</c>.
        /// </value>
        public bool Hide => this.AttributeIsSet((int)FieldAttr.Hide);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is password.
        /// </summary>
        /// <value>
        /// <c>true</c> if password; otherwise, <c>false</c>.
        /// </value>
        public bool Password => this.AttributeIsSet((int)FieldAttr.Password);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is must.
        /// </summary>
        /// <value>
        /// <c>true</c> if must; otherwise, <c>false</c>.
        /// </value>
        public bool Must => this.AttributeIsSet((int)FieldAttr.Must);

        /// <summary>
        /// Gets a value indicating whether [label bold].
        /// </summary>
        /// <value>
        /// <c>true</c> if [label bold]; otherwise, <c>false</c>.
        /// </value>
        public bool LabelBold => this.AttributeIsSet((int)FieldAttr.LabelBold);

        /// <summary>
        /// Gets a value indicating whether [label italic].
        /// </summary>
        /// <value>
        /// <c>true</c> if [label italic]; otherwise, <c>false</c>.
        /// </value>
        public bool LabelItalic => this.AttributeIsSet((int)FieldAttr.LabelItalic);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is phone.
        /// </summary>
        /// <value>
        /// <c>true</c> if phone; otherwise, <c>false</c>.
        /// </value>
        public bool Phone => this.AttributeIsSet((int)FieldAttr.Phone);

        /// <summary>
        /// Gets a value indicating whether [no label].
        /// </summary>
        /// <value>
        /// <c>true</c> if [no label]; otherwise, <c>false</c>.
        /// </value>
        public bool NoLabel => this.AttributeIsSet((int)FieldAttr.NoLabel);

        /// <summary>
        /// Gets a value indicating whether [multi line].
        /// </summary>
        /// <value>
        /// <c>true</c> if [multi line]; otherwise, <c>false</c>.
        /// </value>
        public bool MultiLine => this.AttributeIsSet((int)FieldAttr.MultiLine);

        /// <summary>
        /// Gets a value indicating whether [no multi line].
        /// </summary>
        /// <value>
        /// <c>true</c> if [no multi line]; otherwise, <c>false</c>.
        /// </value>
        public bool NoMultiLine => this.AttributeIsSet((int)FieldAttr.NoMultiLine);

        /// <summary>
        /// Gets the height of the multi line.
        /// </summary>
        /// <value>
        /// The height of the multi line.
        /// </value>
        public int MultiLineHeight
        {
            get
            {
                var mlc = int.Parse(this.ValueForAttribute((int)FieldAttr.MultiLine));
                return mlc > 1 ? mlc : 5;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is image.
        /// </summary>
        /// <value>
        /// <c>true</c> if image; otherwise, <c>false</c>.
        /// </value>
        public bool Image => this.AttributeIsSet((int)FieldAttr.Image);

        /// <summary>
        /// Gets the selector.
        /// </summary>
        /// <value>
        /// The selector.
        /// </value>
        public Dictionary<string, object> Selector
        {
            get
            {
                var fieldValue = this.ValueForAttribute((int)FieldAttr.RecordSelect);
                if (string.IsNullOrWhiteSpace(fieldValue))
                {
                    return null;
                }

                return fieldValue.StartsWith("{")
                           ? fieldValue.JsonDictionaryFromString()
                           : new Dictionary<string, object> { { "value", fieldValue } };
            }
        }

        /// <summary>
        /// Gets a value indicating whether [place holder].
        /// </summary>
        /// <value>
        /// <c>true</c> if [place holder]; otherwise, <c>false</c>.
        /// </value>
        public bool PlaceHolder => this.AttributeIsSet((int)FieldAttr.PlaceHolder);

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldAttributes"/> is empty.
        /// </summary>
        /// <value>
        /// <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public bool Empty => this.AttributeIsSet((int)FieldAttr.Empty);

        /// <summary>
        /// Gets the field style.
        /// </summary>
        /// <value>
        /// The field style.
        /// </value>
        public string FieldStyle => this.ValueForAttribute((int)FieldAttr.FieldStyle);

        /// <summary>
        /// Gets the combine string.
        /// </summary>
        /// <value>
        /// The combine string.
        /// </value>
        public string CombineString => this.combineString;

        /// <summary>
        /// Gets the color of the label.
        /// </summary>
        /// <value>
        /// The color of the label.
        /// </value>
        public AureaColor LabelColor
        {
            get
            {
                var value = this.ValueForAttribute((int)FieldAttr.LabelColor);
                return string.IsNullOrWhiteSpace(value) ? null : AureaColor.ColorWithString(value);
            }
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public AureaColor Color
        {
            get
            {
                var value = this.ValueForAttribute((int)FieldAttr.Color);
                return string.IsNullOrWhiteSpace(value) ? null : AureaColor.ColorWithString(value);
            }
        }

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>
        /// The field count.
        /// </value>
        public int FieldCount => this.fieldCount;

        /// <summary>
        /// Gets the line count.
        /// </summary>
        /// <value>
        /// The line count.
        /// </value>
        public int LineCount => this.lineFieldCounts?.Count ?? 1;

        /// <summary>
        /// Gets the attribute array.
        /// </summary>
        /// <value>
        /// The attribute array.
        /// </value>
        public List<FieldAttribute> AttributeArray { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [no place holders in combine string].
        /// </summary>
        /// <value>
        /// <c>true</c> if [no place holders in combine string]; otherwise, <c>false</c>.
        /// </value>
        public bool NoPlaceHoldersInCombineString { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [combine with indices].
        /// </summary>
        /// <value>
        /// <c>true</c> if [combine with indices]; otherwise, <c>false</c>.
        /// </value>
        public bool CombineWithIndices { get; private set; }

        /// <summary>
        /// Values for attribute.
        /// </summary>
        /// <param name="attrid">
        /// The attrid.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ValueForAttribute(int attrid)
        {
            var attr = this.attributes?.ValueOrDefault(attrid);
            return attr?.Value;
        }

        /// <summary>
        /// Strings from.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="nr">
        /// The nr.
        /// </param>
        /// <param name="replaceString">
        /// The replace string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string StringFrom(string source, int nr, string replaceString)
        {
            var pattern = $"{{{nr + 1}}}";
            if (source.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) > -1)
            {
                return source.Replace(pattern, replaceString);
            }

            pattern = $"%%{nr + 1}";
            return source.Replace(pattern, replaceString);
        }

        /// <summary>
        /// Parses the colspan.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void ParseColspan(string value)
        {
            try
            {
                const char separator = ':';

                var range = value.IndexOf(separator.ToString(), StringComparison.CurrentCulture);
                this.CombineWithIndices = false;

                if (range > -1)
                {
                    var strLineParts = value.Split(separator);

                    if (strLineParts.Length > 3)
                    {
                        if (strLineParts[1] == "Text")
                        {
                            this.combineString = LocalizedString.Localize(
                                strLineParts[2],
                                int.Parse(strLineParts[3]),
                                ConfigurationUnitStore.DefaultStore);
                            this.NoPlaceHoldersInCombineString = true;
                        }
                        else if (strLineParts[1] == "FormatText")
                        {
                            this.combineString = LocalizedString.Localize(
                                strLineParts[2],
                                int.Parse(strLineParts[3]),
                                ConfigurationUnitStore.DefaultStore);
                            this.CombineWithIndices = true;
                        }
                    }

                    if (string.IsNullOrEmpty(this.combineString))
                    {
                        this.combineString = strLineParts[1];
                        this.combineString = this.combineString.Replace("b", " ");
                        this.combineString = this.combineString.Replace("c", separator.ToString());
                    }

                    value = strLineParts[0];
                }
                else
                {
                    this.combineString = " ";
                }

                range = value.IndexOf(";", StringComparison.Ordinal);

                if (range > -1)
                {
                    var strLineFieldCounts = value.Split(';');
                    var lineCount = strLineFieldCounts.Length;
                    var lfc = new List<int>(lineCount);
                    this.fieldCount = 0;

                    for (var j = 0; j < lineCount; j++)
                    {
                        var intValue = int.Parse(strLineFieldCounts[j]);
                        this.fieldCount += intValue;
                        lfc.Add(intValue);
                    }

                    this.lineFieldCounts = lfc;
                }
                else
                {
                    this.fieldCount = int.Parse(value);
                }
            }
            catch (Exception error)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError(error);
            }
        }

        /// <summary>
        /// Fills the attribute dictionary.
        /// </summary>
        private void FillAttributeDictionary()
        {
            var attrdict = new Dictionary<int, FieldAttribute>(this.AttributeArray.Count);
            foreach (var fieldAttribute in this.AttributeArray)
            {
                attrdict[fieldAttribute.Attrid] = fieldAttribute;
            }

            this.attributes = attrdict;
            var value = this.ValueForAttribute((int)FieldAttr.ColSpan);
            if (!string.IsNullOrWhiteSpace(value))
            {
                this.ParseColspan(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldAttributes"/> class.
        /// </summary>
        /// <param name="fieldattributedef">
        /// The fieldattributedef.
        /// </param>
        public FieldAttributes(List<object> fieldattributedef)
        {
            if (fieldattributedef == null)
            {
                return;
            }

            var count = fieldattributedef.Count;
            this.fieldCount = 1;
            if (count > 0)
            {
                var array = new List<FieldAttribute>(fieldattributedef.Count);
                for (var i = 0; i < count; i++)
                {
                    var attrDef = (fieldattributedef[i] as JArray)?.ToObject<List<object>>();
                    if (attrDef == null)
                    {
                        continue;
                    }

                    var attr = new FieldAttribute(attrDef);
                    array.Add(attr);
                }

                this.AttributeArray = array;
                this.FillAttributeDictionary();
            }
            else
            {
                this.AttributeArray = null;
                this.attributes = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldAttributes"/> class.
        /// </summary>
        /// <param name="fieldAttributes">
        /// The field attributes.
        /// </param>
        /// <param name="mode">
        /// The mode.
        /// </param>
        public FieldAttributes(FieldAttributes fieldAttributes, int mode)
        {
            var attributeArray = new List<FieldAttribute>(this.AttributeArray?.Count ?? 4);
            foreach (var fieldAttribute in fieldAttributes?.AttributeArray ?? new List<FieldAttribute>())
            {
                if ((fieldAttribute.Editmode & mode) != 0)
                {
                    attributeArray.Add(fieldAttribute);
                }
            }

            this.AttributeArray = attributeArray;
            this.FillAttributeDictionary();
        }

        /// <summary>
        /// Attributes the is set.
        /// </summary>
        /// <param name="attrid">
        /// The attrid.
        /// </param>
        /// <returns>
        /// true if attribute is set
        /// </returns>
        private bool AttributeIsSet(int attrid)
        {
            return this.attributes?.ValueOrDefault(attrid) != null;
        }

        /// <summary>
        /// Attributs for identifier.
        /// </summary>
        /// <param name="attrid">
        /// The attrid.
        /// </param>
        /// <returns>
        /// field attribute for the given id
        /// </returns>
        public FieldAttribute AttributForId(int attrid)
        {
            return this.attributes.ValueOrDefault(attrid);
        }

        /// <summary>
        /// Objects from json string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// deserialized object
        /// </returns>
        private Dictionary<string, string> ObjectFromJsonString(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
            }
            catch (Exception error)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError(error);
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Jsons for attribute.
        /// </summary>
        /// <param name="attr">
        /// The attribute.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object JsonForAttribute(int attr)
        {
            var value = this.ValueForAttribute(attr);
            return !string.IsNullOrWhiteSpace(value) ? this.ObjectFromJsonString(value) : null;
        }

#if PORTING
        public UPFieldFormat FieldFormatForFieldType(string fieldType)
        {
            var obj = JsonForAttribute((int)FieldAttr.RenderHook);
            if (obj != null && 
                obj.GetType() == typeof(Dictionary<string, string>))
            {
                return UPFieldFormat.FieldFormatForDefinitionFieldType(obj, fieldType);
            }

            return null;
        }
#endif

        /// <summary>
        /// Extendeds the option for key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// option value
        /// </returns>
        public string ExtendedOptionForKey(string key)
        {
            return this.ExtendedOptions?.ValueOrDefault(key);
        }

        /// <summary>
        /// Render hooks for key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// option value
        /// </returns>
        public string RenderHooksForKey(string key)
        {
            return this.RenderHooks?.ValueOrDefault(key);
        }

        /// <summary>
        /// Values the is true.
        /// </summary>
        /// <param name="val">
        /// The value.
        /// </param>
        /// <returns>
        /// true if the value is true; else false.
        /// </returns>
        private bool ValueIsTrue(object val)
        {
            if (val is string)
            {
                return !val.Equals("false");
            }

            return val as int? != 0;
        }

        /// <summary>
        /// Extendeds the option is set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// true if extended option is set; else false
        /// </returns>
        public bool ExtendedOptionIsSet(string key)
        {
            var val = this.ExtendedOptionForKey(key);
            return val != null && this.ValueIsTrue(val);
        }

        /// <summary>
        /// Extendeds the option is set to false.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// true if extended option is set to false
        /// </returns>
        public bool ExtendedOptionIsSetToFalse(string key)
        {
            var val = this.ExtendedOptionForKey(key);
            return val != null && val == false.ToString().ToLower();
        }

        /// <summary>
        /// Attributeses for mode.
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <returns>
        /// Field attributes
        /// </returns>
        public FieldAttributes AttributesForMode(int mode)
        {
            return new FieldAttributes(this, mode);
        }

        /// <summary>
        /// Needs the specific field control attributes for mode.
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <returns>
        /// true if specific field control is required; else false
        /// </returns>
        public bool NeedSpecificFieldControlAttributesForMode(int mode)
        {
            return this.AttributeArray != null && this.AttributeArray.Any(attribute => (attribute.Editmode & mode) != 0);
        }

        /// <summary>
        /// Determines whether this instance has attributes.
        /// </summary>
        /// <returns>true if has attributes; else false</returns>
        public bool HasAttributes()
        {
            return this.attributes != null && this.attributes.Any();
        }

        /// <summary>
        /// Gets the no attribute.
        /// </summary>
        /// <value>
        /// The no attribute.
        /// </value>
        public static FieldAttributes NoAttribute { get; } = new FieldAttributes(null);

        /// <summary>
        /// Fields the count in line.
        /// </summary>
        /// <param name="lineIndex">
        /// Index of the line.
        /// </param>
        /// <returns>
        /// the field count
        /// </returns>
        public int FieldCountInLine(int lineIndex)
        {
            return this.lineFieldCounts == null
                       ? (lineIndex != 0 ? 0 : this.fieldCount)
                       : (this.lineFieldCounts.Count <= lineIndex ? 0 : this.lineFieldCounts[lineIndex]);
        }

        /// <summary>
        /// Formats the line values.
        /// </summary>
        /// <param name="lineIndex">
        /// Index of the line.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// Formated output
        /// </returns>
        public string FormatLineValues(int lineIndex, List<string> values)
        {
            return this.FormatValues(values);
        }

        /// <summary>
        /// Formats the values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// Formatted output
        /// </returns>
        public string FormatValues(List<string> values)
        {
            var count = values.Count;

            if (count == 0)
            {
                return string.Empty;
            }

            if (count == 1)
            {
                return values[0];
            }

            if (this.CombineWithIndices)
            {
                var str = this.combineString;
                for (var i = count; i > 0; i--)
                {
                    str = this.StringFrom(str, i - 1, values[i - 1]);
                }

                return str;
            }

            int nextField, lineCount = this.LineCount;
            string result = null;
            nextField = 0;
            for (var j = 0; j < lineCount; j++)
            {
                string lineResult = null;
                var fieldCountInLine = this.FieldCountInLine(j);
                for (var i = 0; i < fieldCountInLine; i++)
                {
                    if (nextField >= count)
                    {
                        continue;
                    }

                    var part = values[nextField++];
                    if (!string.IsNullOrWhiteSpace(part))
                    {
                        lineResult = !string.IsNullOrWhiteSpace(lineResult)
                                         ? $"{lineResult}{this.combineString}{part}"
                                         : part;
                    }
                }

                if (!string.IsNullOrWhiteSpace(lineResult))
                {
                    result = !string.IsNullOrWhiteSpace(result)
                                 ? $"{result}{Environment.NewLine}{lineResult}"
                                 : lineResult;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{base.ToString()}, attributes: {this.attributes}]";
        }
    }
}
