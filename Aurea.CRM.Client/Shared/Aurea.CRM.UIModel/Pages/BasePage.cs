// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePage.cs" company="Aurea Software Gmbh">
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
//   Base page implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#define PivotMode

namespace Aurea.CRM.UIModel.Pages
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Status;

    /// <summary>
    ///     Base page implementation
    /// </summary>
    /// <seealso cref="UPMContainer" />
    /// <seealso cref="ITopLevelElement" />
    public class Page : UPMContainer, ITopLevelElement, IPage
    {
        /// <summary>
        /// The _label text.
        /// </summary>
        private string labelText;

        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public Page(IIdentifier identifier)
            : base(identifier)
        {
            this.Groups = new List<UPMGroup>();
        }

        /// <summary>
        ///     Gets or sets the groups.
        /// </summary>
        /// <value>
        ///     The groups.
        /// </value>
        public List<UPMGroup> Groups { get; protected set; }

        /// <summary>
        /// Gets or sets the gui page.
        /// </summary>
        public IGUIPage GUIPage { get; set; }

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        public string LabelText
        {
            get
            {
                return this.labelText;
            }

            set
            {
                this.labelText = value;
                if (this.GUIPage != null)
                {
                    this.GUIPage.LabelText = value;
                }
            }
        }

        /// <summary>
        ///     Gets the number of groups.
        /// </summary>
        /// <value>
        ///     The number of groups.
        /// </value>
        public int NumberOfGroups => this.Groups?.Count ?? 0;

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>
        ///     The status.
        /// </value>
        public UPMStatus Status { get; set; }

        /// <summary>
        /// Adds the group.
        /// </summary>
        /// <param name="group">
        /// The group.
        /// </param>
        public virtual void AddGroup(UPMGroup group)
        {
            if (group == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(group.LabelText))
            {
                group.LabelText = "Overview";
            }

            if (this.Groups.Contains(group))
            {
                return;
            }

            this.Groups.Add(group);

            this.GUIPage?.ChildrenAdded(group);
        }

        /// <summary>
        /// Groups at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMGroup"/>.
        /// </returns>
        public virtual UPMGroup GroupAtIndex(int index)
        {
            return this.Children.Count > index ? this.Children[index] as UPMGroup : null;
        }

        /// <inheritdoc />
        public override UPMElement ChildWithIdentidier(IIdentifier identifier)
        {
            return this.Groups?.FirstOrDefault(element => element?.Identifier.MatchesIdentifier(identifier) ?? false);
        }
    }
}
