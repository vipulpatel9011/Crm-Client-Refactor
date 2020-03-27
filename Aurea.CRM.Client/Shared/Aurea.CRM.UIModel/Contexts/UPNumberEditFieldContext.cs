// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPNumberEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for a number field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// Edit field context for a number field
    /// </summary>
    /// <seealso cref="UPEditFieldContext" />
    public class UPNumberEditFieldContext : UPEditFieldContext
    {
        /// <summary>
        /// The has percent.
        /// </summary>
        protected bool hasPercent;

        /// <summary>
        /// The has fraction.
        /// </summary>
        protected bool hasFraction;

        /// <summary>
        /// The percent field.
        /// </summary>
        protected bool percentField;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPNumberEditFieldContext"/> class.
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
        public UPNumberEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPNumberEditFieldContext"/> class.
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
        public UPNumberEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPNumberEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPNumberEditFieldContext(int fieldId, string value)
            : base(fieldId, value)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value
            =>
                this.percentField
                    ? (((UPMNumberEditField)this.editField).NumberValue/100).ToString()
                    : this.hasFraction ? this.editField?.FieldValue?.ToString() : $"{this.editField?.FieldValue}";

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField()
        {
            var crmField = this.FieldConfig?.Field;
            this.percentField = crmField?.FieldInfo.PercentField ?? false;
            this.hasPercent = this.percentField;
            this.hasFraction = this.hasPercent || crmField?.FieldType[0] == 'F';

            UPMNumberEditField numberEditField = this.hasFraction
                                  ? new UPMFloatEditField(this.FieldIdentifier)
                                  : new UPMIntegerEditField(this.FieldIdentifier) as UPMNumberEditField;

            this.ApplyAttributesOnEditFieldConfig(numberEditField, this.FieldConfig);
            return numberEditField;
        }

        /// <summary>
        /// Applies the attributes on edit field configuration.
        /// </summary>
        /// <param name="_editField">
        /// The _edit field.
        /// </param>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        public override void ApplyAttributesOnEditFieldConfig(
            UPMEditField _editField,
            UPConfigFieldControlField fieldConfig)
        {
            base.ApplyAttributesOnEditFieldConfig(_editField, fieldConfig);
            ((UPMNumberEditField)_editField).ShowZero = fieldConfig?.Field?.FieldInfo?.ShowZero ?? false;
        }

#if PORTING
        static NSNumberFormatter numericFormatter;
        static NSNumberFormatter fractionFormatter;

        NSNumberFormatter NumericFormatter()
        {
            if (!numericFormatter)
            {
                numericFormatter = NSNumberFormatter.TheNew();
                numericFormatter.SetFormatterBehavior(NSNumberFormatterBehaviorDefault);
            }

            return numericFormatter;
        }


        NSNumberFormatter FractionFormatter()
        {
            if (!fractionFormatter)
            {
                fractionFormatter = NSNumberFormatter.TheNew();
                fractionFormatter.SetNumberStyle(NSNumberFormatterDecimalStyle);
                fractionFormatter.SetFormatterBehavior(NSNumberFormatterBehaviorDefault);
                fractionFormatter.SetDecimalSeparator(".");
                fractionFormatter.SetUsesGroupingSeparator(false);
            }

            return fractionFormatter;
        }
#endif

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
                ((UPMNumberEditField)this.editField).NumberValue = 0;
                return;
            }

            var factor = 1;
            if (this.hasPercent)
            {
                if (value.EndsWith("%"))
                {
                    value = value.Substring(0, value.Length - 1);
                }
                else
                {
                    this.hasPercent = false;
                    factor = 100;
                }
            }

            var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (value.EndsWith(decimalSeparator))
            {
                // Without this the user can never type a . to enter a decimal number
                return;
            }

            double numberValue;
            if (double.TryParse(value, out numberValue))
            {
                if (factor != 1)
                {
                    numberValue = numberValue * factor;
                }

                ((UPMNumberEditField)this.editField).NumberValue = numberValue;
            }
        }
    }
}
