// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMStandardEditGroup.cs" company="Aurea Software Gmbh">
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
//   Implements the standard edit group UI control
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System.Linq;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// Implements the standard edit group UI control
    /// </summary>
    /// <seealso cref="UPMGroup" />
    public class UPMStandardEditGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMStandardEditGroup"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMStandardEditGroup(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the gap space.
        /// </summary>
        /// <value>
        /// The gap space.
        /// </value>
        public double GapSpace { get; set; }

        /// <summary>
        /// Gets or sets the group delegate.
        /// </summary>
        /// <value>
        /// The group delegate.
        /// </value>
        public object GroupDelegate { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has changes.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        public bool HasChanges
        {
            get { return this.Fields.Cast<UPMEditField>().Any(editField => editField.Changed); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [header style].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [header style]; otherwise, <c>false</c>.
        /// </value>
        public bool HeaderStyle { get; set; }

        /// <summary>
        /// Gets or sets the label space.
        /// </summary>
        /// <value>
        /// The label space.
        /// </value>
        public double LabelSpace { get; set; }

        /// <summary>
        /// Gets or sets the value space.
        /// </summary>
        /// <value>
        /// The value space.
        /// </value>
        public double ValueSpace { get; set; }
    }
}
