// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MResultRow.cs" company="Aurea Software Gmbh">
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
//   Implements the UI control for a serch result row
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Implements the UI control for a serch result row
    /// </summary>
    /// <seealso cref="UPMContainer" />
    public class UPMResultRow : UPMContainer
    {
        // , UPSwipePageRecord
        // , UPMiniDetailsResultRow
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMResultRow"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMResultRow(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public object Context { get; set; }

        /// <summary>
        /// Gets a value indicating whether [data available].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [data available]; otherwise, <c>false</c>.
        /// </value>
        public bool DataAvailable => this.Fields != null;

        /// <summary>
        /// Gets or sets a value indicating whether [data valid].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [data valid]; otherwise, <c>false</c>.
        /// </value>
        public bool DataValid { get; set; }

        /// <summary>
        /// Gets the detail actions.
        /// </summary>
        /// <value>
        /// The detail actions.
        /// </value>
        public List<UPMAction> DetailActions { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [detail actions are invalid].
        /// </summary>
        /// <value>
        /// <c>true</c> if [detail actions are invalid]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailActionsAreInvalid => this.DetailActions.Any(action => action.Invalid);

        /// <summary>
        /// Gets the detail groups.
        /// </summary>
        /// <value>
        /// The detail groups.
        /// </value>
        public List<UPMGroup> DetailGroups
        {
            get
            {
                return this.Children?.Select(c => c as UPMGroup).ToList();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [details available].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [details available]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailsAvailable => this.Children != null;

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public List<UPMField> Fields { get; set; }

        /// <summary>
        /// Gets a value indicating whether [groups are invalid].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [groups are invalid]; otherwise, <c>false</c>.
        /// </value>
        public bool GroupsAreInvalid
        {
            get
            {
                foreach (UPMGroup group in this.Children)
                {
                    if (group.Invalid)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public byte[] Icon { get; set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public virtual string Key => ((RecordIdentifier)this.Identifier).RecordIdentification;

        /// <summary>
        /// Numbers the of detail groups.
        /// </summary>
        /// <returns></returns>
        public int NumberOfDetailGroups => this.Children?.Count ?? 0;

        /// <summary>
        /// Gets or sets a value indicating whether [online data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [online data]; otherwise, <c>false</c>.
        /// </value>
        public bool OnlineData { get; set; }

        /// <summary>
        /// Gets or sets the record image document.
        /// </summary>
        /// <value>
        /// The record image document.
        /// </value>
        public UPMDocument RecordImageDocument { get; set; }

        /// <summary>
        /// Gets or sets the color of the row.
        /// </summary>
        /// <value>
        /// The color of the row.
        /// </value>
        public AureaColor RowColor { get; set; }

        /// <summary>
        /// Gets or sets the status indicator icon.
        /// </summary>
        /// <value>
        /// The status indicator icon.
        /// </value>
        public byte[] StatusIndicatorIcon { get; set; }

        /// <summary>
        /// Gets or sets the style identifier.
        /// </summary>
        /// <value>
        /// The style identifier.
        /// </value>
        public string StyleId { get; set; }

        /// <summary>
        /// The add detail action.
        /// </summary>
        /// <param name="detailAction">
        /// The detail action.
        /// </param>
        public void AddDetailAction(UPMOrganizerAction detailAction)
        {
            if (this.DetailActions == null)
            {
                this.DetailActions = new List<UPMAction>();
            }

            this.DetailActions.Add(detailAction);
        }

        /// <summary>
        /// Adds the detail group.
        /// </summary>
        /// <param name="detailGroup">
        /// The detail group.
        /// </param>
        public void AddDetailGroup(UPMGroup detailGroup)
        {
            this.AddChild(detailGroup);
        }

        /// <summary>
        /// Applies the updates from Result row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        public void ApplyUpdatesFromResultRow(UPMResultRow row)
        {
            if (row == this)
            {
                // do nothing as we assume that if we get the same object it HASN'T changed
                return;
            }

            if (row.Fields != this.Fields)
            {
                this.Fields = row.Fields;
            }

            if (row.DetailActions != this.DetailActions)
            {
                this.DetailActions = row.DetailActions;
            }

            if (row.Icon != this.Icon)
            {
                this.Icon = row.Icon;
            }

            if (row.RecordImageDocument != this.RecordImageDocument)
            {
                this.RecordImageDocument = row.RecordImageDocument;
            }

            if (row.StatusIndicatorIcon != this.StatusIndicatorIcon)
            {
                this.StatusIndicatorIcon = row.StatusIndicatorIcon;
            }

            this.OnlineData = row.OnlineData;
            this.RemoveAllChildren();
            foreach (UPMGroup group in row.Children)
            {
                this.AddChild(group);
            }

            this.RowColor = row.RowColor;
            this.Invalid = false;
        }

        /// <summary>
        /// Childrens for mini details.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPMGroup> ChildrenForMiniDetails() => this.Children.Cast<UPMGroup>().ToList();

        /// <summary>
        /// Details the actions for mini details.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPMAction> DetailActionsForMiniDetails() => this.DetailActions;

        /// <summary>
        /// Details the index of the group at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMGroup"/>.
        /// </returns>
        public UPMGroup DetailGroupAtIndex(int index)
        {
            return this.Children[index] as UPMGroup;
        }

        /// <summary>
        /// Fieldses for mini details.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPMField> FieldsForMiniDetails() => this.Fields;

        /// <summary>
        /// Identifiers for mini details.
        /// </summary>
        /// <returns>
        /// The <see cref="IIdentifier"/>.
        /// </returns>
        public IIdentifier IdentifierForMiniDetails() => this.Identifier;

        /// <summary>
        /// Called when [data for mini details].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool OnlineDataForMiniDetails() => this.OnlineData;

        /// <summary>
        /// Records the image document for mini details.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMDocument"/>.
        /// </returns>
        public UPMDocument RecordImageDocumentForMiniDetails() => this.RecordImageDocument;

        /// <summary>
        /// Rows the color for mini details.
        /// </summary>
        /// <returns>
        /// The <see cref="AureaColor"/>.
        /// </returns>
        public AureaColor RowColorForMiniDetails() => this.RowColor;

        /// <summary>
        /// Sets the invalid.
        /// </summary>
        /// <param name="invalid">
        /// if set to <c>true</c> [invalid].
        /// </param>
        public void SetInvalid(bool invalid)
        {
            // base.SetInvalid(invalid);
            if (invalid)
            {
                this.DataValid = false;
            }
        }

        /// <summary>
        /// Uniques the view identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string UniqueViewIdentifier()
        {
            var identifier = $"UPMResultRow_{this.Children.Count}";

            foreach (UPMGroup group in this.Children)
            {
                identifier = $"{identifier}_{group.NumberOfFields}";
            }

            return identifier;
        }
    }
}
