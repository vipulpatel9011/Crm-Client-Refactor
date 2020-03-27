// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmField.cs" company="Aurea Software Gmbh">
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
//   Conditional operator types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Conditional operator types
    /// </summary>
    public enum UPConditionOperator
    {
        /// <summary>
        /// The equal.
        /// </summary>
        Equal = 0,

        /// <summary>
        /// The not equal.
        /// </summary>
        NotEqual,

        /// <summary>
        /// The like.
        /// </summary>
        Like,

        /// <summary>
        /// The greater equal.
        /// </summary>
        GreaterEqual,

        /// <summary>
        /// The greater.
        /// </summary>
        Greater,

        /// <summary>
        /// The less.
        /// </summary>
        Less,

        /// <summary>
        /// The less equal.
        /// </summary>
        LessEqual,

        /// <summary>
        /// The between.
        /// </summary>
        Between
    }

    /// <summary>
    /// CRM Field implementation
    /// </summary>
    public class UPCRMField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMField"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public UPCRMField(int fieldId, string infoAreaId)
            : this(fieldId, infoAreaId, -1, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMField"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public UPCRMField(int fieldId, string infoAreaId, int linkId)
            : this(fieldId, infoAreaId, linkId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMField"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="parentLink">
        /// The parent link.
        /// </param>
        public UPCRMField(int fieldId, string infoAreaId, int linkId, UPCRMFieldParentLink parentLink)
        {
            this.InfoAreaId = infoAreaId;
            this.FieldId = fieldId;
            this.LinkId = linkId;
            this.ParentLink = parentLink;
        }

        /// <summary>
        /// Gets the catalog.
        /// </summary>
        /// <value>
        /// The catalog.
        /// </value>
        public UPCatalog Catalog => UPCRMDataStore.DefaultStore.CatalogForCrmField(this);

        /// <summary>
        /// Gets the cat no.
        /// </summary>
        /// <value>
        /// The cat no.
        /// </value>
        public int CatNo
        {
            get
            {
                var fieldInfo = this.FieldInfo;
                return fieldInfo?.CatNo ?? -1;
            }
        }

        /// <summary>
        /// Gets or sets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; set; }

        /// <summary>
        /// Gets the field identification.
        /// </summary>
        /// <value>
        /// The field identification.
        /// </value>
        public string FieldIdentification => this.InfoAreaId.InfoAreaIdLinkIdFieldId(this.LinkId, this.FieldId);

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public string FieldType
        {
            get
            {
                var fieldInfo = this.FieldInfo;
                return fieldInfo != null ? fieldInfo.FieldType : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; set; }

        /// <summary>
        /// Gets the information area identifier with link.
        /// </summary>
        /// <value>
        /// The information area identifier with link.
        /// </value>
        public string InfoAreaIdWithLink
            =>
                this.ParentLink != null
                    ? $"{this.ParentLink.Key}{this.InfoAreaId.InfoAreaIdLinkId(this.LinkId)}"
                    : this.InfoAreaId.InfoAreaIdLinkId(this.LinkId);

        /// <summary>
        /// Gets the information area identifier with link ignore parent.
        /// </summary>
        /// <value>
        /// The information area identifier with link ignore parent.
        /// </value>
        public string InfoAreaIdWithLinkIgnoreParent => this.InfoAreaId.InfoAreaIdLinkId(this.LinkId);

        /// <summary>
        /// Gets a value indicating whether this instance is catalog field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is catalog field; otherwise, <c>false</c>.
        /// </value>
        public bool IsCatalogField => this.FieldInfo?.IsCatalogField ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is date field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is date field; otherwise, <c>false</c>.
        /// </value>
        public bool IsDateField => this.FieldInfo?.IsDateField ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is link field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is link field; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsLinkField => false;

        /// <summary>
        /// Gets a value indicating whether this instance is numeric field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is numeric field; otherwise, <c>false</c>.
        /// </value>
        public bool IsNumericField => this.FieldInfo?.IsNumericField ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is rep field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is rep field; otherwise, <c>false</c>.
        /// </value>
        public bool IsRepField => !string.IsNullOrEmpty(this.RepType);

        /// <summary>
        /// Gets a value indicating whether this instance is time field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is time field; otherwise, <c>false</c>.
        /// </value>
        public bool IsTimeField => this.FieldInfo?.IsTimeField ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance is variable catalog field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is variable catalog field; otherwise, <c>false</c>.
        /// </value>
        public bool IsVariableCatalogField => this.FieldInfo?.IsVariableCatalogField ?? false;

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
                var fieldInfo = this.FieldInfo;
                return fieldInfo != null ? fieldInfo.Label : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; set; }

        /// <summary>
        /// Gets the parent link.
        /// </summary>
        /// <value>
        /// The parent link.
        /// </value>
        public UPCRMFieldParentLink ParentLink { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [report raw value].
        /// </summary>
        /// <value>
        /// <c>true</c> if [report raw value]; otherwise, <c>false</c>.
        /// </value>
        public bool ReportRawValue
        {
            get
            {
                var fieldInfo = this.FieldInfo;
                return fieldInfo != null && fieldInfo.ReportRawValue;
            }
        }

        /// <summary>
        /// Gets the type of the rep.
        /// </summary>
        /// <value>
        /// The type of the rep.
        /// </value>
        public string RepType
        {
            get
            {
                var fieldInfo = this.FieldInfo;
                return fieldInfo != null ? fieldInfo.RepType : string.Empty;
            }
        }

        /// <summary>
        /// Conditions the operator from string.
        /// </summary>
        /// <param name="compareString">
        /// The compare string.
        /// </param>
        /// <returns>
        /// The <see cref="UPConditionOperator"/>.
        /// </returns>
        public static UPConditionOperator ConditionOperatorFromString(string compareString)
        {
            if (string.IsNullOrEmpty(compareString) || compareString == "=")
            {
                return UPConditionOperator.Equal;
            }

            if (compareString == "<>")
            {
                return UPConditionOperator.NotEqual;
            }

            if (compareString == "LIKE")
            {
                return UPConditionOperator.Like;
            }

            if (compareString == ">=")
            {
                return UPConditionOperator.GreaterEqual;
            }

            if (compareString == ">")
            {
                return UPConditionOperator.Greater;
            }

            if (compareString == "<=")
            {
                return UPConditionOperator.LessEqual;
            }

            if (compareString == "<")
            {
                return UPConditionOperator.Less;
            }

            if (compareString == "><")
            {
                return UPConditionOperator.Between;
            }

            return UPConditionOperator.Equal;
        }

        /// <summary>
        /// Empties the field with information area.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMField"/>.
        /// </returns>
        public static UPCRMField EmptyFieldWithInfoArea(string infoAreaId)
        {
            return new UPCRMField(-1, infoAreaId);
        }

        /// <summary>
        /// Fields from field information.
        /// </summary>
        /// <param name="fieldInfo">
        /// The field information.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMField"/>.
        /// </returns>
        public static UPCRMField FieldFromFieldInfo(UPCRMFieldInfo fieldInfo)
        {
            return new UPCRMField(fieldInfo.FieldId, fieldInfo.InfoAreaId);
        }

        /// <summary>
        /// Fields from string.
        /// </summary>
        /// <param name="fieldString">
        /// The field string.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMField"/>.
        /// </returns>
        public static UPCRMField FieldFromString(string fieldString)
        {
            var parts = fieldString.Split('.');
            if (parts.Length < 2)
            {
                return null;
            }

            var infoAreaId = parts[0];
            var fieldId = int.Parse(parts[1]);
            parts = infoAreaId.Split('#');
            if (parts.Length <= 1)
            {
                return FieldWithFieldIdInfoAreaId(fieldId, infoAreaId);
            }

            var linkId = int.Parse(parts[1]);
            return FieldWithFieldIdInfoAreaIdLinkId(fieldId, parts[0], linkId);
        }

        /// <summary>
        /// Fields the with field identifier information area identifier.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMField"/>.
        /// </returns>
        public static UPCRMField FieldWithFieldIdInfoAreaId(int fieldId, string infoAreaId)
        {
            return new UPCRMField(fieldId, infoAreaId);
        }

        /// <summary>
        /// Fields the with field identifier information area identifier link identifier.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMField"/>.
        /// </returns>
        public static UPCRMField FieldWithFieldIdInfoAreaIdLinkId(int fieldId, string infoAreaId, int linkId)
        {
            return new UPCRMField(fieldId, infoAreaId, linkId);
        }

        /// <summary>
        /// Likes the result for value pattern.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool LikeResultForValuePattern(string value, string fieldValue)
        {
            var pattern = fieldValue;
            var patternLength = pattern.Length;
            if (fieldValue.EndsWith("*"))
            {
                pattern = fieldValue.Substring(0, patternLength - 1);
                return value.Length >= patternLength - 1 && value.Substring(0, patternLength - 1) == pattern;
            }

            return value == fieldValue;
        }

        /// <summary>
        /// Results for value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="compare">
        /// The compare.
        /// </param>
        /// <param name="_fieldValue">
        /// The _field value.
        /// </param>
        /// <param name="_toFieldValue">
        /// The _to field value.
        /// </param>
        /// <param name="numericEmptyCheck">
        /// if set to <c>true</c> [numeric empty check].
        /// </param>
        /// <param name="boolEmptyCheck">
        /// if set to <c>true</c> [bool empty check].
        /// </param>
        /// <param name="emptyFieldValue">
        /// if set to <c>true</c> [empty field value].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ResultForValue(
            string value,
            UPConditionOperator compare,
            string _fieldValue,
            string _toFieldValue,
            bool numericEmptyCheck,
            bool boolEmptyCheck,
            bool emptyFieldValue)
        {
            var result = 0;
            if (emptyFieldValue)
            {
                if (string.IsNullOrEmpty(value))
                {
                    result = 0;
                }
                else if (numericEmptyCheck)
                {
                    var intValue = int.Parse(value);
                    if (intValue == 0)
                    {
                        result = 0;
                    }
                    else if (intValue > 0)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = -1;
                    }
                }
                else if (boolEmptyCheck && value == "false")
                {
                    result = 0;
                }
                else
                {
                    result = 1;
                }
            }
            else if (compare == UPConditionOperator.Like)
            {
                return LikeResultForValuePattern(value, _fieldValue);
            }
            else
            {
                result = value.CompareTo(_fieldValue);
            }

            switch (compare)
            {
                case UPConditionOperator.Equal:
                    return result == 0;

                case UPConditionOperator.NotEqual:
                    return result != 0;

                case UPConditionOperator.GreaterEqual:
                    return result != -1;

                case UPConditionOperator.Greater:
                    return result == 1;

                case UPConditionOperator.Less:
                    return result == -1;

                case UPConditionOperator.LessEqual:
                    return result != 1;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks the value matches value condition.
        /// </summary>
        /// <param name="firstValue">
        /// The first value.
        /// </param>
        /// <param name="secondValue">
        /// The second value.
        /// </param>
        /// <param name="conditionOperator">
        /// The condition operator.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckValueMatchesValueCondition(
            string firstValue,
            string secondValue,
            UPConditionOperator conditionOperator)
        {
            var result = this.CompareValueWithValue(firstValue, secondValue);
            switch (conditionOperator)
            {
                case UPConditionOperator.Equal:
                    return result == 0;

                case UPConditionOperator.NotEqual:
                    return result != 0;

                case UPConditionOperator.GreaterEqual:
                    return result != -1;

                case UPConditionOperator.Greater:
                    return result == 1;

                case UPConditionOperator.Less:
                    return result == -1;

                case UPConditionOperator.LessEqual:
                    return result != 1;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Compares the value with value.
        /// </summary>
        /// <param name="firstValue">
        /// The first value.
        /// </param>
        /// <param name="secondValue">
        /// The second value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CompareValueWithValue(string firstValue, string secondValue)
        {
            var firstValueIsEmpty = this.IsEmptyValue(firstValue);
            var secondValueIsEmpty = this.IsEmptyValue(secondValue);
            if (firstValueIsEmpty)
            {
                return secondValueIsEmpty ? 0 : -1;
            }

            if (secondValueIsEmpty)
            {
                return 1;
            }
            int diff;
            var fieldType = default(char);
            if (this.FieldType.Length >= 1)
            {
                fieldType = this.FieldType[0];
            }
            else
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogWarn($"Field has empty field type: {this}");
            }

            switch (fieldType)
            {
                case 'B':
                    return 0;
                case 'L':
                case 'S':
                case 'N':
                    diff = int.Parse(firstValue) - int.Parse(secondValue);
                    if (diff > 0)
                    {
                        return 1;
                    }

                    return diff < 0 ? -1 : 0;

                default:
                    return firstValue.CompareTo(secondValue);
            }
        }

        /// <summary>
        /// Empties the is value.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool EmptyIsValue()
        {
            return !string.IsNullOrEmpty(this.FieldType) && this.FieldType[0] == 'X'
                   && UPCRMDataStore.DefaultStore.CatalogForFixedCatalogId(this.CatNo).ZeroHasValue;
        }

        /// <summary>
        /// Fields the information.
        /// </summary>
        /// <returns>
        /// The <see cref="UPCRMFieldInfo"/>.
        /// </returns>
        public UPCRMFieldInfo FieldInfo => UPCRMDataStore.DefaultStore.FieldInfoForField(this);

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
                return !this.EmptyIsValue();
            }

            if (string.IsNullOrEmpty(this.FieldType))
            {
                return false;
            }

            switch (this.FieldType[0])
            {
                case 'B':
                    return value.ToUpper() == "FALSE";
                case 'L':
                case 'S':
                case 'N':
                case 'K':
                    return value == "0";
                case 'X':
                    return value == "0" && !this.EmptyIsValue();

                case 'F':
                    return float.Parse(value.Replace("%", "")) == 0f;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether [is equal to field] [the specified field].
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsEqualToField(UPCRMField field)
        {
            if (field.FieldId != this.FieldId || (field.LinkId != this.LinkId && (field.LinkId > 0 || this.LinkId > 0))
                || field.InfoAreaId != this.InfoAreaId)
            {
                return false;
            }

            if (field.ParentLink == this.ParentLink)
            {
                return true;
            }

            if (field.ParentLink == null || this.ParentLink == null)
            {
                return false;
            }

            if (field.ParentLink.Key == this.ParentLink.Key)
            {
                return true;
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
            var parentLinkDescription = this.ParentLink.Key ?? string.Empty;

            return this.LinkId > 0
                       ? $"UPCRMField {parentLinkDescription}{this.InfoAreaId}:{this.LinkId}.{this.FieldId}"
                       : $"UPCRMField {parentLinkDescription}{this.InfoAreaId}.{this.FieldId}";
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
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueForRawValueOptions(string rawValue, UPFormatOption options)
        {
            return this.FieldInfo != null ? this.FieldInfo.ValueForRawValueOptions(rawValue, options, null) : null;
        }
    }

    /// <summary>
    /// CRM link field
    /// </summary>
    /// <seealso cref="UPCRMField" />
    public class UPCRMLinkField : UPCRMField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkField"/> class.
        /// </summary>
        /// <param name="linkInfoAreaId">
        /// The link information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public UPCRMLinkField(string linkInfoAreaId, int linkId, string infoAreaId)
            : base(-1, infoAreaId)
        {
            this.LinkInfoAreaId = linkInfoAreaId;
            this.LinkLinkId = linkId;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is link field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is link field; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLinkField => true;

        /// <summary>
        /// Gets the name of the link field.
        /// </summary>
        /// <value>
        /// The name of the link field.
        /// </value>
        public string LinkFieldName => $"LINK_{this.LinkInfoAreaId}_{Math.Max(this.LinkLinkId, 0)}";

        /// <summary>
        /// Gets the link information area identifier.
        /// </summary>
        /// <value>
        /// The link information area identifier.
        /// </value>
        public string LinkInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the link link identifier.
        /// </summary>
        /// <value>
        /// The link link identifier.
        /// </value>
        public int LinkLinkId { get; private set; }

        /// <summary>
        /// Fields the with link information area identifier link identifier information area identifier.
        /// </summary>
        /// <param name="linkInfoAreaId">
        /// The link information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMLinkField"/>.
        /// </returns>
        public static UPCRMLinkField FieldWithLinkInfoAreaIdLinkIdInfoAreaId(
            string linkInfoAreaId,
            int linkId,
            string infoAreaId)
        {
            return new UPCRMLinkField(linkInfoAreaId, linkId, infoAreaId);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.LinkInfoAreaId}.{this.LinkLinkId} {base.ToString()}";
        }
    }

    /// <summary>
    /// CRM field parent link
    /// </summary>
    public class UPCRMFieldParentLink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldParentLink"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public UPCRMFieldParentLink(string infoAreaId, int linkId)
            : this(infoAreaId, linkId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldParentLink"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="parentLink">
        /// The parent link.
        /// </param>
        public UPCRMFieldParentLink(string infoAreaId, int linkId, UPCRMFieldParentLink parentLink)
        {
            this.InfoAreaId = infoAreaId;
            this.LinkId = linkId;
            this.ParentLink = parentLink;
        }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the information area identifier with link.
        /// </summary>
        /// <value>
        /// The information area identifier with link.
        /// </value>
        public string InfoAreaIdWithLink
            => $"{this.ParentLink?.Key ?? string.Empty}{this.InfoAreaId.InfoAreaIdLinkId(this.LinkId)}";

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key => $"{this.ParentLink?.Key ?? string.Empty}{this.InfoAreaId.InfoAreaIdLinkId(this.LinkId)}>";

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets the parent link.
        /// </summary>
        /// <value>
        /// The parent link.
        /// </value>
        public UPCRMFieldParentLink ParentLink { get; private set; }

        /// <summary>
        /// Links from string.
        /// </summary>
        /// <param name="parentLinkString">
        /// The parent link string.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldParentLink"/>.
        /// </returns>
        public static UPCRMFieldParentLink LinkFromString(string parentLinkString)
        {
            var parts = parentLinkString.Split(':');
            return parts.Length > 1
                       ? new UPCRMFieldParentLink(parts[0], int.Parse(parts[1]))
                       : new UPCRMFieldParentLink(parentLinkString, -1);
        }

        /// <summary>
        /// Parents the link path.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMFieldParentLink> ParentLinkPath()
        {
            if (this.ParentLink == null)
            {
                return new List<UPCRMFieldParentLink> { this };
            }

            var pa = this.ParentLink.ParentLinkPath().ToList();
            pa.Add(this);
            return pa;
        }
    }
}
