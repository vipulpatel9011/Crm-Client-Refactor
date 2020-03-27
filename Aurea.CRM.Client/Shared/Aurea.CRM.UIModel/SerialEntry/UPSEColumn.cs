// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEColumn.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   Serial Entry Column
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Column From enum
    /// </summary>
    public enum UPSEColumnFrom
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Source
        /// </summary>
        Source = 2,

        /// <summary>
        /// Source child
        /// </summary>
        SourceChild = 3,

        /// <summary>
        /// Destination
        /// </summary>
        Dest = 4,

        /// <summary>
        /// Destination child
        /// </summary>
        DestChild = 5,

        /// <summary>
        /// Additional source
        /// </summary>
        AdditionalSource = 6
    }

    /// <summary>
    /// Serial Entry Column class
    /// </summary>
    public class UPSEColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEColumn"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="index">The index.</param>
        /// <param name="positionInControl">The position in control.</param>
        public UPSEColumn(UPConfigFieldControlField fieldConfig, int index, int positionInControl)
        {
            this.FieldConfig = fieldConfig;
            this.CrmField = this.FieldConfig.Field;
            this.PositionInControl = positionInControl;
            this.Index = index;
            string functionName = fieldConfig.Function;
            if (!string.IsNullOrEmpty(functionName))
            {
                var parts = functionName.Split(';');
                if (parts.Length > 1)
                {
                    this.Function = parts[0];
                    this.InitialValue = parts[1];
                }
                else
                {
                    this.Function = fieldConfig.Function;
                }
            }

            this.SortInfo = new UPSEColumnSortInfo(this, fieldConfig);
            if (this.FieldConfig.Attributes.Hide)
            {
                this.Hidden = true;
            }

            if (this.FieldConfig.Attributes.Must)
            {
                this.Must = true;
            }

            this.ListingDestinationFunctionName = this.FieldConfig.Attributes.ExtendedOptions?["ListingTarget"];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEColumn"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="column">The column.</param>
        public UPSEColumn(UPConfigFieldControlField fieldConfig, UPSEColumn column)
        {
            this.FieldConfig = fieldConfig;
            this.CrmField = column.CrmField;
            this.PositionInControl = column.PositionInControl;
            this.Index = column.Index;
            this.Function = column.Function;
            this.InitialValue = column.InitialValue;
            this.SortInfo = column.SortInfo;
            this.Hidden = column.Hidden;
            this.ListingDestinationFunctionName = column.ListingDestinationFunctionName;
        }

        /// <summary>
        /// Gets or sets the initial value.
        /// </summary>
        /// <value>
        /// The initial value.
        /// </value>
        public string InitialValue { get; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId => this.FieldConfig.InfoAreaId;

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId => this.FieldConfig.FieldId;

        /// <summary>
        /// Gets the index of the parent column.
        /// </summary>
        /// <value>
        /// The index of the parent column.
        /// </value>
        public virtual int ParentColumnIndex => -1;

        /// <summary>
        /// Gets the column from.
        /// </summary>
        /// <value>
        /// The column from.
        /// </value>
        public virtual UPSEColumnFrom ColumnFrom => UPSEColumnFrom.Unknown;

        /// <summary>
        /// Gets the field configuration.
        /// </summary>
        /// <value>
        /// The field configuration.
        /// </value>
        public UPConfigFieldControlField FieldConfig { get; }

        /// <summary>
        /// Gets the position in control.
        /// </summary>
        /// <value>
        /// The position in control.
        /// </value>
        public int PositionInControl { get; }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; }

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        public string Function { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPSEColumn"/> is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        public bool Hidden { get; }

        /// <summary>
        /// Gets the CRM field.
        /// </summary>
        /// <value>
        /// The CRM field.
        /// </value>
        public UPCRMField CrmField { get; }

        /// <summary>
        /// Gets the sort information.
        /// </summary>
        /// <value>
        /// The sort information.
        /// </value>
        public UPSEColumnSortInfo SortInfo { get; }

        /// <summary>
        /// Gets the name of the listing destination function.
        /// </summary>
        /// <value>
        /// The name of the listing destination function.
        /// </value>
        public string ListingDestinationFunctionName { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPSEColumn"/> is must.
        /// </summary>
        /// <value>
        ///   <c>true</c> if must; otherwise, <c>false</c>.
        /// </value>
        public bool Must { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label => this.FieldConfig.Label;

        /// <summary>
        /// Strings the value from object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public string StringValueFromObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            //else if (this.NumberFormatter && obj.IsKindOfClass(typeof(NSNumber)))
            //{
            //    if (this.CrmField.FieldType == "F")
            //    {
            //        return NSString.StringWithFormat("%15.4f", ((NSNumber)obj).FloatValue());
            //    }
            //    else
            //    {
            //        return NSString.StringWithFormat("%ld", (long)((NSNumber)obj).IntegerValue());
            //    }

            //}
            //else
            {
                return (string)obj;
            }
        }

        /// <summary>
        /// Objects the value from string.
        /// </summary>
        /// <param name="theString">The string.</param>
        /// <returns></returns>
        public object ObjectValueFromString(string theString)
        {
            if (theString == null)
            {
                return null;
            }

            //if (this.NumberFormatter)
            //{
            //    NSNumber num = this.NumberFormatter.NumberFromString(theString);
            //    if (num.FloatValue() == 0)
            //    {
            //        return null;
            //    }
            //    else
            //    {
            //        return num;
            //    }

            //}
            return theString;
        }

        public object TheObjectValueFromObject(object theObject)
        {
            //if (theObject.IsKindOfClass(typeof(NSNumber)))
            //{
            //    return this.TheObjectValueFromNumber(theObject);
            //}

            //if (theObject.IsKindOfClass(typeof(string)))
            //{
            //    return this.TheObjectValueFromString(theObject);
            //}

            return theObject;
        }

        public object ObjectValueFromNumber(double number)
        {
            return number;
            //if (number == null)
            //{
            //    return null;
            //}
            //else if (this.NumberFormatter)
            //{
            //    if (number.FloatValue() == 0)
            //    {
            //        return null;
            //    }
            //    else
            //    {
            //        return number;
            //    }

            //}
            //else
            //{
            //    return NSString.StringWithFormat("%16.4f", number.FloatValue);
            //}
        }

        /// <summary>
        /// Determines whether [is value different than] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="origVal">The original value.</param>
        /// <returns>
        ///   <c>true</c> if [is value different than] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValueDifferentThan(string value, string origVal)
        {
            if (this.CrmField.FieldType == "F" || this.CrmField.FieldInfo.PercentField)
            {
                decimal numberValue = this.NumberFromValue(value);
                decimal origNumberValue = this.NumberFromValue(origVal);
                return numberValue != origNumberValue;
            }

            if (this.CrmField.FieldType == "B")
            {
                origVal = string.IsNullOrEmpty(origVal) ? "false" : origVal;
                value = string.IsNullOrEmpty(value) ? "false" : value;
                int oldValue = origVal == "true" ? 1 : (origVal == "false" ? 0 : Convert.ToInt32(origVal));
                int newValue = value == "true" ? 1 : (value == "false" ? 0 : Convert.ToInt32(value));
                return oldValue != newValue;
            }

            if ("XKSL".Contains(this.CrmField.FieldType))
            {
                return Convert.ToInt32(origVal) != Convert.ToInt32(value);
            }

            return (value != null || origVal != null) && origVal != value;
        }

        /// <summary>
        /// Determines whether [is empty value] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is empty value] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmptyValue(string value)
        {
            if (this.CrmField.FieldType == "F")
            {
                return Convert.ToDecimal(value, System.Globalization.CultureInfo.InvariantCulture) == 0;
            }

            if (this.CrmField.FieldType == "B")
            {
                return value == "false" || (Convert.ToInt32(value) == 0 && value != "true");
            }

            if ("XKSL".Contains(this.CrmField.FieldType))
            {
                return Convert.ToInt32(value) == 0;
            }

            return value.Length == 0;
        }

        /// <summary>
        /// Numbers from value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public decimal NumberFromValue(object obj)
        {
            return obj == null ? 0 : Convert.ToDecimal(obj, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
