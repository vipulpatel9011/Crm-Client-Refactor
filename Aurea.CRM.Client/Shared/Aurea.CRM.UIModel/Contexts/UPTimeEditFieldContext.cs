// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPTimeEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for Time field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Contexts
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// Edit field context for Time field
    /// </summary>
    /// <seealso cref="UPEditFieldContext" />
    public class UPTimeEditFieldContext : UPEditFieldContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPTimeEditFieldContext"/> class.
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
        public UPTimeEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPTimeEditFieldContext"/> class.
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
        public UPTimeEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPTimeEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field id.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPTimeEditFieldContext(int fieldId, string value)
            : base(fieldId, value)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value => this.DateValue.CrmValueFromTime();

        /// <summary>
        /// Gets the date value.
        /// </summary>
        /// <value>
        /// The date value.
        /// </value>
        private DateTime? DateValue => (this.editField as UPMDateTimeEditField)?.DateValue;

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField()
        {
            var field = new UPMDateTimeEditField(this.FieldIdentifier) { Type = DateTimeType.Time };
            this.ApplyAttributesOnEditFieldConfig(field, this.FieldConfig);
            return field;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var date = value.DateFromCrmValue();

            ((UPMDateTimeEditField)this.editField).DateValue = date;
        }
    }
}
