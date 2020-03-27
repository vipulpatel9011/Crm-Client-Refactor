// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCharacteristicsItemGroup.cs" company="Aurea Software Gmbh">
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
//   The Documents Group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Characteristics Item Group Type enum
    /// </summary>
    public enum UPMCharacteristicsItemGroupType
    {
        /// <summary>
        /// Multi select
        /// </summary>
        MultiSelect = 1,

        /// <summary>
        /// Single select
        /// </summary>
        SingleSelect = 2
    }

    /// <summary>
    /// Characteristics Item Group class
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMContainer" />
    public class UPMCharacteristicsItemGroup : UPMContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMContainer"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMCharacteristicsItemGroup(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the type of the group.
        /// </summary>
        /// <value>
        /// The type of the group.
        /// </value>
        public UPMCharacteristicsItemGroupType GroupType { get; set; }

        /// <summary>
        /// Gets or sets the group name field.
        /// </summary>
        /// <value>
        /// The group name field.
        /// </value>
        public UPMStringField GroupNameField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show expanded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show expanded]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowExpanded { get; set; }
    }
}
