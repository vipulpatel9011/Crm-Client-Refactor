// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMListRow.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The PMiniDetailsResultRow interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// The PMiniDetailsResultRow interface.
    /// </summary>
    public interface UPMiniDetailsResultRow
    {
        /// <summary>
        /// Gets the children for mini details.
        /// </summary>
        List<UPMGroup> ChildrenForMiniDetails { get; }

        // List<UPMOrganizerAction> DetailActionsForMiniDetails { get; }
        /// <summary>
        /// Gets the detail actions for mini details.
        /// </summary>
        List<UPMAction> DetailActionsForMiniDetails { get; }

        /// <summary>
        /// Gets the fields for mini details.
        /// </summary>
        List<UPMField> FieldsForMiniDetails { get; }

        /// <summary>
        /// Gets the identifier for mini details.
        /// </summary>
        IIdentifier IdentifierForMiniDetails { get; }

        /// <summary>
        /// Gets a value indicating whether online data for mini details.
        /// </summary>
        bool OnlineDataForMiniDetails { get; }

        /// <summary>
        /// Gets the record image document for mini details.
        /// </summary>
        UPMDocument RecordImageDocumentForMiniDetails { get; }

        /// <summary>
        /// Gets the row color for mini details.
        /// </summary>
        AureaColor RowColorForMiniDetails { get; }
    }

    /// <summary>
    /// The upm list row.
    /// </summary>
    public class UPMListRow : UPMContainer, UPMiniDetailsResultRow
    {
        // public UIImage Icon { get; set; }    // CRM-5007

        /// <summary>
        /// The _row color.
        /// </summary>
        private AureaColor rowColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMListRow"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMListRow(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// The children for mini details.
        /// </summary>
        public List<UPMGroup> ChildrenForMiniDetails => this.DetailGroups;

        /// <summary>
        /// Gets the detail actions.
        /// </summary>
        public List<UPMOrganizerAction> DetailActions { get; private set; }

        /// <summary>
        /// Gets a value indicating whether detail actions are invalid.
        /// </summary>
        public bool DetailActionsAreInvalid { get; }

        /// <summary>
        /// The detail actions for mini details.
        /// </summary>
        public List<UPMAction> DetailActionsForMiniDetails => this.DetailActions.Cast<UPMAction>().ToList();

        /// <summary>
        /// Gets the detail groups.
        /// </summary>
        public List<UPMGroup> DetailGroups { get; private set; }

        /// <summary>
        /// The fields.
        /// </summary>
        public List<UPMField> Fields => this.Children.Cast<UPMField>().ToList();

        /// <summary>
        /// Gets the fields for mini details.
        /// </summary>
        public List<UPMField> FieldsForMiniDetails
        {
            get
            {
                var miniFields = new List<UPMElement>();

                foreach (UPMGroup vGroup in this.DetailGroups)
                {
                    miniFields.AddRange(vGroup.Fields);
                }

                return this.Fields;
            }
        }

        /// <summary>
        /// Gets or sets the gui list row.
        /// </summary>
        public IGUIListRow GUIListRow { get; set; }

        // UPMiniDetailsResultRow Implementation
        /// <summary>
        /// The identifier for mini details.
        /// </summary>
        public IIdentifier IdentifierForMiniDetails => this.Identifier;

        /// <summary>
        /// The key.
        /// </summary>
        public string Key => (this.Identifier as RecordIdentifier)?.RecordIdentification;

        /// <summary>
        /// The number of fields.
        /// </summary>
        public int NumberOfFields => this.Children.Count;

        /// <summary>
        /// Gets or sets a value indicating whether online data.
        /// </summary>
        public bool OnlineData { get; set; }

        /// <summary>
        /// The online data for mini details.
        /// </summary>
        public bool OnlineDataForMiniDetails => this.OnlineData;

        /// <summary>
        /// Gets or sets the record image document.
        /// </summary>
        public UPMDocument RecordImageDocument { get; set; }

        /// <summary>
        /// The record image document for mini details.
        /// </summary>
        public UPMDocument RecordImageDocumentForMiniDetails => this.RecordImageDocument;

        /// <summary>
        /// Gets or sets the row action.
        /// </summary>
        public UPMAction RowAction { get; set; }

        /// <summary>
        /// Gets or sets the row color.
        /// </summary>
        public AureaColor RowColor
        {
            get
            {
                return this.rowColor;
            }

            set
            {
                this.rowColor = value;
                if (this.GUIListRow != null)
                {
                    this.GUIListRow.RowColor = value;
                }
            }
        }

        /// <summary>
        /// The row color for mini details.
        /// </summary>
        public AureaColor RowColorForMiniDetails => this.RowColor;

        /// <summary>
        /// The unique view identifier.
        /// </summary>
        public string UniqueViewIdentifier => $"UPMListRow_{this.Children.Count}";

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
                this.DetailActions = new List<UPMOrganizerAction>();
            }

            this.DetailActions.Add(detailAction);
        }

        /// <summary>
        /// The add detail group.
        /// </summary>
        /// <param name="detailGroup">
        /// The detail group.
        /// </param>
        public void AddDetailGroup(UPMGroup detailGroup)
        {
            if (this.DetailGroups == null)
            {
                this.DetailGroups = new List<UPMGroup>();
            }

            this.DetailGroups.Add(detailGroup);
        }

        /// <summary>
        /// The add field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        public void AddField(UPMField field)
        {
            this.AddChild(field);
        }

        /// <summary>
        /// The field at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMField"/>.
        /// </returns>
        public UPMField FieldAtIndex(int index)
        {
            return this.Children[index] as UPMField;
        }
    }
}
