// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmFieldInfo.cs" company="Aurea Software Gmbh">
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
//   The up format option.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel.FieldValueFormatters;
    using DAL;
    using Extensions;
    using Session;

    /// <summary>
    /// The up format option.
    /// </summary>
    [Flags]
    public enum UPFormatOption
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The report.
        /// </summary>
        Report = 1,

        /// <summary>
        /// The show 0.
        /// </summary>
        Show0 = 2,

        /// <summary>
        /// The show 0 float.
        /// </summary>
        Show0Float = 4,

        /// <summary>
        /// The dont remove line break.
        /// </summary>
        DontRemoveLineBreak = 8,
    }

    /// <summary>
    /// CRM field info
    /// </summary>
    public class UPCRMFieldInfo
    {
        /// <summary>
        /// The field information
        /// </summary>
        private FieldInfo fieldInfo;

        private UPCRMFieldInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldInfo" /> class.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="dataStore">The data store.</param>
        /// <returns>UPCRMFieldInfo</returns>
        public static UPCRMFieldInfo Create(int fieldId, string infoAreaId, ICRMDataStore dataStore)
        {
            var tableInfo = dataStore.DatabaseInstance?.GetTableInfoByInfoArea(infoAreaId);
            if (tableInfo == null)
            {
                return null;
            }

            return new UPCRMFieldInfo
            {
                InfoAreaId = infoAreaId,
                FieldId = fieldId,
                fieldInfo = tableInfo.GetFieldInfo(fieldId)
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldInfo" /> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="fieldInfoPtr">The field information PTR.</param>
        /// <returns>UPCRMFieldInfo</returns>
        public static UPCRMFieldInfo Create(string infoAreaId, FieldInfo fieldInfoPtr)
        {
            return new UPCRMFieldInfo
            {
                fieldInfo = fieldInfoPtr,
                FieldId = fieldInfoPtr.FieldId,
                InfoAreaId = infoAreaId
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldInfo" /> class.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <returns>UPCRMFieldInfo</returns>
        public static UPCRMFieldInfo Create(int fieldId, string infoAreaId)
        {
            return Create(fieldId, infoAreaId, UPCRMDataStore.DefaultStore);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldInfo" /> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="dataStore">The data store.</param>
        /// <returns>UPCRMFieldInfo</returns>
        public static UPCRMFieldInfo Create(UPCRMField field, ICRMDataStore dataStore)
        {
            if (field == null || dataStore == null)
            {
                return null;
            }

            var tableInfo = dataStore.DatabaseInstance?.GetTableInfoByInfoArea(field.InfoAreaId);

            var fieldInfo = tableInfo?.GetFieldInfo(field.FieldId);
            if (fieldInfo == null)
            {
                return null;
            }

            return new UPCRMFieldInfo
            {
                InfoAreaId = field.InfoAreaId,
                FieldId = field.FieldId,
                fieldInfo = fieldInfo
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldInfo" /> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>UPCRMFieldInfo</returns>
        public static UPCRMFieldInfo Create(UPCRMField field)
        {
            return Create(field, UPCRMDataStore.DefaultStore);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldInfo" /> class.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="crmtableInfo">The crmtable information.</param>
        /// <returns> UPCRMFieldInfo</returns>
        public static UPCRMFieldInfo Create(int fieldId, UPCRMTableInfo crmtableInfo)
        {
            var tableInfo = crmtableInfo.DataStore.DatabaseInstance.GetTableInfoByInfoArea(crmtableInfo.InfoAreaId);
            var fieldInfo = tableInfo?.GetFieldInfo(fieldId);
            if (fieldInfo == null)
            {
                return null;
            }

            return new UPCRMFieldInfo
            {
                InfoAreaId = crmtableInfo.InfoAreaId,
                FieldId = fieldId,
                fieldInfo = fieldInfo
            };
        }

        /// <summary>
        /// Gets a value indicating whether [amount field].
        /// </summary>
        /// <value>
        /// <c>true</c> if [amount field]; otherwise, <c>false</c>.
        /// </value>
        public bool AmountField => this.fieldInfo?.IsAmount ?? false;

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public int Attributes => this.fieldInfo?.Attributes ?? 0;

        /// <summary>
        /// Gets the cat no.
        /// </summary>
        /// <value>
        /// The cat no.
        /// </value>
        public int CatNo => this.fieldInfo?.Cat ?? 0;

        /// <summary>
        /// Gets the database field information.
        /// </summary>
        /// <value>
        /// The database field information.
        /// </value>
        public FieldInfo DatabaseFieldInfo => this.fieldInfo;

        /// <summary>
        /// Gets the name of the database field.
        /// </summary>
        /// <value>
        /// The name of the database field.
        /// </value>
        public string DatabaseFieldName => this.fieldInfo?.DatabaseFieldName;

        /// <summary>
        /// Gets the date field identifier.
        /// </summary>
        /// <value>
        /// The date field identifier.
        /// </value>
        public int DateFieldId => this.fieldInfo?.FieldType == 'T' ? this.fieldInfo.UCat : -1;

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets the length of the field.
        /// </summary>
        /// <value>
        /// The length of the field.
        /// </value>
        public int FieldLength => this.fieldInfo?.FieldLen ?? 0;

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public string FieldType => $"{this.fieldInfo?.FieldType}";

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public int Format => this.fieldInfo?.Format ?? 0;

        /// <summary>
        /// Gets a value indicating whether [four decimal digits].
        /// </summary>
        /// <value>
        /// <c>true</c> if [four decimal digits]; otherwise, <c>false</c>.
        /// </value>
        public bool FourDecimalDigits => this.fieldInfo?.FourDecimalDigits ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance has group separator.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has group separator; otherwise, <c>false</c>.
        /// </value>
        public bool HasGroupSeparator => this.fieldInfo?.HasGroupingSeparator ?? false;

        /// <summary>
        /// Gets a value indicating whether [HTML field].
        /// </summary>
        /// <value>
        /// <c>true</c> if [HTML field]; otherwise, <c>false</c>.
        /// </value>
        public bool HtmlField => this.fieldInfo?.IsHtml ?? false;

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is catalog field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is catalog field; otherwise, <c>false</c>.
        /// </value>
        public bool IsCatalogField => this.fieldInfo?.IsCatalog ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is date field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is date field; otherwise, <c>false</c>.
        /// </value>
        public bool IsDateField => this.fieldInfo?.IsDate ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is numeric field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is numeric field; otherwise, <c>false</c>.
        /// </value>
        public bool IsNumericField => this.fieldInfo?.IsNumeric ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is participants field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is participants field; otherwise, <c>false</c>.
        /// </value>
        public bool IsParticipantsField => this.fieldInfo?.IsParticipantsField ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is readonly.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is readonly; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadonly => (this.Rights & 0x20) > 0;

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is time field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is time field; otherwise, <c>false</c>.
        /// </value>
        public bool IsTimeField => this.fieldInfo?.IsTime ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is variable catalog field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is variable catalog field; otherwise, <c>false</c>.
        /// </value>
        public bool IsVariableCatalogField => this.fieldInfo?.IsVariableCatalog ?? false;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label => this.fieldInfo?.Name;

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCRMFieldInfo"/> is locked.
        /// </summary>
        /// <value>
        /// <c>true</c> if locked; otherwise, <c>false</c>.
        /// </value>
        public bool Locked => this.fieldInfo != null && (this.Rights & 0x00000001) <= 0;

        /// <summary>
        /// Gets a value indicating whether [locked on new].
        /// </summary>
        /// <value>
        /// <c>true</c> if [locked on new]; otherwise, <c>false</c>.
        /// </value>
        public bool LockedOnNew => this.fieldInfo != null && (this.Rights & 0x00000002) <= 0;

        /// <summary>
        /// Gets a value indicating whether [locked on update].
        /// </summary>
        /// <value>
        /// <c>true</c> if [locked on update]; otherwise, <c>false</c>.
        /// </value>
        public bool LockedOnUpdate => this.fieldInfo != null && (this.Rights & 0x00000004) <= 0;

        /// <summary>
        /// Gets a value indicating whether [must field].
        /// </summary>
        /// <value>
        /// <c>true</c> if [must field]; otherwise, <c>false</c>.
        /// </value>
        public bool MustField => (this.fieldInfo?.Rights & 0x40000000) != 0;

        /// <summary>
        /// Gets a value indicating whether [no decimal digits].
        /// </summary>
        /// <value>
        /// <c>true</c> if [no decimal digits]; otherwise, <c>false</c>.
        /// </value>
        public bool NoDecimalDigits => this.fieldInfo?.NoDecimalDigits ?? false;

        /// <summary>
        /// Gets the number of array fields.
        /// </summary>
        /// <value>
        /// The number of array fields.
        /// </value>
        public int NumberOfArrayFields => this.fieldInfo?.ArrayFieldCount ?? -1;

        /// <summary>
        /// Gets a value indicating whether [one decimal digit].
        /// </summary>
        /// <value>
        /// <c>true</c> if [one decimal digit]; otherwise, <c>false</c>.
        /// </value>
        public bool OneDecimalDigit => this.fieldInfo?.OneDecimalDigit ?? false;

        /// <summary>
        /// Gets the parent catalog field identifier.
        /// </summary>
        /// <value>
        /// The parent catalog field identifier.
        /// </value>
        public int ParentCatalogFieldId => this.fieldInfo?.FieldType == 'K' ? (int)this.fieldInfo?.UCat : -1;

        /// <summary>
        /// Gets the parent field identifier.
        /// </summary>
        /// <value>
        /// The parent field identifier.
        /// </value>
        public int ParentFieldId => this.fieldInfo?.UCat ?? 0;

        /// <summary>
        /// Gets a value indicating whether [percent field].
        /// </summary>
        /// <value>
        /// <c>true</c> if [percent field]; otherwise, <c>false</c>.
        /// </value>
        public bool PercentField => this.fieldInfo?.IsPercent ?? false;

        /// <summary>
        /// Gets the referenced rep z field.
        /// </summary>
        /// <value>
        /// The referenced rep z field.
        /// </value>
        public int ReferencedRepZField => !string.IsNullOrEmpty(this.fieldInfo?.RepMode) ? this.fieldInfo.ZField : -1;

        /// <summary>
        /// Gets a value indicating whether [report raw value].
        /// </summary>
        /// <value>
        /// <c>true</c> if [report raw value]; otherwise, <c>false</c>.
        /// </value>
        public bool ReportRawValue => this.fieldInfo?.FieldType == 'C';

        /// <summary>
        /// Gets the type of the rep.
        /// </summary>
        /// <value>
        /// The type of the rep.
        /// </value>
        public string RepType => this.fieldInfo?.RepMode ?? string.Empty;

        /// <summary>
        /// Gets the rights.
        /// </summary>
        /// <value>
        /// The rights.
        /// </value>
        public int Rights => this.fieldInfo?.Rights ?? 0;

        /// <summary>
        /// Gets a value indicating whether [show zero].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show zero]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowZero => this.fieldInfo?.ShowZero ?? false;

        /// <summary>
        /// Gets a value indicating whether [three decimal digits].
        /// </summary>
        /// <value>
        /// <c>true</c> if [three decimal digits]; otherwise, <c>false</c>.
        /// </value>
        public bool ThreeDecimalDigits => this.fieldInfo?.ThreeDecimalDigits ?? false;

        /// <summary>
        /// Gets the time field identifier.
        /// </summary>
        /// <value>
        /// The time field identifier.
        /// </value>
        public int TimeFieldId => this.fieldInfo?.FieldType == 'D' ? this.fieldInfo.UCat : -1;

        /// <summary>
        /// Arrays the index of the field index at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ArrayFieldIndexAtIndex(int index)
        {
            return this.fieldInfo?.ArrayFieldIndices?[index] ?? 0;
        }

        /// <summary>
        /// Exts the key for raw value options.
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
        public string ExtKeyForRawValueOptions(string rawValue, int options)
        {
            if (this.FieldType[0] != 'K')
            {
                return null;
            }

            return UPCRMDataStore.DefaultStore.ExtKeyForVariableCatalogIdRawValue(this.fieldInfo.Cat, rawValue);
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
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            if (string.IsNullOrEmpty(this.FieldType))
            {
                return false;
            }

            switch (this.FieldType[0])
            {
                case 'L':
                case 'S':
                case 'N':
                case 'K':
                case 'X':
                    if (value == "0")
                    {
                        var crmField = new UPCRMField(this.FieldId, this.InfoAreaId);
                        return crmField.IsEmptyValue("0");
                    }

                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the short the value for raw value.
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
        public string ShortValueForRawValue(string rawValue, UPFormatOption options)
        {
            if (this.FieldType[0] != 'D')
            {
                return this.ValueForRawValueOptions(rawValue, options, null);
            }

            DateTime? date = DateTime.Parse(rawValue);
            return options == UPFormatOption.Report ? date?.ReportFormattedDate() : date.ShortLocalizedFormattedDate();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"id={this.InfoAreaId}.{this.FieldId}, type={this.FieldType}, catNo={this.CatNo}, parent={this.ParentFieldId}, rep={this.RepType}";
        }

        /// <summary>
        /// Values for raw value options.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="configField">
        /// The config field.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueForRawValueOptions(string rawValue, UPFormatOption options, UPConfigFieldControlField configField)
        {
            if (string.IsNullOrEmpty(rawValue))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(this.FieldType))
            {
                return rawValue;
            }

            var fieldType = this.FieldType[0];
            var fieldTypeValue = configField?.Attributes.RenderHooksForKey("FieldType");

            if (!string.IsNullOrWhiteSpace(fieldTypeValue))
            {
                fieldType = fieldTypeValue[0];
            }

            switch (fieldType)
            {
                case 'D':
                    return FieldValueFormatterDateTime.ConvertDate(rawValue, options);

                case 'T':
                    return FieldValueFormatterDateTime.ConvertTime(rawValue);

                case 'B':
                    if (rawValue == "true")
                    {
                        return ServerSession.CurrentSession.UpTextForYES();
                    }

                    return !string.IsNullOrEmpty(rawValue) ? ServerSession.CurrentSession.UpTextForNO() : string.Empty;

                
                case 'S'://case 'L':
                case 'F':
                    return FieldValueFormatterNumeric.Convert(rawValue, fieldType, options, configField, this, this.fieldInfo);

                case 'X':
                    return UPCRMDataStore.DefaultStore.CatalogValueForFixedCatalogIdRawValue(this.fieldInfo.Cat, rawValue);

                case 'K':
                    return UPCRMDataStore.DefaultStore.CatalogValueForVariableCatalogIdRawValue(this.fieldInfo.Cat, rawValue);

                default:
                    return rawValue;
            }
        }
    }
}
