// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectivesPageModelController.cs" company="Aurea Software Gmbh">
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
//   Modify Record Action Handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Objectives;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    public class UPObjectivesPageModelController : EditPageModelController, UPObjectivesDelegate
    {
        private List<UPObjectivesGroup> objectivesGroupArray;
        private bool editMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPObjectivesPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="editMode">if set to <c>true</c> [edit mode].</param>
        public UPObjectivesPageModelController(ViewReference viewReference, bool editMode = false)
            : base(viewReference)
        {
            this.editMode = editMode;
            this.BuildPage();
        }

        /// <summary>
        /// The deleted items
        /// </summary>
        protected List<UPObjectivesItem> DeletedItems;

        /// <summary>
        /// Gets the objectives core.
        /// </summary>
        /// <value>
        /// The objectives core.
        /// </value>
        public UPObjectives ObjectivesCore { get; private set; }

        /// <summary>
        /// Instantiates the page.
        /// </summary>
        /// <returns></returns>
        public override Page InstantiatePage()
        {
            return this.Page != null
                ? new UPMObjectivesPage(this.Page.Identifier)
                : new UPMObjectivesPage(FieldIdentifier.IdentifierWithRecordIdentificationFieldId(this.recordId, "Page0"));
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public override void BuildPage()
        {
            this.recordId = this.ViewReference.ContextValueForKey("RecordId");
            this.recordId = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(this.recordId);
            this.InfoAreaId = this.ViewReference.ContextValueForKey("InfoAreaId");
            this.ConfigName = this.ViewReference.ContextValueForKey("ConfigName");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "viewReference", this.ViewReference } };
            this.ObjectivesCore = new UPObjectives(this.recordId, parameters, this.editMode, this);
            this.editPageContext = new UPEditPageContext(this.recordId, false, null, null, this.ViewReference);
            this.DeletedItems = new List<UPObjectivesItem>();
            UPMObjectivesPage _page = (UPMObjectivesPage)this.InstantiatePage();
            _page.LabelText = LocalizedString.TextTabOverview;
            _page.Invalid = true;
            this.TopLevelElement = _page;
            this.ApplyLoadingStatusOnPage(_page);
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="oldDetailPage">The old detail page.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElementForPage(Page oldDetailPage)
        {
            this.ObjectivesCore.Build();
            return oldDetailPage;
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            if (element is Page)
            {
                return this.UpdatedElementForPage((Page)element);
            }

            return element;
        }

        public override void ApplyLoadingStatusOnPage(Page page)
        {
            UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
            statusField.FieldValue = LocalizedString.TextLoadingData;
            stillLoadingError.StatusMessageField = statusField;
            page.Status = stillLoadingError;
        }

        /// <summary>
        /// Objectiveses the did fail with error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        public void ObjectivesDidFailWithError(UPObjectives sender, Exception error)
        {
            if (error.IsConnectionOfflineError())
            {
                this.HandlePageErrorDetails(LocalizedString.TextOffline, LocalizedString.TextErrorTitleNoInternet);
            }
            else
            {
                this.HandlePageErrorDetails(error.Message, error.StackTrace);
            }

            this.Page.Invalid = false;
            this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, null);
        }

        /// <summary>
        /// Objectiveses the did finish.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void ObjectivesDidFinish(UPObjectives sender)
        {
            this.objectivesGroupArray = new List<UPObjectivesGroup>(sender.Groups);
            this.UpdatePageWithResult();
        }

        private void FillPage(Page page)
        {
            if (page is UPMObjectivesPage)
            {
                this.LoadData((UPMObjectivesPage)page);
            }

            page.Invalid = false;
        }

        private void UpdatePageWithResult()
        {
            Page oldPage = this.Page;
            UPMObjectivesPage newPage = (UPMObjectivesPage)this.InstantiatePage();
            newPage.Invalid = false;
            newPage.LabelText = oldPage.LabelText;
            this.FillPage(newPage);
            this.TopLevelElement = newPage;
            this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
        }

        private UPMObjectivesSection CreateSectionIdentfier(string title, IIdentifier identifier)
        {
            UPMObjectivesSection section0 = new UPMObjectivesSection(identifier);
            section0.TitleField = new UPMStringField(identifier);
            section0.TitleField.StringValue = title;
            return section0;
        }

        private UPMObjective CreateObjectiveIdentfier(UPObjectivesItem item, IIdentifier identifier)
        {
            UPMObjective objective = new UPMObjective(identifier);
            objective.ObjectiveItem = null;
            UPMStringField titleField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"{identifier.IdentifierAsString}-title"));
            titleField.StringValue = item.TitleFieldValue;
            UPMStringField subtitleField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"{identifier.IdentifierAsString}-subtitle"));
            subtitleField.StringValue = item.SubTitelFieldValue;
            UPMStringField dateField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"{identifier.IdentifierAsString}-date"));
            dateField.StringValue = DateExtensions.LocalizedFormattedDate(item.Date);
            objective.AddMainField(titleField);
            objective.AddMainField(subtitleField);
            objective.AddMainField(dateField);
            objective.DoneField = new UPMBooleanEditField(StringIdentifier.IdentifierWithStringId($"{identifier.IdentifierAsString}-done"));
            objective.DoneField.BoolValue = item.Completed;
            objective.CanBeDeletedField = new UPMBooleanEditField(StringIdentifier.IdentifierWithStringId($"{identifier.IdentifierAsString}-canByDelete"));
            objective.CanBeDeletedField.BoolValue = item.CanBeDeleted;
            return objective;
        }

        private void LoadData(UPMObjectivesPage objectivesPage)
        {
            int itemCounter = 0;
            int sectionCounter = 0;
            foreach (UPObjectivesGroup group in this.objectivesGroupArray)
            {
                UPMObjectivesSection section = this.CreateSectionIdentfier(group.Label, StringIdentifier.IdentifierWithStringId(group.GroupKey));
                foreach (UPObjectivesItem item in group.Items)
                {
                    UPMObjective mobjective = this.CreateObjectiveIdentfier(item, StringIdentifier.IdentifierWithStringId($"ObjectiveItem_{itemCounter}"));
                    if (item.Documents != null)
                    {
                        UPMDocumentsGroup documentGroup = new UPMDocumentsGroup(StringIdentifier.IdentifierWithStringId($"documentgroup{itemCounter}"));
                        foreach (DocumentData document in item.Documents)
                        {
                            UPMDocument documentModel = new UPMDocument(document);
                            documentGroup.AddChild(documentModel);
                        }

                        mobjective.AddGroup(documentGroup);
                    }

                    mobjective.ObjectiveItem = item;
                    Dictionary<string, UPEditFieldContext> itemEditFields = new Dictionary<string, UPEditFieldContext>();
                    for (int additionalFieldIndex = 0; additionalFieldIndex < item.AdditionalFields.Count; additionalFieldIndex++)
                    {
                        UPConfigFieldControlField field = item.AdditionalFields[additionalFieldIndex];
                        RecordIdentifier fieldIdentifier = new RecordIdentifier(field.Identification);
                        UPEditFieldContext fieldContext = null;
                        FieldAttributes attributes = field.Attributes;
                        if (attributes != null && attributes.Hide)
                        {
                            fieldContext = UPEditFieldContext.HiddenFieldFor(field, fieldIdentifier, item.Values[additionalFieldIndex]);
                        }
                        else if (attributes != null && attributes.ReadOnly)
                        {
                            fieldContext = UPEditFieldContext.ReadonlyFieldFor(field, fieldIdentifier, item.Values[additionalFieldIndex]);
                        }
                        else
                        {
                            fieldContext = UPEditFieldContext.FieldContextFor(field, fieldIdentifier, item.Values[additionalFieldIndex], (List<UPEditFieldContext>)null);
                        }

                        if (fieldContext?.Field != null)
                        {
                            string fieldIdentification = this.FieldIdentificationSectionCounterItemCounter(field.Field, sectionCounter, itemCounter);
                            this.editPageContext.EditFields.SetObjectForKey(fieldContext, fieldIdentification);
                            itemEditFields.SetObjectForKey(fieldContext, fieldIdentification);
                            if (fieldContext.EditField != null)
                            {
                                fieldContext.EditField.EditFieldsContext = this.editPageContext;
                                mobjective.AddField(fieldContext.EditField);
                            }
                            else
                            {
                                mobjective.AddField(fieldContext.Field);
                            }
                        }
                    }

                    this.HandleDependentFieldsSectionCounterItemCounter(itemEditFields, sectionCounter, itemCounter);
                    if (item.ButtonActions.Count > 0)
                    {
                        List<UPMOrganizerAction> buttonActions = new List<UPMOrganizerAction>();
                        foreach (UPConfigButton button in item.ButtonActions)
                        {
                            StringIdentifier fieldIdentifier = StringIdentifier.IdentifierWithStringId("button");
                            UPMOrganizerAction action = new UPMOrganizerAction(fieldIdentifier);
                            //action.SetTargetAction(this.ParentOrganizerModelController, PerformObjectivesAction);
                            action.ViewReference = button.ViewReference.ViewReferenceWith(item.Record.RecordIdentification);
                            action.LabelText = button.Label;
                            buttonActions.Add(action);
                        }

                        mobjective.Actions = buttonActions;
                    }

                    section.AddChild(mobjective);
                    itemCounter++;
                }

                if (section.Children.Count > 0)
                {
                    objectivesPage.AddChild(section);
                }
            }

            if (objectivesPage.Children.Count == 0)
            {
                UPMMessageStatus messageStatus = new UPMMessageStatus(StringIdentifier.IdentifierWithStringId("messageIdentifier"));
                UPMStringField messageField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
                messageField.FieldValue = LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicNoObjectives);
                messageStatus.DetailMessageField = messageField;
                objectivesPage.Status = messageStatus;
            }
        }

        private void HandleDependentFieldsSectionCounterItemCounter(Dictionary<string, UPEditFieldContext> editFields, int sectionCounter, int itemCounter)
        {
            List<UPEditFieldContext> parentFieldContextArray = new List<UPEditFieldContext>();
            foreach (UPEditFieldContext fieldContext in editFields.Values)
            {
                UPCRMField parentField = fieldContext.ParentField;
                if (parentField != null)
                {
                    UPEditFieldContext parentFieldContext = this.editPageContext.EditFields.ValueOrDefault(this.FieldIdentificationSectionCounterItemCounter(parentField, sectionCounter, itemCounter));
                    if (parentFieldContext != null)
                    {
                        parentFieldContext.AddDependentFieldContext(fieldContext);
                        if (parentFieldContextArray.Contains(parentFieldContext) == false)
                        {
                            parentFieldContextArray.Add(parentFieldContext);
                        }
                    }
                }
            }

            foreach (UPEditFieldContext parentFieldContext in parentFieldContextArray)
            {
                parentFieldContext.NotifyDependentFields();
            }
        }

        private string FieldIdentificationSectionCounterItemCounter(UPCRMField field, int sectionCounter, int itemCounter)
        {
            return $"{field.FieldIdentification}-{sectionCounter}-{itemCounter}";
        }
    }
}
