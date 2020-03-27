// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSerialEntryNumberEditField.cs" company="Aurea Software Gmbh">
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
//   The UPMSerialEntryNumberEditField.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.SerialEntry
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.SerialEntry;

    /// <summary>
    /// UPMSerialEntryNumberEditField
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Fields.Edit.UPMIntegerEditField" />
    /// <seealso cref="Aurea.CRM.UIModel.Fields.SerialEntry.UPMSerialEntryEditField" />
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPMSerialEntryStepableSize" />
    public class UPMSerialEntryNumberEditField : UPMIntegerEditField, UPMSerialEntryEditField, UPMSerialEntryStepableSize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSerialEntryNumberEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="column">The column.</param>
        /// <param name="position">The position.</param>
        public UPMSerialEntryNumberEditField(IIdentifier identifier, UPSEColumn column, UPMSEPosition position)
            : base(identifier)
        {
            this.SerialEntryColumn = column;
            this.SerialEntryPosition = position;
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
        /// Gets or sets a value indicating whether this instance has step size check.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has step size check; otherwise, <c>false</c>.
        /// </value>
        public bool HasStepSizeCheck { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has minimum maximum check.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has minimum maximum check; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinMaxCheck { get; set; }

        /// <summary>
        /// Gets or sets the size of the step.
        /// </summary>
        /// <value>
        /// The size of the step.
        /// </value>
        public int StepSize { get; set; }

        /// <summary>
        /// Gets or sets the minimum quantity.
        /// </summary>
        /// <value>
        /// The minimum quantity.
        /// </value>
        public int MinQuantity { get; set; }

        /// <summary>
        /// Gets or sets the maximum quantity.
        /// </summary>
        /// <value>
        /// The maximum quantity.
        /// </value>
        public int MaxQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [violates minimum maximum].
        /// </summary>
        /// <value>
        /// <c>true</c> if [violates minimum maximum]; otherwise, <c>false</c>.
        /// </value>
        public bool ViolatesMinMax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [violates step].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [violates step]; otherwise, <c>false</c>.
        /// </value>
        public bool ViolatesStep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [quantity step field].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [quantity step field]; otherwise, <c>false</c>.
        /// </value>
        public bool QuantityStepField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [supports decimals].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [supports decimals]; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsDecimals { get; set; }
    }
}
