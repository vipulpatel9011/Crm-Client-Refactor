// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSerialEntryDateTimeEditField.cs" company="Aurea Software Gmbh">
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
//   The UPMSerialEntryDateTimeEditField.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.SerialEntry
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.SerialEntry;

    /// <summary>
    /// UPMSerialEntryDateTimeEditField
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Fields.Edit.UPMDateTimeEditField" />
    /// <seealso cref="Aurea.CRM.UIModel.Fields.SerialEntry.UPMSerialEntryEditField" />
    public class UPMSerialEntryDateTimeEditField : UPMDateTimeEditField, UPMSerialEntryEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSerialEntryDateTimeEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="column">The column.</param>
        /// <param name="position">The position.</param>
        /// <param name="dateType">Type of the date.</param>
        public UPMSerialEntryDateTimeEditField(IIdentifier identifier, UPSEColumn column, UPMSEPosition position, DateTimeType dateType)
            : base(identifier)
        {
            this.SerialEntryColumn = column;
            this.SerialEntryPosition = position;
            this.Type = dateType;
        }

        /// <summary>
        /// Gets the serial entry column.
        /// </summary>
        /// <value>
        /// The serial entry column.
        /// </value>
        public UPSEColumn SerialEntryColumn { get; }

        /// <summary>
        /// Gets the serial entry position.
        /// </summary>
        /// <value>
        /// The serial entry position.
        /// </value>
        public UPMSEPosition SerialEntryPosition { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [new line].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [new line]; otherwise, <c>false</c>.
        /// </value>
        public bool NewLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [one column].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [one column]; otherwise, <c>false</c>.
        /// </value>
        public bool OneColumn { get; set; }

        /// <summary>
        /// Gets the string edit value.
        /// </summary>
        /// <value>
        /// The string edit value.
        /// </value>
        public override string StringEditValue => this.StringDisplayValue;

        /// <summary>
        /// Gets the string display value.
        /// </summary>
        /// <value>
        /// The string display value.
        /// </value>
        public override string StringDisplayValue
        {
            get
            {
                switch (this.Type)
                {
                    case DateTimeType.Date:
                        return this.DateValue.LongLocalizedFormattedDate();

                    case DateTimeType.Time:
                        return this.DateValue.LocalizedFormattedTime();

                    default:
                        return string.Empty;
                }
            }
        }
    }
}
