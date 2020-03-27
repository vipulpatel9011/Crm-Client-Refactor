// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMGroup.cs" company="Aurea">
// Copyright (c) 2016 Aurea. All rights reserved.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Implements a field group
    /// </summary>
    /// <seealso cref="UPMContainer" />
    public class UPMGroup : UPMContainer
    {
        private string labelText;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMGroup(IIdentifier identifier)
            : base(identifier)
        {
            this.Deletable = true;
            this.Actions = new List<object>();
        }

        /// <summary>
        /// The GUI group
        /// </summary>
        public IGUIGroup GUIGroup;

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public virtual List<UPMField> Fields => this.Children?.Where(c => c != null).Cast<UPMField>().ToList();

        /// <summary>
        /// Gets the number of fields.
        /// </summary>
        /// <value>
        /// The number of fields.
        /// </value>
        public int NumberOfFields => this.Fields?.Count ?? 0;

        /// <summary>
        /// Gets or sets the configured postion of group.
        /// Postion of group should be tab index or index of form item
        /// </summary>
        /// <value>
        /// The configured postion of group.
        /// </value>
        public int ConfiguredPostionOfGroup { get; set; }

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        /// <value>
        /// The label text.
        /// </value>
        public string LabelText
        {
            get
            {
                return this.labelText;
            }

            set
            {
                this.labelText = value;
                if (this.GUIGroup != null)
                {
                    this.GUIGroup.LabelText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the header image.
        /// </summary>
        /// <value>
        /// The name of the header image.
        /// </value>
        public string HeaderImageName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMGroup"/> is deletable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if deletable; otherwise, <c>false</c>.
        /// </value>
        public bool Deletable { get; set; }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        public List<object> Actions { get; }

        /// <summary>
        /// Fields at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMField FieldAtIndex(int index)
        {
            if (this.Fields == null || this.Children.Count < index)
            {
                return null;
            }

            return this.Fields[index];
        }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="field">The field.</param>
        public virtual void AddField(UPMField field)
        {
            this.AddChild(field);
            this.GUIContainer?.ChildrenAdded(field);
        }

        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void AddAction(UPMOrganizerAction action)
        {
            this.Actions.Add(action);
        }

        /// <summary>
        /// Applies the updates from group.
        /// </summary>
        /// <param name="group">The group.</param>
        public void ApplyUpdatesFromGroup(UPMGroup group)
        {
            if (group == this)
            {
                // do nothing as we assume that if we get the same object it HASN'T changed
                return;
            }

            if (group?.Fields == null)
            {
                return;
            }

            foreach (UPMField field in group.Fields)
            {
                this.AddField(field);
            }

            this.Invalid = group.Invalid;
        }
    }
}
