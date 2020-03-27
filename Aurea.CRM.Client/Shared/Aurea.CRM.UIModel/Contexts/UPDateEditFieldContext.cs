// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPDateEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for date field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// Edit field context for date field
    /// </summary>
    /// <seealso cref="UPEditFieldContext" />
    public class UPDateEditFieldContext : UPEditFieldContext
    {
        /// <summary>
        /// The child time context.
        /// </summary>
        private UPChildTimeEditFieldContext childTimeContext;

        /// <summary>
        /// The explicit time value.
        /// </summary>
        private string explicitTimeValue;

        /// <summary>
        /// The original child value assigned.
        /// </summary>
        private bool originalChildValueAssigned;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPDateEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="fieldIdentifier">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="childFields">
        /// The child fields.
        /// </param>
        public UPDateEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPDateEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="fieldIdentifier">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPDateEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPDateEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPDateEditFieldContext(int fieldId, string value)
            : base(fieldId, value)
        {
        }

        /// <summary>
        /// Gets the date value.
        /// </summary>
        /// <value>
        /// The date value.
        /// </value>
        public DateTime? DateValue => (this.editField as UPMDateTimeEditField)?.DateValue;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value
            => this.DateValue == DateTime.MinValue ? string.Empty : StringExtensions.CrmValueFromDate(this.DateValue);

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField()
        {
            var field = new UPMDateTimeEditField(this.FieldIdentifier);
            if (this.ChildFields != null && this.ChildFields.Any())
            {
                var context = this.ChildFields[0];
                if (context is UPChildTimeEditFieldContext)
                {
                    this.childTimeContext = (UPChildTimeEditFieldContext)context;
                }
            }

            field.Type = this.childTimeContext != null ? DateTimeType.DateTime : DateTimeType.Date;
            this.ApplyAttributesOnEditFieldConfig(field, this.FieldConfig);
            return field;
        }

        /// <summary>
        /// Sets the time value.
        /// </summary>
        /// <param name="timeValue">
        /// The time value.
        /// </param>
        public void SetTimeValue(string timeValue)
        {
            if (this.childTimeContext != null)
            {
                this.childTimeContext.SetValue(timeValue);
                var date = $"{this.Value} {timeValue}".DateTimeFromCrmValue();
                ((UPMDateTimeEditField)this.editField).DateValue = date;
            }

            this.explicitTimeValue = timeValue;
            this.originalChildValueAssigned = false;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(string value)
        {
            DateTime? date;
            if (this.childTimeContext != null)
            {
                string childTimeValue;
                if (this.editField != null && this.originalChildValueAssigned)
                {
                    childTimeValue = this.DateValue.CrmValueFromTime();
                }
                else
                {
                    childTimeValue = !string.IsNullOrEmpty(this.explicitTimeValue)
                        ? this.explicitTimeValue
                        : this.childTimeContext.OriginalValue;
                    this.explicitTimeValue = this.childTimeContext.OriginalValue;
                    this.originalChildValueAssigned = true;
                }

                var dateString = !string.IsNullOrEmpty(childTimeValue)
                    ? $"{value} {childTimeValue}"
                    : value;
                date =
                    UPCRMTimeZone.Current.ClientDataDateFormatter.DateFromString(
                        dateString.DateFromCrmValue().ToString());
            }
            else
            {
                date = value.DateFromCrmValue();
            }

            (this.editField as UPMDateTimeEditField).DateValue = date;
            this.SetChanged(true);
        }
    }
}
