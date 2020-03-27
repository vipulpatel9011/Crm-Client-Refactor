// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MStringEditField.cs" company="Aurea Software Gmbh">
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
//   The type of the string edit field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;

    /// <summary>
    /// The type of the string edit field
    /// </summary>
    public enum StringEditFieldType
    {
        /// <summary>
        /// The plain.
        /// </summary>
        Plain = 0,

        /// <summary>
        /// The email.
        /// </summary>
        Email,

        /// <summary>
        /// The url.
        /// </summary>
        Url,

        /// <summary>
        /// The phone.
        /// </summary>
        Phone
    }

    /// <summary>
    /// UI control for editing a string field value
    /// </summary>
    /// <seealso cref="UPMEditField" />
    public class UPMStringEditField : UPMEditField
    {
        /// <summary>
        /// The _max length.
        /// </summary>
        private int maxLength = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMStringEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMStringEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the gui string edit field.
        /// </summary>
        public IGUIStringEditField GUIStringEditField { get; set; }

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
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public StringEditFieldType Type { get; set; }
    }
}
