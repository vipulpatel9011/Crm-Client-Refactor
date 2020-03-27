// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MMultiLineEditField.cs" company="Aurea Software Gmbh">
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
//   UI control for editing a multi line value field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;

    /// <summary>
    /// UI control for editing a multi line value field
    /// </summary>
    /// <seealso cref="UPMEditField" />
    public class UPMMultilineEditField : UPMEditField
    {
        /// <summary>
        /// The _max length.
        /// </summary>
        private int maxLength = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMMultilineEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMMultilineEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPMMultilineEditField"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public override bool Empty => string.IsNullOrEmpty(this.StringValue);

        /// <summary>
        /// Gets or sets the gui string edit field.
        /// </summary>
        public IGUIStringEditField GUIStringEditField { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        /// <c>true</c> if HTML; otherwise, <c>false</c>.
        /// </value>
        public bool Html { get; set; }

        public int RowSpan { get; set; }

        public object MultiLine { get; set; }

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int MaxLength
        {
            get
            {
                return this.maxLength;
            }

            set
            {
                this.maxLength = value;
                if (this.GUIStringEditField != null)
                {
                    this.GUIStringEditField.MaxLength = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no label].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no label]; otherwise, <c>false</c>.
        /// </value>
        public bool NoLabel { get; set; }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <value>
        /// The string value.
        /// </value>
        public override string StringValue => this.FieldValue as string;
    }
}
