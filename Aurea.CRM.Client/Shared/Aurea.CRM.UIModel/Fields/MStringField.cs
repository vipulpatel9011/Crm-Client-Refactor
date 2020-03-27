// <copyright file="MStringField.cs" company="Aurea Software Gmbh">
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
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The UI control to show a string field value
    /// </summary>
    /// <seealso cref="UPMField" />
    public class UPMStringField : UPMField
    {
        private string stringValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMStringField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMStringField(IIdentifier identifier) : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is row field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is row field; otherwise, <c>false</c>.
        /// </value>
        public bool IsRowField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [strip new lines].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [strip new lines]; otherwise, <c>false</c>.
        /// </value>
        public bool StripNewLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [mulit line].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mulit line]; otherwise, <c>false</c>.
        /// </value>
        public bool MulitLine { get; set; }

        /// <summary>
        /// Gets or sets the string value.
        /// Used in MiniDetails to display in 2 Columns and more Lines
        /// </summary>
        /// <value>
        /// The string value.
        /// </value>
        public override string StringValue
        {
            get
            {
                return (string)this.FieldValue;
            }

            set
            {
                this.FieldValue = value;
                this.stringValue = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the multiline in grid maximum rows.
        /// </summary>
        /// <value>
        /// The multiline in grid maximum rows.
        /// </value>
        public int MultilineInGridMaxRows { get; set; }

        /// <summary>
        /// Gets or sets the raw string value.
        /// </summary>
        /// <value>
        /// The raw string value.
        /// </value>
        public string RawStringValue { get; set; }

        /// <summary>
        /// Strings the field with identifier value.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static UPMStringField StringFieldWithIdentifierValue(IIdentifier identifier, string value)
        {
            return StringFieldWithIdentifierValueLabel(identifier, value, null);
        }

        /// <summary>
        /// Strings the field with identifier value label.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        public static UPMStringField StringFieldWithIdentifierValueLabel(IIdentifier identifier, string value, string label)
        {
            return new UPMStringField(identifier)
            {
                LabelText = label,
                StringValue = value
            };
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPMField" /> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public new bool Empty => string.IsNullOrEmpty(this.FieldValue as string);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{base.ToString()} label: {this.LabelText} value: {this.StringValue}]";
        }
    }
}
