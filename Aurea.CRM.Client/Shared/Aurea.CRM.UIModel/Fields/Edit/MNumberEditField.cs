// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MNumberEditField.cs" company="Aurea Software Gmbh">
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
//   UI control for editing a number value field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Core.CRM.DataModel.FieldValueFormatters;

    /// <summary>
    /// UI control for editing a number value field
    /// </summary>
    /// <seealso cref="UPMEditField" />
    public class UPMNumberEditField : UPMEditField
    {
        /// <summary>
        /// The display format.
        /// </summary>
        protected string displayFormat;

        /// <summary>
        /// The edit formatter.
        /// </summary>
        protected string editFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMNumberEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMNumberEditField(IIdentifier identifier)
            : this(identifier, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMNumberEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="displayFromat">
        /// The display fromat.
        /// </param>
        /// <param name="editFormatter">
        /// The edit formatter.
        /// </param>
        public UPMNumberEditField(IIdentifier identifier, string displayFromat, string editFormatter)
            : base(identifier)
        {
            this.displayFormat = displayFromat;
            this.editFormatter = editFormatter;
        }

        /// <summary>
        /// Gets or sets the number value.
        /// </summary>
        /// <value>
        /// The number value.
        /// </value>
        public virtual double NumberValue
        {
            get
            {
                double numberValue = 0;
                if (this.FieldValue is double)
                {
                    numberValue = (double?)this.FieldValue ?? 0.0;
                }
                else if (this.FieldValue != null)
                {
                    double.TryParse(this.FieldValue.ToString(), out numberValue);
                }

                return numberValue;
            }

            set
            {
                this.FieldValue = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show zero].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show zero]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowZero { get; set; }

        /// <summary>
        /// Gets the string display value.
        /// </summary>
        /// <value>
        /// The string display value.
        /// </value>
        public override string StringDisplayValue
            =>
                !this.ShowZero && (this.FieldValue == null || this.NumberValue < double.Epsilon)
                    ? string.Empty
                    : this.FieldValue?.ToString();

        /// <summary>
        /// Gets the string edit value.
        /// </summary>
        /// <value>
        /// The string edit value.
        /// </value>
        public override string StringEditValue => this.StringDisplayValue;

        /// <summary>
        /// Determines whether [is valid edit string] [the specified the string].
        /// </summary>
        /// <param name="theString">
        /// The string.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsValidEditString(string theString)
        {
            if (string.IsNullOrEmpty(theString))
            {
                return true;
            }

            double temp;
            return double.TryParse(theString, out temp);
        }

        /// <summary>
        /// Numbers from string value.
        /// </summary>
        /// <param name="numberAsString">
        /// The number as string.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public virtual double? NumberFromStringValue(string numberAsString)
        {
            double result;
            bool parseSuccessful = double.TryParse(numberAsString, out result);
            return parseSuccessful ? result : (double?)null;
        }

        /// <summary>
        /// Sets the number from string value.
        /// </summary>
        /// <param name="numberAsString">
        /// The number as string.
        /// </param>
        public virtual void SetNumberFromStringValue(string numberAsString)
        {
            this.FieldValue = this.NumberFromStringValue(numberAsString);
            var editFieldContext = this.EditFieldContext as UPEditFieldContext;
            editFieldContext?.SetValue(numberAsString);
        }
    }
}
