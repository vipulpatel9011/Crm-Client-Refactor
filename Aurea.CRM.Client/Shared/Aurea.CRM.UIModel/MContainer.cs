// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MContainer.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//      Rashan Anushka
// </author>
// <summary>
//   The UPMContainer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;

    /// <summary>
    /// Defines the table cell styles
    /// </summary>
    public enum TableCellStyle
    {
        /// <summary>
        /// Row
        /// </summary>
        Row = 0,

        /// <summary>
        /// Row only
        /// </summary>
        RowOnly,

        /// <summary>
        /// Card
        /// </summary>
        Card,

        /// <summary>
        /// Card23
        /// </summary>
        Card23,

        /// <summary>
        /// Card2 only
        /// </summary>
        Card2Only,

        /// <summary>
        /// Classic
        /// </summary>
        Classic,
    }

    /// <summary>
    /// Implements a base UI container
    /// </summary>
    /// <seealso cref="UPMElement" />
    public class UPMContainer : UPMElement
    {
        /// <summary>
        /// Gets or sets the GUI container.
        /// </summary>
        /// <value>
        /// The GUI container.
        /// </value>
        public IGUIContainer GUIContainer { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<UPMElement> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMContainer"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMContainer(IIdentifier identifier)
            : base(identifier)
        {
            this.Children = new List<UPMElement>();
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="element">The element.</param>
        public virtual void AddChild(UPMElement element)
        {
            if (element != null)
            {
                element.Parent = this;
                this.Children.Add(element);
            }

            this.GUIContainer?.ChildrenAdded(element);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="element">The element.</param>
        public virtual void RemoveChild(UPMElement element)
        {
            if (element != null)
            {
                if (this.Children.Contains(element))
                {
                    this.Children.Remove(element);
                }
            }

            this.GUIContainer?.ChildrenRemoved(element);
        }

        /// <summary>
        /// Inserts the index of the child at.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="index">The index.</param>
        public void InsertChildAtIndex(UPMElement element, int index)
        {
            if (element == null)
            {
                return;
            }

            this.Children?.Insert(index, element);

            // element.Parent = this;

            this.GUIContainer?.ChildrenAddedAtIndex(element, index);
        }

        /// <summary>
        /// Indexes the of child.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public int IndexOfChild(UPMElement element) => this.Children?.IndexOf(element) ?? -1;

        /// <summary>
        /// Childs the with identidier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns></returns>
        public virtual UPMElement ChildWithIdentidier(IIdentifier identifier)
        {
            return this.Children?.FirstOrDefault(element => element?.Identifier.MatchesIdentifier(identifier) ?? false);
        }

        /// <summary>
        /// Removes all children.
        /// </summary>
        public void RemoveAllChildren()
        {
            this.Children?.Clear();
            this.GUIContainer?.ChildrenCleared();
        }

        /// <summary>
        /// Processes the changes applied identifiers.
        /// </summary>
        /// <param name="listOfIdentifiers">The list of identifiers.</param>
        /// <param name="appliedIdentifiers">The applied identifiers.</param>
        public override void ProcessChangesAppliedIdentifiers(List<IIdentifier> listOfIdentifiers, List<IIdentifier> appliedIdentifiers)
        {
            base.ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);
            foreach (var element in this.Children)
            {
                element.ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);
            }
        }
    }
}
