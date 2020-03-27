// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmFieldValue.cs" company="Aurea Software Gmbh">
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
//   CRM field value
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;

    using Utilities;

    /// <summary>
    /// CRM field value
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Utilities.UPSerializable" />
    public class UPCRMFieldValue : UPSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldValue"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="onlyOffline">
        /// if set to <c>true</c> [only offline].
        /// </param>
        public UPCRMFieldValue(string value, string oldValue, string infoAreaId, int fieldId, bool onlyOffline)
        {
            this.InfoAreaId = infoAreaId;
            this.FieldId = fieldId;
            this.ChangeValue = value;
            this.OldValue = oldValue;
            this.OnlyOffline = onlyOffline;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldValue"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="onlyOffline">
        /// if set to <c>true</c> [only offline].
        /// </param>
        public UPCRMFieldValue(string value, string infoAreaId, int fieldId, bool onlyOffline)
            : this(value, null, infoAreaId, fieldId, onlyOffline)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldValue"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        public UPCRMFieldValue(string value, string oldValue, string infoAreaId, int fieldId)
            : this(value, oldValue, infoAreaId, fieldId, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldValue"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="dateTimeValue">
        /// The date time value.
        /// </param>
        /// <param name="oldDateValue">
        /// The old date value.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="onlyOffline">
        /// if set to <c>true</c> [only offline].
        /// </param>
        /// <param name="isTime">
        /// if set to <c>true</c> [is time].
        /// </param>
        public UPCRMFieldValue(
            string value,
            string dateTimeValue,
            string oldDateValue,
            string infoAreaId,
            int fieldId,
            bool onlyOffline,
            bool isTime)
            : this(value, oldDateValue, infoAreaId, fieldId, onlyOffline)
        {
            if (isTime)
            {
                this.TimeOriginalValue = dateTimeValue;
            }
            else
            {
                this.DateOriginalValue = dateTimeValue;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFieldValue"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        public UPCRMFieldValue(string value, string infoAreaId, int fieldId)
            : this(value, infoAreaId, fieldId, false)
        {
        }

        /// <summary>
        /// Gets or sets the change value.
        /// </summary>
        /// <value>
        /// The change value.
        /// </value>
        public string ChangeValue { get; set; }

        /// <summary>
        /// Gets or sets the date original value.
        /// </summary>
        /// <value>
        /// The date original value.
        /// </value>
        public string DateOriginalValue { get; set; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the integer value.
        /// </summary>
        /// <value>
        /// The integer value.
        /// </value>
        public int IntegerValue => int.Parse(this.ChangeValue);

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>
        /// The old value.
        /// </value>
        public string OldValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether [only offline].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [only offline]; otherwise, <c>false</c>.
        /// </value>
        public bool OnlyOffline { get; private set; }

        /// <summary>
        /// Gets the string field identifier.
        /// </summary>
        /// <value>
        /// The string field identifier.
        /// </value>
        public string StringFieldId => $"{this.FieldId}";

        /// <summary>
        /// Gets or sets the time original value.
        /// </summary>
        /// <value>
        /// The time original value.
        /// </value>
        public string TimeOriginalValue { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value => this.ChangeValue;

        /// <summary>
        /// Changes the value old value information area identifier field identifier.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldValue"/>.
        /// </returns>
        public static UPCRMFieldValue ChangeValueOldValueInfoAreaIdFieldId(
            string value,
            string oldValue,
            string infoAreaId,
            int fieldId)
        {
            return new UPCRMFieldValue(value, oldValue, infoAreaId, fieldId);
        }

        /// <summary>
        /// Changes the value old value information area identifier field identifier only offline.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="onlyOffline">
        /// if set to <c>true</c> [only offline].
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldValue"/>.
        /// </returns>
        public static UPCRMFieldValue ChangeValueOldValueInfoAreaIdFieldIdOnlyOffline(
            string value,
            string oldValue,
            string infoAreaId,
            int fieldId,
            bool onlyOffline)
        {
            return new UPCRMFieldValue(value, oldValue, infoAreaId, fieldId, onlyOffline);
        }

        /// <summary>
        /// Values the information area identifier field identifier.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldValue"/>.
        /// </returns>
        public static UPCRMFieldValue ValueInfoAreaIdFieldId(string value, string infoAreaId, int fieldId)
        {
            return new UPCRMFieldValue(value, infoAreaId, fieldId);
        }

        /// <summary>
        /// ChangeValueOldValue
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="oldValue">oldValue</param>
        /// <param name="onlyOffline">onlyOffline</param>
        /// <returns>bool</returns>
        public bool ChangeValueOldValue(string value, string oldValue, bool onlyOffline = false)
        {
            this.OnlyOffline = onlyOffline;
            if (!this.ChangeValue.Equals(oldValue))
            {
                return false;
            }

            this.ChangeValue = value;
            return true;
        }

        /// <summary>
        /// Jsons this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Json()
        {
            if (this.OnlyOffline || this.ChangeValue == null || this.ChangeValue.Equals(this.OldValue))
            {
                return null;
            }

            // return new List<object> { FieldId, null, null };
            return new List<object> { this.FieldId, this.ChangeValue, this.OldValue };
        }

        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public void Serialize(UPSerializer writer)
        {
            writer.WriteElementStart("Value");
            writer.WriteAttributeIntegerValue("fieldId", this.FieldId);
            writer.WriteElementValue("New", this.Value);
            if (!string.IsNullOrEmpty(this.OldValue))
            {
                writer.WriteElementValue("Old", this.OldValue);
            }

            writer.WriteElementEnd();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.OnlyOffline
                       ? $"{this.InfoAreaId}.{this.FieldId}: {this.ChangeValue} (old:{this.OldValue}) OFFLINE"
                       : $"{this.InfoAreaId}.{this.FieldId}: {this.ChangeValue} (old:{this.OldValue})";
        }

        /// <summary>
        /// Values the is equal to string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValueIsEqualToString(string value)
        {
            if (this.ChangeValue != null)
            {
                return !string.IsNullOrEmpty(value) && value.Equals(this.ChangeValue);
            }

            return string.IsNullOrEmpty(value);
        }
    }
}
