// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSerialEntryStringEditField.cs" company="Aurea Software Gmbh">
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
//   The UPMSerialEntryStringEditField.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.SerialEntry
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.SerialEntry;

    /// <summary>
    /// UPMSerialEntryStringEditField
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Fields.Edit.UPMStringEditField" />
    /// <seealso cref="Aurea.CRM.UIModel.Fields.SerialEntry.UPMSerialEntryEditField" />
    public class UPMSerialEntryStringEditField : UPMStringEditField, UPMSerialEntryEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSerialEntryStringEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="column">The column.</param>
        /// <param name="position">The position.</param>
        public UPMSerialEntryStringEditField(IIdentifier identifier, UPSEColumn column, UPMSEPosition position)
            : base(identifier)
        {
            this.SerialEntryColumn = column;
            this.SerialEntryPosition = position;
        }

        /// <summary>
        /// Gets or sets the height of the multi line.
        /// </summary>
        /// <value>
        /// The height of the multi line.
        /// </value>
        public int MultiLineHeight { get; set; }

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
        /// Gets or sets a value indicating whether this instance is multiline.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multiline; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultiline { get; set; }
    }
}
