// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MDateTimeEditField.cs" company="Aurea Software Gmbh">
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
//   UI control for editing a date and/or time field value
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Fields.Edit
{
    using System;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;

    /// <summary>
    /// UI control for editing a date and/or time field value
    /// </summary>
    /// <seealso cref="UPMEditField" />
    public class UPMDateTimeEditField : UPMEditField
    {
        /// <summary>
        /// The date time type.
        /// </summary>
        private DateTimeType dateTimeType;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDateTimeEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMDateTimeEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the date value.
        /// </summary>
        /// <value>
        /// The date value.
        /// </value>
        public DateTime? DateValue
        {
            get
            {
                return this.FieldValue as DateTime?;
            }

            set
            {
                this.FieldValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public DateTimeType Type
        {
            get
            {
                return this.dateTimeType;
            }

            set
            {
                this.dateTimeType = value;
                this.GUIDateTimeEditField?.ChangedType(value);
            }
        }

        /// <summary>
        /// Gets or sets the gui date time edit field.
        /// </summary>
        IGUIDateTimeEditField GUIDateTimeEditField { get; set; }
    }
}
