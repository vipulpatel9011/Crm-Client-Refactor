// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectivesEditPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Objctives Edit Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Objectives;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// The Objctives Edit Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.UPObjectivesPageModelController" />
    public class ObjectivesEditPageModelController : UPObjectivesPageModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectivesEditPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public ObjectivesEditPageModelController(ViewReference viewReference)
            : base(viewReference, true)
        {
        }

        /// <summary>
        /// Deletes the objective.
        /// </summary>
        /// <param name="_objective">The objective.</param>
        public void DeleteObjective(UPMObjective _objective)
        {
            _objective.ObjectiveItem.Deleted = true;
            this.DeletedItems.Add(_objective.ObjectiveItem);

            foreach (UPMObjectivesSection objectiveSection in this.Page.Children)
            {
                foreach (UPMObjective objective in objectiveSection.Children)
                {
                    if (objective.Identifier.MatchesIdentifier(_objective.Identifier))
                    {
                        List<UPMElement> objectives = new List<UPMElement>(objectiveSection.Children);
                        objectiveSection.RemoveAllChildren();
                        foreach (UPMObjective objective2 in objectives)
                        {
                            if (!objective2.Identifier.MatchesIdentifier(_objective.Identifier))
                            {
                                objectiveSection.AddChild(objective2);
                            }
                        }

                        break;
                    }
                }
            }

            this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, new List<IIdentifier> { _objective.Identifier }, null);
        }

        /// <summary>
        /// Fields the context for edit field.
        /// </summary>
        /// <param name="editField">The edit field.</param>
        /// <returns></returns>
        public UPEditFieldContext FieldContextForEditField(UPMEditField editField)
        {
            List<UPEditFieldContext> fieldContextList = this.editPageContext.EditFields.Values.ToList();
            foreach (UPEditFieldContext fieldContext in fieldContextList)
            {
                if (fieldContext.EditField == editField)
                {
                    return fieldContext;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns changed records.
        /// </summary>
        /// <returns></returns>
        public override List<UPCRMRecord> ChangedRecords()
        {
            List<UPCRMRecord> changedRecords = new List<UPCRMRecord>();
            UPMObjectivesPage page = (UPMObjectivesPage)this.Page;
            foreach (UPMObjectivesSection section in page.Children)
            {
                foreach (UPMObjective item in section.Children)
                {
                    UPObjectivesItem crmItem = item.ObjectiveItem;
                    if (item.DoneField.Changed)
                    {
                        if (crmItem != null)
                        {
                            crmItem.Completed = item.DoneField.BoolValue;
                        }
                    }

                    int editFieldIndex = 0;
                    for (int elementIndex = 0; elementIndex < item.Fields.Count; elementIndex++)
                    {
                        UPMElement element = item.Fields[elementIndex];
                        if (element is UPMEditField)
                        {
                            UPMEditField editField = (UPMEditField)element;
                            if (editField.Changed)
                            {
                                List<UPEditFieldContext> fieldContextList = this.editPageContext.EditFields.Values.ToList();
                                foreach (UPEditFieldContext fieldContext in fieldContextList)
                                {
                                    if (fieldContext.EditField == editField)
                                    {
                                        crmItem?.SetValueForAdditionalFieldPosition(fieldContext.Value ?? string.Empty, editFieldIndex);
                                    }
                                }
                            }

                            editFieldIndex++;
                        }
                        else if (element is UPMStringField)
                        {
                            editFieldIndex++;
                        }
                    }

                    List<UPCRMRecord> itemChangedRecords = crmItem?.ChangedRecords();
                    if (itemChangedRecords != null)
                    {
                        changedRecords.AddRange(itemChangedRecords);
                    }
                }
            }

            foreach (UPObjectivesItem deletedItem in this.DeletedItems)
            {
                List<UPCRMRecord> itemChangedRecords = deletedItem.ChangedRecords();
                if (itemChangedRecords != null)
                {
                    changedRecords.AddRange(deletedItem.ChangedRecords());
                }
            }

            return changedRecords.Count == 0 ? null : changedRecords;
        }
    }
}
