// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldControlField.cs" company="Aurea Software Gmbh">
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
//   Field control field configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Field control field configurations
    /// </summary>
    public class UPConfigFieldControlField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFieldControlField"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <param name="tabConfig">
        /// The tab configuration.
        /// </param>
        public UPConfigFieldControlField(UPConfigFieldControlField source, int mode, FieldControlTab tabConfig)
        {
            this.Field = source.Field;
            this.TabIndependentFieldIndex = source.TabIndependentFieldIndex;
            this.Attributes = source.Attributes.AttributesForMode(mode);
            this.TargetFieldNumber = source.TargetFieldNumber;
            this.ExplicitLabel = source.ExplicitLabel;
            this.Function = source.Function;
            this.TabConfig = tabConfig;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFieldControlField"/> class.
        /// </summary>
        /// <param name="def">
        /// The definition.
        /// </param>
        /// <param name="tabIndependentFieldIndex">
        /// Index of the tab independent field.
        /// </param>
        /// <param name="tabConfig">
        /// The tab configuration.
        /// </param>
        public UPConfigFieldControlField(List<object> def, int tabIndependentFieldIndex, FieldControlTab tabConfig)
        {
            var linkId = 0;
            if (def.Count > 7)
            {
                linkId = JObjectExtensions.ToInt(def[7]);
            }

            var fieldattributedef = (def[2] as JArray)?.ToObject<List<object>>();
            this.Attributes = fieldattributedef != null
                                  ? new FieldAttributes(fieldattributedef)
                                  : FieldAttributes.NoAttribute;

            UPCRMFieldParentLink fieldParentLink = null;
            var parentLinkString = this.Attributes.ExtendedOptionForKey("parentLink");
            if (!string.IsNullOrEmpty(parentLinkString))
            {
                fieldParentLink = UPCRMFieldParentLink.LinkFromString(parentLinkString);
            }

            if (fieldParentLink != null)
            {
                this.Field = new UPCRMField(
                    JObjectExtensions.ToInt(def[1]),
                    (string)def[0],
                    linkId > 0 ? linkId : -1,
                    fieldParentLink);
            }
            else if (linkId > 0)
            {
                this.Field = new UPCRMField(JObjectExtensions.ToInt(def[1]), (string)def[0], linkId);
            }
            else
            {
                this.Field = new UPCRMField(JObjectExtensions.ToInt(def[1]), (string)def[0]);
            }

            if (def.Count > 3)
            {
                this.TargetFieldNumber = JObjectExtensions.ToInt(def[3]);
            }

            var val = def[4] as string;
            if (val != null)
            {
                this.ExplicitLabel = val;
            }

            // val = [def objectAtIndex:5];
            // if (val != [NSNull null]) tooltip = (NSString *) val;
            if (def.Count() > 6)
            {
                val = def[6] as string;
                if (val != null)
                {
                    this.Function = val;
                }
            }

            this.TabConfig = tabConfig;
            this.TabIndependentFieldIndex = tabIndependentFieldIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFieldControlField"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="tabIndependentFieldIndex">
        /// Index of the tab independent field.
        /// </param>
        /// <param name="tabConfig">
        /// The tab configuration.
        /// </param>
        /// <param name="rootInfoAreaId">
        /// The root information area identifier.
        /// </param>
        /// <param name="rootLinkId">
        /// The root link identifier.
        /// </param>
        public UPConfigFieldControlField(
            UPConfigFieldControlField source,
            int tabIndependentFieldIndex,
            FieldControlTab tabConfig,
            string rootInfoAreaId,
            int rootLinkId)
        {
            if (rootInfoAreaId != null && rootInfoAreaId == source.Field.InfoAreaId && rootLinkId >= 0 && rootLinkId == source.Field.LinkId)
            {
                this.Field = UPCRMField.FieldWithFieldIdInfoAreaIdLinkId(source.Field.FieldId, source.Field.InfoAreaId, -1);
            }
            else
            {
                this.Field = source.Field;
            }

            this.TabIndependentFieldIndex = tabIndependentFieldIndex;
            this.Attributes = source.Attributes;
            this.TargetFieldNumber = source.TargetFieldNumber;
            this.ExplicitLabel = source.ExplicitLabel;
            this.Function = source.Function;
            this.TabConfig = tabConfig;
        }

        /// <summary>
        /// Gets the explicit false value.
        /// </summary>
        /// <value>
        /// The explicit false value.
        /// </value>
        public string ExplicitFalseValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.ExplicitLabel))
                {
                    return null;
                }

                var parts = this.ExplicitLabel.Split(';');
                return parts.Length > 2 ? parts[2] : null;
            }
        }

        /// <summary>
        /// Gets the index of the tab independent field.
        /// </summary>
        /// <value>
        /// The index of the tab independent field.
        /// </value>
        public int TabIndependentFieldIndex { get; private set; }

        /// <summary>
        /// Needses the specific field control field for mode.
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool NeedsSpecificFieldControlFieldForMode(FieldDetailsMode mode)
        {
            return this.Attributes.NeedSpecificFieldControlAttributesForMode((int)mode);
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public FieldAttributes Attributes { get; private set; }

        /// <summary>
        /// Gets the target field number.
        /// </summary>
        /// <value>
        /// The target field number.
        /// </value>
        public int TargetFieldNumber { get; private set; }

        /// <summary>
        /// Gets the explicit label.
        /// </summary>
        /// <value>
        /// The explicit label.
        /// </value>
        public string ExplicitLabel { get; private set; }

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        public string Function { get; private set; }

        /// <summary>
        /// Gets the tab configuration.
        /// </summary>
        /// <value>
        /// The tab configuration.
        /// </value>
        public FieldControlTab TabConfig { get; private set; }

#if PORTING
        public UPFieldFormat FieldFormat
            => Attributes.FieldFormatForFieldType(Field.FieldType);
#endif

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public UPCRMField Field { get; private set; }

        /// <summary>
        /// The field meta information
        /// </summary>
        protected UPContainerFieldMetaInfo fieldMetaInfo;

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId => this.Field.InfoAreaId;

        /// <summary>
        /// Gets a value indicating whether this instance is linked field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is linked field; otherwise, <c>false</c>.
        /// </value>
        public bool IsLinkedField => this.TabConfig.FieldControl.InfoAreaId != this.InfoAreaId;

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId => this.Field.LinkId;

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier. Returns 0 if field is null.
        /// </value>
        public int FieldId => this.Field?.FieldId ?? 0;

        /// <summary>
        /// Gets the identification.
        /// </summary>
        /// <value>
        /// The identification.
        /// </value>
        public string Identification => this.Field.FieldIdentification;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.ExplicitLabel))
                {
                    return this.Field.Label;
                }

                var parts = this.ExplicitLabel.Split(';');
                var _label = parts[0];
                return !string.IsNullOrWhiteSpace(_label) ? _label : this.Field.Label;
            }
        }

        /// <summary>
        /// Gets the full label.
        /// </summary>
        /// <value>
        /// The full label.
        /// </value>
        public string FullLabel
            => !string.IsNullOrWhiteSpace(this.ExplicitLabel) ? this.ExplicitLabel : this.Field.Label;

        /// <summary>
        /// Gets the explicit true value.
        /// </summary>
        /// <value>
        /// The explicit true value.
        /// </value>
        public string ExplicitTrueValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.ExplicitLabel))
                {
                    return null;
                }

                var parts = this.ExplicitLabel.Split(';');
                return parts.Length > 1 ? parts[1] : null;
            }
        }

        /// <summary>
        /// Determines whether [is empty value] [the specified value].
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsEmptyValue(string value)
        {
            return this.Field.IsEmptyValue(value);
        }

        /// <summary>
        /// Values from raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// parsed vlue
        /// </returns>
        public string ValueFromRawValue(string rawValue)
        {
            if (this.fieldMetaInfo == null)
            {
                this.fieldMetaInfo = new UPContainerFieldMetaInfo(this);
            }

            return this.fieldMetaInfo.ValueFromRawValue(rawValue);
        }

        /// <summary>
        /// Values from raw value options.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// parsed value
        /// </returns>
        public string ValueFromRawValueOptions(string rawValue, UPFormatOption options)
        {
            if (this.fieldMetaInfo == null)
            {
                this.fieldMetaInfo = new UPContainerFieldMetaInfo(this);
            }

            return this.fieldMetaInfo.ValueFromRawValue(rawValue, options);
        }

        /// <summary>
        /// Shorts the value from raw value options.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// short value
        /// </returns>
        public string ShortValueFromRawValueOptions(string rawValue, UPFormatOption options)
        {
            if (this.fieldMetaInfo == null)
            {
                this.fieldMetaInfo = new UPContainerFieldMetaInfo(this);
            }

            return this.fieldMetaInfo.ShortValueFromRawValue(rawValue, options);
        }

        /// <summary>
        /// Reports the value from raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// report valu
        /// </returns>
        public string ReportValueFromRawValue(string rawValue)
        {
            if (this.fieldMetaInfo == null)
            {
                this.fieldMetaInfo = new UPContainerFieldMetaInfo(this);
            }

            return this.fieldMetaInfo.ReportValueFromRawValue(rawValue);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.LinkId > 0
                       ? $"{this.InfoAreaId}:{this.LinkId}.{this.FieldId}"
                       : $"{this.InfoAreaId}.{this.FieldId}";
        }
    }
}
