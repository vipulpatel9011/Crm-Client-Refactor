// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionnaireEditOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Edit Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Questionnaire
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Status;
    using Core.Questionnaire;

    /// <summary>
    /// Questionnaire Edit Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.EditOrganizerModelController" />
    public class QuestionnaireEditOrganizerModelController : EditOrganizerModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireEditOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public QuestionnaireEditOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options)
            : base(viewReference, options)
        {
            this.BuildEditOrganizerPage();
        }

        /// <summary>
        /// Gets the survey text.
        /// </summary>
        /// <value>
        /// The survey text.
        /// </value>
        public string SurveyText
        {
            get
            {
                string surveyText = LocalizedString.TextProcessSurvey;
                return surveyText == LocalizedString.TextEditCharacteristics ? "Survey" : surveyText;
            }
        }

        /// <summary>
        /// Sets the name of the survey.
        /// </summary>
        /// <param name="surveyName">Name of the survey.</param>
        public void SetSurveyName(string surveyName)
        {
            if (!string.IsNullOrEmpty(surveyName))
            {
                this.Organizer.TitleText = $"{this.SurveyText}: {surveyName}";
            }
        }

        private void BuildEditOrganizerPage()
        {
            UPMOrganizer organizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("EditQuestionnaire"));
            this.TopLevelElement = organizer;
            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            this.InfoAreaId = this.RecordIdentification.InfoAreaId();
            UPConfigTableCaption tableCaption = ConfigurationUnitStore.DefaultStore.TableCaptionByName(this.RecordIdentification.InfoAreaId());
            organizer.TitleText = this.SurveyText;
            organizer.SubtitleText = tableCaption.TableCaptionForRecordIdentification(this.RecordIdentification);
            QuestionnaireEditPageModelController pageModelController = new QuestionnaireEditPageModelController(this.ViewReference);
            this.AddPageModelController(pageModelController);
            organizer.AddPage(pageModelController.Page);
            this.AddOrganizerActions();
            organizer.ExpandFound = true;
        }

        /// <summary>
        /// Adds the organizer actions.
        /// </summary>
        public override void AddOrganizerActions()
        {
            UPMOrganizerAction cancelAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.cancel"));
            cancelAction.SetTargetAction(this, this.Cancel);
            cancelAction.LabelText = LocalizedString.TextCancel;
            this.AddLeftNavigationBarActionItem(cancelAction);
            UPMOrganizerAction saveAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.save"));
            saveAction.SetTargetAction(this, this.Save);
            saveAction.LabelText = LocalizedString.TextSave;
            saveAction.MainAction = true;
            this.AddRightNavigationBarActionItem(saveAction);
            this.SaveActionItems.Add(saveAction);
        }

        /// <summary>
        /// Sets the questionnaire finalized.
        /// </summary>
        /// <param name="finalized">if set to <c>true</c> [finalized].</param>
        public void SetQuestionnaireFinalized(bool finalized)
        {
            if (finalized)
            {
                UPMOrganizerAction action = this.ActionItem(StringIdentifier.IdentifierWithStringId("action.save"));
                List<IIdentifier> changedIdentifiers = new List<IIdentifier>();
                if (action != null)
                {
                    action.LabelText = finalized ? LocalizedString.TextClose : LocalizedString.TextSave;
                    changedIdentifiers.Add(action.Identifier);
                }

                if (this.LeftNavigationBarItems.Count > 0)
                {
                    action = (UPMOrganizerAction)this.LeftNavigationBarItems[0];
                    action.Enabled = false;
                    changedIdentifiers.Add(action.Identifier);
                }

                this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, changedIdentifiers, UPChangeHints.ChangeHintsWithHint("EditActionsChanged"));
            }
        }

        /// <summary>
        /// Cancels edit page
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        protected override void Cancel(object actionDictionary)
        {
            this.ModelControllerDelegate.PopToPreviousContentViewController();
        }

        private void NoChangesWhileSaving()
        {
            this.ModelControllerDelegate.PopToPreviousContentViewController();
        }

        /// <summary>
        /// Saves the and confirm.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public void SaveAndConfirm(ViewReference viewReference)
        {
            UPConfigFilter rootTemplateFilter = null, baseTemplateFilter = null;
            string templateFilterName = viewReference.ContextValueForKey("ConfirmFilter");
            if (!string.IsNullOrEmpty(templateFilterName))
            {
                rootTemplateFilter = ConfigurationUnitStore.DefaultStore.FilterByName(templateFilterName);
            }

            templateFilterName = viewReference.ContextValueForKey("BaseRecordConfirmFilter");
            if (!string.IsNullOrEmpty(templateFilterName))
            {
                baseTemplateFilter = ConfigurationUnitStore.DefaultStore.FilterByName(templateFilterName);
            }

            this.SaveRootTemplateFilterBaseTemplateFilter(null, rootTemplateFilter, baseTemplateFilter);
        }

        /// <summary>
        /// Saves the specified action dictionary.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public override void Save(object actionDictionary)
        {
            this.SaveRootTemplateFilterBaseTemplateFilter(actionDictionary, null, null);
        }

        /// <summary>
        /// Closes the organizer allowed with delegate.
        /// </summary>
        /// <param name="closeOrganizerDelegate">The close organizer delegate.</param>
        /// <returns></returns>
        public override bool CloseOrganizerAllowedWithDelegate(UPMCloseOrganizerDelegate closeOrganizerDelegate)
        {
            List<UPCRMRecord> changedRecords = this.ChangedRecordsWithRootTemplateFilter(null, null, true);
            if (changedRecords.Count > 0)
            {
                this.CloseOrganizerDelegate = closeOrganizerDelegate;
                //UIAlertView alertview = new UIAlertView(LocalizedString.TextYouMadeChanges, LocalizedString.TextReallyAbortAndLoseChanges, this, LocalizedString.TextNo, LocalizedString.TextYes, null);
                //alertview.Show();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, Dictionary<string, object> result)
        {
            List<UPCRMRecord> changedRecords = (List<UPCRMRecord>)data;
            if (changedRecords.Count > 0)
            {
                List<IIdentifier> changedIdentifiers = new List<IIdentifier>();
                changedIdentifiers.AddRange(changedRecords.Select(x => new RecordIdentifier(x.RecordIdentification)));
                UPChangeManager.CurrentChangeManager.RegisterChanges(changedIdentifiers);
            }

            this.ModelControllerDelegate.PopToPreviousContentViewController();
        }

        /// <summary>
        /// Offlines the request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public override void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            if (this.ModelControllerDelegate != null)
            {
                this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
            }

            this.OfflineRequest = null;
        }

        private List<UPCRMRecord> ChangedRecordsWithRootTemplateFilter(UPConfigFilter rootTemplateFilter, UPConfigFilter baseTemplateFilter, bool ignoreDefault)
        {
            List<UPCRMRecord> changedRecordArray = null;
            UPSurvey survey = null;
            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                List<UPCRMRecord> cr;

                var questionnaireEditPageModelController = pageModelController as QuestionnaireEditPageModelController;
                if (ignoreDefault && questionnaireEditPageModelController != null)
                {
                    cr = questionnaireEditPageModelController.ChangedRecordsIgnoreDefaultWithRootTemplateFilterBaseTemplateFilter(rootTemplateFilter, baseTemplateFilter);
                }
                else if (!ignoreDefault && questionnaireEditPageModelController != null)
                {
                    cr = questionnaireEditPageModelController.ChangedRecordsWithRootTemplateFilterBaseTemplateFilter(rootTemplateFilter, baseTemplateFilter);
                }
                else
                {
                    cr = pageModelController.ChangedRecords();
                }

                if (cr?.Count > 0)
                {
                    if (changedRecordArray == null)
                    {
                        changedRecordArray = new List<UPCRMRecord>(cr);
                    }
                    else
                    {
                        changedRecordArray.AddRange(cr);
                    }
                }

                if (survey == null && questionnaireEditPageModelController != null)
                {
                    survey = questionnaireEditPageModelController.Survey;
                }
            }

            return changedRecordArray;
        }

        private void SaveRootTemplateFilterBaseTemplateFilter(object actionDictionary, UPConfigFilter rootTemplateFilter, UPConfigFilter baseTemplateFilter)
        {
            UPMOrganizerAction action = this.ActionItem(StringIdentifier.IdentifierWithStringId("action.save"));
            if (action.LabelText == LocalizedString.TextClose)
            {
                this.Cancel(actionDictionary);
                return;
            }

            List<UPCRMRecord> changedRecordArray = this.ChangedRecordsWithRootTemplateFilter(rootTemplateFilter, baseTemplateFilter, false);
            UPSurvey survey = null;
            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                if (survey == null && pageModelController is QuestionnaireEditPageModelController)
                {
                    survey = ((QuestionnaireEditPageModelController)pageModelController).Survey;
                }
            }

            if (changedRecordArray == null)
            {
                this.NoChangesWhileSaving();
                return;
            }

            if (survey != null)
            {
                UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("savingIdentifier"));
                UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
                statusField.FieldValue = LocalizedString.TextWaitForChanges;
                stillLoadingError.StatusMessageField = statusField;
                this.Organizer.Status = stillLoadingError;
                this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, null);

                survey.OfflineRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, changedRecordArray, this);
            }
        }
    }
}
