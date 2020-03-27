// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectivesEditOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The ObjectivesEditOrganizerModelController
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Services.Delegates;
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
    using GalaSoft.MvvmLight.Views;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// ObjectivesEditOrganizerModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    public class ObjectivesEditOrganizerModelController : UPOrganizerModelController
    {
        private ViewReference pendingExecutionViewReference;
        private string actionButtonText;
        private UPOfflineUploadDocumentRequest uploadDocumentRequest;
        private List<UPOfflineUploadDocumentRequest> uploadDocumentRequests;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public ObjectivesEditOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options = null)
            : base(viewReference, options)
        {
        }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is edit organizer.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is edit organizer; otherwise, <c>false</c>.
        /// </value>
        public override bool IsEditOrganizer => true;

        private void AddOrganizerActions()
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
        /// Performs the objectives action.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public async void PerformObjectivesAction(object parameters)
        {
            var paramDict = (Dictionary<string, object>)parameters;
            UPMOrganizerAction action = (UPMOrganizerAction)paramDict[Core.Constants.OrganizerAction];
            ViewReference viewReference = action.ViewReference;
            if (viewReference == null)
            {
                return;
            }

            if (this.HasChanges())
            {
                this.pendingExecutionViewReference = viewReference;
                await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(
                    LocalizedString.TextObjectivesSaveWarning,
                    LocalizedString.TextObjectivesTitle,
                    LocalizedString.TextSave,
                    LocalizedString.TextCancel,
                    c =>
                    {
                        if (c)
                        {
                            this.Save(null);
                        }
                        else
                        {
                            this.pendingExecutionViewReference = null;
                        }
                    });

                return;
            }

            this.ContinueWithAction(viewReference);
        }

        private void ContinueWithAction(ViewReference viewReference)
        {
            UPMultipleOrganizerManager.CurrentOrganizerManager.SetEditingForNavControllerId(false, this.NavControllerId);
            this.InvalidateEditObjectivesEditPage();
            this.PerformActionWithViewReference(viewReference);
        }

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public override void BuildPagesFromViewReference()
        {
            ObjectivesEditPageModelController pageModelController = new ObjectivesEditPageModelController(this.ViewReference);
            UPMOrganizer organizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("EditObjectives"));
            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            string infoAreaId = this.RecordIdentification.InfoAreaId();
            UPConfigTableCaption tableCaption = ConfigurationUnitStore.DefaultStore.TableCaptionByName(this.RecordIdentification.InfoAreaId());
            organizer.TitleText = tableCaption.TableCaptionForRecordIdentification(this.RecordIdentification);
            organizer.SubtitleText = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(infoAreaId).Label;
            Page page = pageModelController.Page;
            page.LabelText = "Error";
            this.AddPageModelController(pageModelController);
            organizer.AddPage(page);
            this.TopLevelElement = organizer;
            organizer.ExpandFound = true;
            this.AddOrganizerActions();
        }

        private void InvalidateEditObjectivesEditPage()
        {
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                if (!(modelController is ObjectivesEditPageModelController))
                {
                    continue;
                }

                Page page = (Page)modelController.TopLevelElement;
                page.Invalid = true;
            }
        }

        private bool HasChanges()
        {
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                var objectivesEditPageModelController = modelController as ObjectivesEditPageModelController;

                if (objectivesEditPageModelController?.ChangedRecords().Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private async void Cancel(object actionDictionary)
        {
            if (!this.HasChanges())
            {
                this.ModelControllerDelegate.PopToPreviousContentViewController();
            }
            else
            {
                await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(
                    LocalizedString.TextReallyAbortAndLoseChanges,
                    LocalizedString.TextYouMadeChanges,
                    LocalizedString.TextYes,
                    LocalizedString.TextCancel,
                    c =>
                    {
                        if (c)
                        {
                            if (this.CloseOrganizerDelegate != null)
                            {
                                this.CloseOrganizerDelegate.UpOrganizerModelControllerAllowedClosingOrganizer(this);
                                this.CloseOrganizerDelegate = null;
                            }
                            else
                            {
                                this.ModelControllerDelegate.PopToPreviousContentViewController();
                            }
                        }
                    });
            }
        }

        private void NoChangesWhileSaving()
        {
            this.ModelControllerDelegate.PopToPreviousContentViewController();
        }

        private void Save(object actionDictionary)
        {
            List<UPCRMRecord> changedRecords = null;
            UPObjectives objectives = null;
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                var objectivesEditPageModelController = modelController as ObjectivesEditPageModelController;
                if (objectivesEditPageModelController != null)
                {
                    changedRecords = objectivesEditPageModelController.ChangedRecords();
                    objectives = objectivesEditPageModelController.ObjectivesCore;
                }
            }

            if (changedRecords == null)
            {
                this.NoChangesWhileSaving();
                return;
            }

            UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("savingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
            statusField.FieldValue = LocalizedString.TextWaitForChanges;
            stillLoadingError.StatusMessageField = statusField;
            this.Organizer.Status = stillLoadingError;
            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, null);
            objectives.OfflineRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, changedRecords, this);
        }

        private void FillUploadRequestsForRecord(UPCRMRecord record)
        {
            this.uploadDocumentRequests = new List<UPOfflineUploadDocumentRequest>();
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                if (modelController is EditPageModelController)
                {
                    ObjectivesEditPageModelController editModelController = (ObjectivesEditPageModelController)modelController;
                    UPMObjectivesPage editPage = (UPMObjectivesPage)editModelController.Page;
                    foreach (UPMGroup editGroup in editPage.Groups)
                    {
                        foreach (UPMObjective objective in editGroup.Children)
                        {
                            foreach (UPMEditField editField in objective.Fields)
                            {
                                if (editField is UPMImageEditField && editField.Changed)
                                {
                                    UPMImageEditField imageEditField = (UPMImageEditField)editField;
                                    UPEditFieldContext fieldContext = editModelController.FieldContextForEditField(imageEditField);
                                    //this.uploadDocumentRequest = new UPOfflineUploadDocumentRequest(UIImageJPEGRepresentation(imageEditField.Image, 1.0), -1, "photo.jpg", "image/jpeg", record.RecordIdentification, fieldContext.FieldId, "true");         // CRM-5007
                                    this.uploadDocumentRequests.Add(this.uploadDocumentRequest);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online,
            object context, Dictionary<string, object> result)
        {
            if (request != this.uploadDocumentRequest)
            {
                if (!string.IsNullOrEmpty(this.RecordIdentification))
                {
                    List<IIdentifier> changes = new List<IIdentifier> { new RecordIdentifier(this.RecordIdentification) };
                    UPChangeManager.CurrentChangeManager.RegisterChanges(changes);
                }

                List<UPCRMRecord> records = (List<UPCRMRecord>)data;
                this.FillUploadRequestsForRecord(records[0]);
            }

            if (this.uploadDocumentRequests?.Count > 0)
            {
                if (request != this.uploadDocumentRequest)
                {
                    this.uploadDocumentRequest = this.uploadDocumentRequests[0];
                    this.uploadDocumentRequests.Remove(this.uploadDocumentRequest);
                    bool requestAccepted = this.uploadDocumentRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, this);
                    if (requestAccepted)
                    {
                        return;
                    }
                }
                else
                {
                    this.uploadDocumentRequest = this.uploadDocumentRequests[0];
                    bool requestAccepted = this.uploadDocumentRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, this);
                    if (requestAccepted)
                    {
                        return;
                    }
                }
            }

            if (this.pendingExecutionViewReference != null)
            {
                ViewReference viewReference = this.pendingExecutionViewReference;
                this.pendingExecutionViewReference = null;
                this.Organizer.Status = null;
                this.ContinueWithAction(viewReference);
                return;
            }

            this.ModelControllerDelegate.PopToPreviousContentViewController();
            this.OfflineRequest = null;
        }

        /// <summary>
        /// Offlines the request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public override async void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            this.Organizer.Status = null;
#if PORTING
            //UPStatusHandler.ShowErrorMessage(LocalizedString.TextErrorCouldNotBeSaved);
#endif
            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, null);
            if (this.pendingExecutionViewReference != null)
            {
                await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(
                      LocalizedString.TextObjectivesSaveError,
                      LocalizedString.TextObjectivesTitle,
                      this.actionButtonText,
                      LocalizedString.TextCancel,
                      c =>
                      {
                          if (c)
                          {
                              ViewReference viewReference = this.pendingExecutionViewReference;
                              this.pendingExecutionViewReference = null;
                              this.ContinueWithAction(viewReference);
                          }
                          else
                          {
                              this.pendingExecutionViewReference = null;
                          }
                      });
            }
        }

        /// <summary>
        /// Closes the organizer allowed with delegate.
        /// </summary>
        /// <param name="closeOrganizerDelegate">The close organizer delegate.</param>
        /// <returns></returns>
        public override bool CloseOrganizerAllowedWithDelegate(UPMCloseOrganizerDelegate closeOrganizerDelegate)
        {
            if (this.HasChanges())
            {
                this.CloseOrganizerDelegate = closeOrganizerDelegate;

#if PORTING
                await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(
                    LocalizedString.TextReallyAbortAndLoseChanges,
                    LocalizedString.TextYouMadeChanges,
                    LocalizedString.TextYes,
                    LocalizedString.TextNo,
                    c =>
                    {
                        if (c)
                        {
                            if (this.CloseOrganizerDelegate != null)
                            {
                                this.CloseOrganizerDelegate.UpOrganizerModelControllerAllowedClosingOrganizer(this);
                                this.CloseOrganizerDelegate = null;
                            }
                            else
                            {
                                this.ModelControllerDelegate.PopToPreviousContentViewController();
                            }
                        }
                    });
#endif
                return false;
            }

            return true;
        }
    }
}
