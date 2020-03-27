// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMObjective.cs" company="Aurea Software Gmbh">
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
//   The UPMObjective class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Objectives
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// UPMObjective
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMObjective : UPMElement
    {
        private List<UPMField> fields;
        private List<UPMField> mainFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMObjective"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMObjective(IIdentifier identifier)
            : base(identifier)
        {
            this.fields = new List<UPMField>();
            this.mainFields = new List<UPMField>();
            this.HasDocuments = false;
            this.ObjectiveItem = null;
        }

        /// <summary>
        /// Gets the readonly fields.
        /// </summary>
        /// <value>
        /// The readonly fields.
        /// </value>
        public List<UPMStringField> ReadonlyFields
        {
            get
            {
                List<UPMStringField> readonlyFields = new List<UPMStringField>(this.fields.Count);
                foreach (UPMField field in this.fields)
                {
                    if (field is UPMNumberEditField)
                    {
                        UPMNumberEditField numberEditField = (UPMNumberEditField)field;
                        readonlyFields.Add(UPMStringField.StringFieldWithIdentifierValueLabel(numberEditField.Identifier, numberEditField.StringDisplayValue, numberEditField.LabelText));
                    }
                    else if (field is UPMMultilineEditField)
                    {
                        UPMMultilineEditField stringEditField = (UPMMultilineEditField)field;
                        readonlyFields.Add(UPMStringField.StringFieldWithIdentifierValueLabel(stringEditField.Identifier, stringEditField.StringValue, stringEditField.LabelText));
                    }
                    else if (field is UPMCatalogEditField)
                    {
                        UPMCatalogEditField catalogEditField = (UPMCatalogEditField)field;
                        readonlyFields.Add(UPMStringField.StringFieldWithIdentifierValueLabel(catalogEditField.Identifier, catalogEditField.StringDisplayValue, catalogEditField.LabelText));
                    }
                    else if (field is UPMDateTimeEditField)
                    {
                        UPMDateTimeEditField dateEditField = (UPMDateTimeEditField)field;
                        readonlyFields.Add(UPMStringField.StringFieldWithIdentifierValueLabel(dateEditField.Identifier, dateEditField.DateValue.LocalizedFormattedDate(), dateEditField.LabelText));
                    }
                    else if (field is UPMBooleanEditField)
                    {
                        UPMBooleanEditField booleanEditField = (UPMBooleanEditField)field;
                        readonlyFields.Add(UPMStringField.StringFieldWithIdentifierValueLabel(booleanEditField.Identifier, booleanEditField.BoolValue ? LocalizedString.TextYes : LocalizedString.TextNo, booleanEditField.LabelText));
                    }
                    else if (field is UPMStringEditField)
                    {
                        UPMStringEditField stringEditField = (UPMStringEditField)field;
                        readonlyFields.Add(UPMStringField.StringFieldWithIdentifierValueLabel(stringEditField.Identifier, stringEditField.StringValue, stringEditField.LabelText));
                    }
                    else
                    {
                        var stringField = field as UPMStringField;
                        if (stringField != null)
                        {
                            readonlyFields.Add(stringField);
                        }
                    }
                }

                return readonlyFields;
            }
        }

        /// <summary>
        /// Gets the main fields.
        /// </summary>
        /// <value>
        /// The main fields.
        /// </value>
        public List<UPMField> MainFields => this.mainFields;

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public List<UPMField> Fields => this.fields;

        /// <summary>
        /// Gets or sets the done field.
        /// </summary>
        /// <value>
        /// The done field.
        /// </value>
        public UPMBooleanEditField DoneField { get; set; }

        /// <summary>
        /// Gets or sets the can be deleted field.
        /// </summary>
        /// <value>
        /// The can be deleted field.
        /// </value>
        public UPMBooleanEditField CanBeDeletedField { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has documents.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has documents; otherwise, <c>false</c>.
        /// </value>
        public bool HasDocuments { get; private set; }

        /// <summary>
        /// Gets or sets the objective item.
        /// </summary>
        /// <value>
        /// The objective item.
        /// </value>
        public UPObjectivesItem ObjectiveItem { get; set; }

        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        public List<UPMOrganizerAction> Actions { get; set; }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="field">The field.</param>
        public void AddField(UPMField field)
        {
            this.fields.Add(field);
        }

        /// <summary>
        /// Adds the main field.
        /// </summary>
        /// <param name="field">The field.</param>
        public void AddMainField(UPMField field)
        {
            this.mainFields.Add(field);
        }

        /// <summary>
        /// Adds the group.
        /// </summary>
        /// <param name="group">The group.</param>
        public void AddGroup(UPMDocumentsGroup group)
        {
            this.HasDocuments = group.Children.Count > 0;
            this.fields.AddRange(group.Fields);     // Verify this
        }
    }
}
