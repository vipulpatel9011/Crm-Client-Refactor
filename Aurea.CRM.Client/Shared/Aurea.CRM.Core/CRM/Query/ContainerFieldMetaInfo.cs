// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerFieldMetaInfo.cs" company="Aurea Software Gmbh">
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
//   Container field meta info
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Query
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Container field meta info
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.ICrmDataSourceField" />
    public class UPContainerFieldMetaInfo : ICrmDataSourceField
    {
        /// <summary>
        /// CrmField.FieldType = B
        /// </summary>
        private const string CrmFieldTypeB = "B";

        /// <summary>
        /// CrmField.FieldType = T
        /// </summary>
        private const string CrmFieldTypeT = "T";

        /// <summary>
        /// The result offset.
        /// </summary>
        private int resultOffset;

        /// <summary>
        /// The result value mapper initialized.
        /// </summary>
        private bool resultValueMapperInitialized;

        /// <summary>
        /// The return raw value.
        /// </summary>
        private bool returnRawValue;

        /// <summary>
        /// The catalog.
        /// </summary>
        private UPCatalog catalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerFieldMetaInfo"/> class.
        /// </summary>
        /// <param name="crmField">
        /// The CRM field.
        /// </param>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <param name="fieldFormat">
        /// The field format.
        /// </param>
        public UPContainerFieldMetaInfo(UPCRMField crmField, string functionName, object fieldFormat)
        {
            this.CrmField = crmField;
            this.CrmFieldInfo = UPCRMDataStore.DefaultStore.FieldInfoForField(this.CrmField);
            this.catalog = null;
#if PORTING
            FieldFormat = fieldFormat;
#endif
            this.FunctionName = functionName;
            this.resultValueMapperInitialized = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerFieldMetaInfo"/> class.
        /// </summary>
        /// <param name="fieldControlField">
        /// The field control field.
        /// </param>
        public UPContainerFieldMetaInfo(UPConfigFieldControlField fieldControlField)
            : this(fieldControlField?.Field,
                fieldControlField?.Function,
#if PORTING
                  fieldControlField?.Attributes.FieldFormatForFieldType(fieldControlField.Field.FieldType))
#else
                null)
#endif
        {
            this.ConfigField = fieldControlField;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerFieldMetaInfo"/> class.
        /// </summary>
        /// <param name="crmField">
        /// The CRM field.
        /// </param>
        public UPContainerFieldMetaInfo(UPCRMField crmField)
            : this(crmField, null, null)
        {
        }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId => this.CrmField.InfoAreaId;

        /// <summary>
        /// Gets the information area identifier with link.
        /// </summary>
        /// <value>
        /// The information area identifier with link.
        /// </value>
        public string InfoAreaIdWithLink => this.CrmField.InfoAreaIdWithLink;

        /// <summary>
        /// Gets the information area identifier with link ignore parent.
        /// </summary>
        /// <value>
        /// The information area identifier with link ignore parent.
        /// </value>
        public string InfoAreaIdWithLinkIgnoreParent => this.CrmField.InfoAreaIdWithLinkIgnoreParent;

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId => this.CrmField.LinkId;

        /// <summary>
        /// Gets a value indicating whether this instance is link field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is link field; otherwise, <c>false</c>.
        /// </value>
        public bool IsLinkField => this.CrmField is UPCRMLinkField;

        /// <summary>
        /// Gets a value indicating whether [date time adjustment].
        /// </summary>
        /// <value>
        /// <c>true</c> if [date time adjustment]; otherwise, <c>false</c>.
        /// </value>
        public bool DateTimeAdjustment => this.OtherDateTimeField != null;

        /// <summary>
        /// Gets a value indicating whether this instance is date field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is date field; otherwise, <c>false</c>.
        /// </value>
        public bool IsDateField => this.CrmField.IsDateField;

        /// <summary>
        /// Gets a value indicating whether this instance is time field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is time field; otherwise, <c>false</c>.
        /// </value>
        public bool IsTimeField => this.CrmField.IsTimeField;

        /// <summary>
        /// Gets or sets the position in result.
        /// </summary>
        /// <value>
        /// The position in result.
        /// </value>
        public int PositionInResult { get; set; }

        /// <summary>
        /// Gets the CRM field.
        /// </summary>
        /// <value>
        /// The CRM field.
        /// </value>
        public UPCRMField CrmField { get; private set; }

        /// <summary>
        /// Gets the CRM field information.
        /// </summary>
        /// <value>
        /// The CRM field information.
        /// </value>
        public UPCRMFieldInfo CrmFieldInfo { get; private set; }

        /// <summary>
        /// Gets or sets the position in information area.
        /// </summary>
        /// <value>
        /// The position in information area.
        /// </value>
        public int PositionInInfoArea { get; set; }

        /// <summary>
        /// Gets or sets the information area position.
        /// </summary>
        /// <value>
        /// The information area position.
        /// </value>
        public int InfoAreaPosition { get; set; }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public string FunctionName { get; private set; }

        /// <summary>
        /// Gets or sets the position in server result map.
        /// </summary>
        /// <value>
        /// The position in server result map.
        /// </value>
        public int PositionInServerResultMap { get; set; }

#if PORTING
        public UPFieldFormat FieldFormat { get; private set; }
#endif

        /// <summary>
        /// Gets the configuration field.
        /// </summary>
        /// <value>
        /// The configuration field.
        /// </value>
        public UPConfigFieldControlField ConfigField { get; private set; }

        /// <summary>
        /// Gets or sets the other date time field.
        /// </summary>
        /// <value>
        /// The other date time field.
        /// </value>
        public UPContainerFieldMetaInfo OtherDateTimeField { get; set; }

        /// <summary>
        /// Builds the result mapper.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool BuildResultMapper()
        {
            this.resultValueMapperInitialized = true;
            if (this.CrmFieldInfo?.FieldType == "K")
            {
                this.catalog = UPCRMDataStore.DefaultStore.CatalogForVariableCatalogId(this.CrmFieldInfo.CatNo);
            }
            else if (this.CrmFieldInfo?.FieldType == "X")
            {
                this.catalog = UPCRMDataStore.DefaultStore.CatalogForFixedCatalogId(this.CrmFieldInfo.CatNo);
            }

            return true;
        }

        /// <summary>
        /// Returns Value from raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="option">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueFromRawValue(string rawValue, UPFormatOption option)
        {
            if (!resultValueMapperInitialized)
            {
                BuildResultMapper();
            }

            if (returnRawValue)
            {
                return rawValue;
            }

            if (catalog != null)
            {
                var value = (string)null;

                if (!string.IsNullOrWhiteSpace(rawValue) && int.TryParse(rawValue, out var code))
                {
                    value = catalog.TextValueForCode(code);
                }

                return value ?? (rawValue == "0" ? string.Empty : rawValue);
            }

            if (CrmField.FieldType == CrmFieldTypeB)
            {
                return ValueFromRawValueForCrmFieldTypeB(rawValue, option);
            }
            else if (CrmField.FieldType == CrmFieldTypeT)
            {
                var configStore = ConfigurationUnitStore.DefaultStore;
                if (rawValue == "0000" && configStore.ConfigValueIsSet("Format.EmptyForMidnight"))
                {
                    if (!option.HasFlag(UPFormatOption.Report) || !configStore.ConfigValueIsSet("Format.NoEmptyForMidnightInReports"))
                    {
                        return string.Empty;
                    }
                }
            }

            return ValueForRawValueOptions(rawValue, option);
        }

        /// <summary>
        /// Returns the short value from raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ShortValueFromRawValue(string rawValue, UPFormatOption options)
        {
            if (!this.resultValueMapperInitialized)
            {
                this.BuildResultMapper();
            }

            if (this.returnRawValue)
            {
                return rawValue;
            }

            if (this.catalog != null)
            {
                var value = this.catalog.TextValueForCode(int.Parse(rawValue));
                return value ?? (rawValue == "0" ? string.Empty : rawValue);
            }

            return this.CrmFieldInfo.ShortValueForRawValue(rawValue, options);
        }

        /// <summary>
        /// Values from raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueFromRawValue(string rawValue)
        {
            return this.ValueFromRawValue(rawValue, UPFormatOption.None);
        }

        /// <summary>
        /// Reports the value from raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ReportValueFromRawValue(string rawValue)
        {
            return this.ValueFromRawValue(rawValue, UPFormatOption.Report);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.CrmField} pi={this.PositionInInfoArea} ip={this.InfoAreaPosition} pr={this.PositionInResult}";
        }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId => this.CrmField.FieldId;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label => this.CrmFieldInfo.Label;

        /// <summary>
        /// Subs the field indices.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<object> SubFieldIndices()
        {
            if (this.CrmField.IsCatalogField)
            {
                return null;
            }

            var numberOfArrayFields = this.CrmFieldInfo.NumberOfArrayFields;
            if (numberOfArrayFields <= 0)
            {
                return null;
            }

            List<object> arrayFieldIndices = null;
            for (var i = 0; i < numberOfArrayFields; i++)
            {
                var fieldIndex = this.CrmFieldInfo.ArrayFieldIndexAtIndex(i);
                if (arrayFieldIndices == null)
                {
                    arrayFieldIndices = new List<object> { fieldIndex };
                }
                else
                {
                    arrayFieldIndices.Add(fieldIndex);
                }
            }

            return arrayFieldIndices;
        }

        /// <summary>
        /// Returns value from raw value for CrmField.FieldType = B
        /// </summary>
        /// <param name="rawValue">
        /// The raw value
        /// </param>
        /// <param name="option">
        /// UPFormatOption
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ValueFromRawValueForCrmFieldTypeB(string rawValue, UPFormatOption option)
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            var showYesNo = false;
            var showFieldNameForTrueValue = false;

            const string showFieldNameForTrueSetting = "ShowFieldNameForTrueValue";

            showFieldNameForTrueValue = configStore.ConfigValueIsSet("Format.ShowFieldNameForTrueValue");
            if (option.HasFlag(UPFormatOption.Report) && configStore.ConfigValueIsSet("Format.ShowYesNoInReports"))
            {
                showYesNo = true;
                showFieldNameForTrueValue = false;
            }

            var extendedOption = ConfigField?.Attributes.ExtendedOptionForKey(showFieldNameForTrueSetting);
            if (extendedOption != null)
            {
                if (ConfigField.Attributes.ExtendedOptionIsSetToFalse(showFieldNameForTrueSetting))
                {
                    showFieldNameForTrueValue = false;
                }
                else if (ConfigField.Attributes.ExtendedOptionIsSet(showFieldNameForTrueSetting))
                {
                    showFieldNameForTrueValue = true;
                }
            }

            var value = (string)null;
            if (ConfigField != null)
            {
                if (rawValue == "true")
                {
                    value = ConfigField.ExplicitTrueValue;
                    if (string.IsNullOrWhiteSpace(value) && showFieldNameForTrueValue)
                    {
                        return ConfigField.Label;
                    }
                }
                else
                {
                    value = ConfigField.ExplicitFalseValue;
                    if (string.IsNullOrWhiteSpace(value) && (showFieldNameForTrueValue || (!showYesNo && configStore.ConfigValueIsSet("Format.EmptyForFalse"))))
                    {
                        return string.Empty;
                    }
                }
            }
            else if (showFieldNameForTrueValue)
            {
                return rawValue == "true" ? this.CrmField.Label : string.Empty;
            }
            else if (!showYesNo && configStore.ConfigValueIsSet("Format.EmptyForFalse") && rawValue != "true")
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return ValueForRawValueOptions(rawValue, option);
        }

        /// <summary>
        /// Returns value from raw value and Option
        /// </summary>
        /// <param name="rawValue">
        /// The raw value
        /// </param>
        /// <param name="option">
        /// UPFormatOption
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ValueForRawValueOptions(string rawValue, UPFormatOption option)
        {
#if PORTING
            if (FieldFormat != null)
            {
                return FieldFormat.ValueFromRawValue(rawValue);
            }
#endif
            return CrmFieldInfo?.ValueForRawValueOptions(rawValue, option, ConfigField);
        }
    }
}
