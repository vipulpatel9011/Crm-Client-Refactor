// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MOrganizerActionGroup.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm organizer action group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The upm organizer action group.
    /// </summary>
    public class UPMOrganizerActionGroup : UPMContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMOrganizerActionGroup"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMOrganizerActionGroup(IIdentifier identifier)
            : base(identifier)
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the icon name.
        /// </summary>
        public string IconName { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The add action.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public void AddAction(UPMOrganizerAction action)
        {
            this.AddChild(action);
        }
    }
}
