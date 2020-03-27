// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSerialEntryStepableSize.cs" company="Aurea Software Gmbh">
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
//   UPMSerialEntryStepableSize
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    /// <summary>
    /// UPMSerialEntryStepableSize
    /// </summary>
    public interface UPMSerialEntryStepableSize
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has step size check.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has step size check; otherwise, <c>false</c>.
        /// </value>
        bool HasStepSizeCheck { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has minimum maximum check.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has minimum maximum check; otherwise, <c>false</c>.
        /// </value>
        bool HasMinMaxCheck { get; set; }

        /// <summary>
        /// Gets or sets the size of the step.
        /// </summary>
        /// <value>
        /// The size of the step.
        /// </value>
        int StepSize { get; set; }

        /// <summary>
        /// Gets or sets the minimum quantity.
        /// </summary>
        /// <value>
        /// The minimum quantity.
        /// </value>
        int MinQuantity { get; set; }

        /// <summary>
        /// Gets or sets the maximum quantity.
        /// </summary>
        /// <value>
        /// The maximum quantity.
        /// </value>
        int MaxQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [violates minimum maximum].
        /// </summary>
        /// <value>
        /// <c>true</c> if [violates minimum maximum]; otherwise, <c>false</c>.
        /// </value>
        bool ViolatesMinMax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [violates step].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [violates step]; otherwise, <c>false</c>.
        /// </value>
        bool ViolatesStep { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [quantity step field].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [quantity step field]; otherwise, <c>false</c>.
        /// </value>
        bool QuantityStepField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [supports decimals].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [supports decimals]; otherwise, <c>false</c>.
        /// </value>
        bool SupportsDecimals { get; set; }
    }
}
