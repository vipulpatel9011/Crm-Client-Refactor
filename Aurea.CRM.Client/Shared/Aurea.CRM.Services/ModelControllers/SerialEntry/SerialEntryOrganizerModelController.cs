// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialEntryOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The Serial Entry Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.SerialEntry
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
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    /// <summary>
    /// Enum UPSerialEntryFinalSaveResult
    /// </summary>
    public enum UPSerialEntryFinalSaveResult
    {
        /// <summary>
        /// Finished
        /// </summary>
        Finished = 1,

        /// <summary>
        /// Pending
        /// </summary>
        Pending,

        /// <summary>
        /// Abort
        /// </summary>
        Abort
    }

    /// <summary>
    /// Serial Entry Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.EditOrganizerModelController" />
    public class SerialEntryOrganizerModelController : EditOrganizerModelController
    {
        private bool created;
        private bool isSaving;
        private bool isErrorSaveDialog;
        private bool waitForSave;
        private bool waitForSerialEntrySave;
        private bool disappearingOrganizer;
        private UPMOrganizerAction leftAction;
        private UPPageModelController currentModelController;
        private SerialEntryPageModelController _modelControllerToRedraw;
        private bool finalSaveCalled;
        private bool organizerDealsWithSaveErrors;
        private SerialEntryPageModelController serialEntryPageModelController;
        private bool closeOnErrors;
        private string finishActionName;

        /// <summary>
        /// The right action
        /// </summary>
        protected UPMOrganizerAction RightAction;

        /// <summary>
        /// Gets or sets a value indicating whether [conflict handling mode].
        /// </summary>
        /// <value>
        /// <c>true</c> if [conflict handling mode]; otherwise, <c>false</c>.
        /// </value>
        public bool ConflictHandlingMode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialEntryOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public SerialEntryOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options)
            : base(viewReference, options)
        {
            this.ShouldShowTabsForSingleTab = options.bShouldShowTabsForSingleTab;
            this.isSaving = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialEntryOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public SerialEntryOrganizerModelController(ViewReference viewReference)
            : this(viewReference, UPOrganizerInitOptions.ShouldShowTabsForSingleTab())
        {
        }

        /// <summary>
        /// Deallocs this instance.
        /// </summary>
        public override void Dealloc()
        {
            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                if (pageModelController is SerialEntryPageModelController)
                {
                    //pageModelController.RemoveObserverForKeyPath(this, "hasRunningChangeRequests");
                }
            }
        }

        /// <summary>
        /// Adds the page model controllers.
        /// </summary>
        public override void AddPageModelControllers()
        {
            ViewReference pageViewReference;
            if (this.CopyFieldDictionary != null)
            {
                pageViewReference = this.ViewReference.ViewReferenceWith(new Dictionary<string, object> { { "copyFields", this.CopyFieldDictionary } });
            }
            else
            {
                pageViewReference = this.ViewReference;
            }

            EditPageModelController editPageModelController = new EditPageModelController(pageViewReference, this.IsNew, this.InitialValueDictionary, null);
            editPageModelController.Delegate = this;
            Page overviewPage = editPageModelController.Page;
            if (editPageModelController.DisableRightActionItems || !(overviewPage.Status is UPMProgressStatus))
            {
                //this.EnableActionItemsDisableActionItems(this.LeftNavigationBarItems, this.RightNaviagtionBarItems);
            }

            this.AddPageModelController(editPageModelController);
            this.Organizer.AddPage(overviewPage);
            if (this.IsNew == false)
            {
                this.AddRemainingPageModelController();
            }
        }

        /// <summary>
        /// Adds the organizer actions.
        /// </summary>
        public override void AddOrganizerActions()
        {
            this.leftAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.cancel"));
            this.leftAction.SetTargetAction(this, this.Cancel);
            this.leftAction.LabelText = LocalizedString.TextCancel;
            this.AddLeftNavigationBarActionItem(this.leftAction);
            if (this.IsNew == false)
            {
                this.RightAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.save"));
                this.RightAction.MainAction = true;
                this.RightAction.LabelText = LocalizedString.TextSerialEntryComplete;
                this.RightAction.SetTargetAction(this, this.Save);
                this.AddRightNavigationBarActionItem(this.RightAction);
                return;
            }

            this.RightAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.create"));
            this.RightAction.MainAction = true;
            this.RightAction.SetTargetAction(this, this.Save);
            this.RightAction.LabelText = LocalizedString.TextCreate;
            this.AddRightNavigationBarActionItem(this.RightAction);
        }

        /// <summary>
        /// Saves the specified action dictionary.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public override void Save(object actionDictionary)
        {
            this.organizerDealsWithSaveErrors = true;
            this.finishActionName = null;
            this.ExecuteFinalSave();
        }

        /// <summary>
        /// Performs the finish action.
        /// </summary>
        /// <param name="name">The name.</param>
        public void PerformFinishAction(string name)
        {
            this.organizerDealsWithSaveErrors = true;
            this.finishActionName = name;
            this.ExecuteFinalSave();
        }

        /// <summary>
        /// Cancels the specified action dictionary.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public void Cancel(object actionDictionary)
        {
            this.ModelControllerDelegate.PopToPreviousContentViewController();
        }

        /// <summary>
        /// Updates the title text.
        /// </summary>
        private void UpdateTitleText()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.Organizer.SubtitleText = configStore.InfoAreaConfigById(this.RecordIdentification.InfoAreaId()).SingularName;
            UPConfigTableCaption tableCaption = configStore.TableCaptionByName(this.RecordIdentification.InfoAreaId());
            if (tableCaption != null)
            {
                string recordTableCaption = tableCaption.TableCaptionForRecordIdentification(this.RecordIdentification);

                this.Organizer.TitleText = recordTableCaption;
            }
            else
            {
                this.Organizer.TitleText = this.RecordIdentification;
            }
        }

        /// <summary>
        /// Handles the change manager.
        /// </summary>
        private void HandleChangeManager()
        {
            var changeRecordIdentification = !this.IsNew ? this.RecordIdentification : this.LinkRecordIdentification;

            if (!string.IsNullOrEmpty(changeRecordIdentification))
            {
                UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { new RecordIdentifier(changeRecordIdentification) });
            }
        }

        /// <summary>
        /// Offlines the request did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public void OfflineRequestDidFailWithError(UPOfflineRecordRequest request, object context, Exception error)
        {
            this.waitForSave = false;
            this.EnableActionItemsDisableActionItems(this.LeftNavigationBarItems, this.RightNaviagtionBarItems);

            Page page = (Page)this.TopLevelElement;
            UPMStringField errorMessageField = new UPMStringField(StringIdentifier.IdentifierWithStringId("errorField"))
            {
                StringValue = LocalizedString.TextErrorCouldNotBeSaved
            };

            UPMStringField detailErrorMessageField = new UPMStringField(StringIdentifier.IdentifierWithStringId("detailErrorField"))
            {
                StringValue = LocalizedString.Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsCouldNotBeSavedDetailMessage)
            };

            UPMErrorStatus errorStatus = new UPMErrorStatus(StringIdentifier.IdentifierWithStringId("error"))
            {
                MessageField = errorMessageField,
                DetailMessageField = detailErrorMessageField
            };
            page.Status = errorStatus;
            this.InformAboutDidFailTopLevelElement(page);
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
            this.waitForSave = false;
            this.Organizer.Status = null;
            if (this.created)
            {
                if (!this.waitForSerialEntrySave)
                {
                    base.OfflineRequestDidFinishWithResult(request, data, online, context, result);
                }

                return;
            }

            List<IIdentifier> changedIdentifiers = new List<IIdentifier>(((UPOfflineRecordRequest)request).Records.Count);
            this.SetRecordIdentificationForRecords((List<UPCRMRecord>)data, changedIdentifiers);
            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                changedIdentifiers.Add(new RecordIdentifier(this.LinkRecordIdentification));
                if (!string.IsNullOrEmpty(this.LinkRecordIdentification2))
                {
                    changedIdentifiers.Add(new RecordIdentifier(this.LinkRecordIdentification2));
                }
            }

            if (this.IsNew)
            {
                this.AddRemainingPageModelController();
                this.UpdateTitleText();
                this.UpdateOrganizerActions(changedIdentifiers);
            }

            UPChangeManager.CurrentChangeManager.RegisterChanges(changedIdentifiers);
            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, changedIdentifiers, null);
        }

        private void SetRecordIdentificationForRecords(List<UPCRMRecord> records, List<IIdentifier> changedIdentifiers)
        {
            int count = records.Count;
            bool originalRecordChanged = false;
            string firstRecordIdentification = null;
            int i;
            for (i = 0; i < count; i++)
            {
                UPCRMRecord record = records[i];
                changedIdentifiers.Add(new RecordIdentifier(record.InfoAreaId, record.RecordId));
                if (record.RecordIdentification != this.RecordIdentification)
                {
                    originalRecordChanged = true;
                }

                if (i == 0)
                {
                    firstRecordIdentification = record.RecordIdentification;
                }
            }

            if (!originalRecordChanged)
            {
                changedIdentifiers.Add(new RecordIdentifier(this.InfoAreaId, this.RecordIdentification.RecordId()));
                UPCRMRecord record = records[i];
                changedIdentifiers.AddRange(record.Links.Select(link => new RecordIdentifier(link.RecordIdentification)));
            }

            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                changedIdentifiers.Add(new RecordIdentifier(this.LinkRecordIdentification));
            }

            this.RecordIdentification = firstRecordIdentification;
        }

        /// <summary>
        /// Pages for view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <returns></returns>
        public override UPPageModelController PageForViewReference(ViewReference viewReference)
        {
            if (viewReference.ViewName == "WebContentView" || viewReference.ViewName == "ConfirmWebContentView")
            {
                return new SerialEntryWebContentModelController(viewReference, true);
            }

            return base.PageForViewReference(viewReference);
        }

        /// <summary>
        /// Adds the remaining page model controller.
        /// </summary>
        public void AddRemainingPageModelController()
        {
            this.created = true;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPConfigHeader editHeader = configStore.HeaderByNameFromGroup(this.IsNew ? "New" : "Update", this.ExpandConfig.HeaderGroupName) ??
                                        configStore.HeaderByNameFromGroup("Edit", this.ExpandConfig.HeaderGroupName);

            if (editHeader != null && editHeader.SubViews.Count != 0)
            {
                Page rootPage = this.PageModelControllers[0].Page;
                if (string.IsNullOrEmpty(rootPage.LabelText) && !string.IsNullOrEmpty(editHeader.Label))
                {
                    this.PageModelControllers[0].Page.LabelText = editHeader.Label;
                }

                foreach (UPConfigHeaderSubView subView in editHeader.SubViews)
                {
                    ViewReference pageViewReference = this.CopyFieldDictionary != null
                        ? subView.ViewReference.ViewReferenceWith(new Dictionary<string, object> { { "copyFields", this.CopyFieldDictionary } })
                        : subView.ViewReference;

                    UPPageModelController pageModelController = pageViewReference.ViewName == "RecordView"
                        ? new EditPageModelController(pageViewReference.ViewReferenceWith(this.RecordIdentification))
                        : this.PageForViewReference(pageViewReference.ViewReferenceWith(this.RecordIdentification));

                    if (pageModelController != null)
                    {
                        if (pageModelController is SerialEntryPageModelController)
                        {
                            //pageModelController.AddObserverForKeyPathOptionsContext(this, "hasRunningChangeRequests", NSKeyValueObservingOptionNew, null);
                        }

                        pageModelController.Page.Invalid = true;
                        pageModelController.Page.LabelText = subView.Label;
                        pageModelController.Delegate = this;
                        this.AddPageModelController(pageModelController);
                        this.Organizer.AddPage(pageModelController.Page);

                        if (pageModelController is UPWebContentPageModelController)
                        {
                            if (((UPWebContentPageModelController)pageModelController).AllowsXMLExport)
                            {
                                UPMOrganizerAction exportXMLAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.exportXML"));
                                //exportXMLAction.SetTargetAction(pageModelController, ExportXML);
                                exportXMLAction.LabelText = "Export XML";
                                this.AddOrganizerHeaderActionItem(exportXMLAction);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the organizer actions.
        /// </summary>
        /// <param name="changedIdentifiers">The changed identifiers.</param>
        public void UpdateOrganizerActions(List<IIdentifier> changedIdentifiers)
        {
            this.RightAction.LabelText = LocalizedString.TextClose;
            this.RightAction.SetTargetAction(this, this.Save);
            this.LeftNavigationBarItems.Remove(this.leftAction);
            changedIdentifiers.Add(this.RightAction.Identifier);
            changedIdentifiers.Add(this.leftAction.Identifier);
            this.EnableActionItemsInArray(this.RightNaviagtionBarItems);
        }

        /// <summary>
        /// Enables the action items in array.
        /// </summary>
        /// <param name="array">The array.</param>
        public void EnableActionItemsInArray(List<UPMElement> array)
        {
            foreach (UPMElement actionElement in array)
            {
                if (actionElement is UPMOrganizerAction)
                {
                    ((UPMOrganizerAction)actionElement).Enabled = true;
                }
                else if (actionElement is UPMOrganizerActionGroup)
                {
                    ((UPMOrganizerActionGroup)actionElement).Enabled = true;
                }
            }
        }

        /// <summary>
        /// Pops to previous content view controller.
        /// </summary>
        public override void PopToPreviousContentViewController()
        {
            this.disappearingOrganizer = true;
            base.PopToPreviousContentViewController();
        }

        /// <summary>
        /// Performed when the model controller view will disappear.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        public override void PageModelControllerViewWillDisappear(UPPageModelController pageModelController)
        {
            if (this.disappearingOrganizer)
            {
                return;
            }

            if (pageModelController is SerialEntryPageModelController)
            {
                ((SerialEntryPageModelController)pageModelController).SaveChangedPosition(null);
            }
            else if (pageModelController is EditPageModelController)
            {
            }

            this.currentModelController = null;
        }

        /// <summary>
        /// Performed when the model controller view will appear.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        public override void PageModelControllerViewWillAppear(UPPageModelController pageModelController)
        {
            this.currentModelController = pageModelController;
        }

        /// <summary>
        /// Observes the value for key path of object change context.
        /// </summary>
        /// <param name="keyPath">The key path.</param>
        /// <param name="theObject">The object.</param>
        /// <param name="change">The change.</param>
        /// <param name="context">The context.</param>
        public void ObserveValueForKeyPathOfObjectChangeContext(string keyPath, object theObject, Dictionary<string, object> change, object context)
        {
            if (keyPath == "hasRunningChangeRequests")
            {
                SerialEntryPageModelController modelController = (SerialEntryPageModelController)theObject;
                this.PageModelControllerSetContextValueForKey(modelController, modelController.ChangeRequestError, "organizerError");
                if (modelController.ChangeRequestError != null && !this.closeOnErrors)
                {
                    this.isSaving = false;
                    if (this.organizerDealsWithSaveErrors)
                    {
                        this.Organizer.Status = null;
                        modelController.Page.Status = null;
                        modelController.Page.Invalid = true;
                        this.RedisplayPage(null, true);
                    }
                    else if (this.CloseOrganizerDelegate != null)
                    {
                        this._modelControllerToRedraw = modelController;
                    }

                    if (this.organizerDealsWithSaveErrors || this.CloseOrganizerDelegate != null)
                    {
                        this.organizerDealsWithSaveErrors = false;
                        if (modelController.SerialEntry.ConflictHandling)
                        {
                            this.currentModelController = modelController;
                            this.RedisplayPage(null, true);
                            return;
                        }
                        else
                        {
                            this.serialEntryPageModelController = modelController;
                            //UIAlertView alertview = new UIAlertView(LocalizationKeys.upTextYouMadeChanges, LocalizationKeys.upTextProcessSerialEntrySaveErrors, this, LocalizationKeys.upTextOK, LocalizationKeys.upTextBasicDiscardChanges, null);
                            //this.isErrorSaveDialog = true;
                            //alertview.Show();
                        }

                        return;
                    }
                }

                foreach (UPPageModelController pageModelController in this.PageModelControllers)
                {
                    var controller = pageModelController as SerialEntryWebContentModelController;
                    if (controller != null)
                    {
                        controller.Page.Invalid = true;
                        controller.ShouldWaitForPendingChanges = modelController.HasRunningChangeRequests;
                    }
                }

                if (this.isSaving && !modelController.HasRunningChangeRequests)
                {
                    this.organizerDealsWithSaveErrors = false;
                    this.waitForSerialEntrySave = false;
                    this.isSaving = false;
                    if (!this.waitForSave)
                    {
                        if (this.CloseOrganizerDelegate != null)
                        {
                            UPMCloseOrganizerDelegate del = this.CloseOrganizerDelegate;
                            this.CloseOrganizerDelegate = null;
                            del.UpOrganizerModelControllerAllowedClosingOrganizer(this);
                        }
                        else
                        {
                            this.HandleChangeManager();
                            this.HandleSaved();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Closes the organizer allowed with delegate.
        /// </summary>
        /// <param name="closeOrganizerDelegate">The close organizer delegate.</param>
        /// <returns></returns>
        public override bool CloseOrganizerAllowedWithDelegate(UPMCloseOrganizerDelegate closeOrganizerDelegate)
        {
            if (this.ConflictHandlingMode)
            {
                return true;
            }

            this.CloseOrganizerDelegate = closeOrganizerDelegate;
            this.finishActionName = null;

            UPSerialEntryFinalSaveResult finalSaveResult = this.ExecuteFinalSave();
            switch (finalSaveResult)
            {
                case UPSerialEntryFinalSaveResult.Abort:
                    this.CloseOrganizerDelegate = null;
                    return false;

                case UPSerialEntryFinalSaveResult.Finished:
                    this.CloseOrganizerDelegate = null;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Alerts the index of the view did dismiss with button.
        /// </summary>
        /// <param name="alertView">The alert view.</param>
        /// <param name="buttonIndex">Index of the button.</param>
        public void AlertViewDidDismissWithButtonIndex(/*UIAlertView*/ object alertView, int buttonIndex)
        {
            if (this.isErrorSaveDialog)
            {
                if (buttonIndex == 1)
                {
                    this.waitForSerialEntrySave = false;
                    this.isSaving = false;
                    if (!this.waitForSave)
                    {
                        if (this.serialEntryPageModelController.SerialEntry.ChangedRecordsForEndingSerialEntry().Count > 0)
                        {
                            this.closeOnErrors = true;
                            this.isSaving = true;
                            this.serialEntryPageModelController.SaveWithDiscardedChanges();
                            return;
                        }

                        if (this.CloseOrganizerDelegate != null)
                        {
                            UPMCloseOrganizerDelegate del = this.CloseOrganizerDelegate;
                            this.CloseOrganizerDelegate = null;
                            del.UpOrganizerModelControllerAllowedClosingOrganizer(this);
                        }
                        else
                        {
                            this.HandleChangeManager();
                            this.HandleSaved();
                        }
                    }
                }
                else if (this.CloseOrganizerDelegate != null)
                {
                    this.Organizer.Status = null;
                    this.CloseOrganizerDelegate = null;
                    this._modelControllerToRedraw.Page.Status = null;
                    this._modelControllerToRedraw.Page.Invalid = true;
                    this.RedisplayPage(null, true);
                    this._modelControllerToRedraw = null;
                }
            }
        }

        private UPSerialEntryFinalSaveResult ExecuteFinalSave()
        {
            this.ModelControllerDelegate?.StopAllEditing();
            if (this.finalSaveCalled)
            {
                return UPSerialEntryFinalSaveResult.Finished;
            }

            bool hasViolations = false;
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                EditPageModelController editModelController = modelController as EditPageModelController;
                if (editModelController == null)
                {
                    continue;
                }

                if (editModelController.UpdatePageWithViolations())
                {
                    hasViolations = true;
                }
            }

            if (hasViolations)
            {
                return UPSerialEntryFinalSaveResult.Abort;
            }

            this.finalSaveCalled = true;
            this.DisableAllActionItems(true);
            if (this.currentModelController is SerialEntryPageModelController)
            {
                UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("savingIdentifier"));
                UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
                statusField.FieldValue = LocalizedString.TextWaitForChanges;
                stillLoadingError.StatusMessageField = statusField;
                this.Organizer.Status = stillLoadingError;
                this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, null);
            }

            bool noChanges = true;
            if (!this.IsNew || !this.created)
            {
                List<UPCRMRecord> changedRecords = this.ChangedRecords(false);
                if (changedRecords?.Count > 0)
                {
                    this.EditRecordRequest.TitleLine = this.Organizer.TitleText;
                    this.EditRecordRequest.DetailsLine = this.Organizer.SubtitleText;
                    this.waitForSave = true;
                    noChanges = false;
                    this.EditRecordRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, changedRecords, this);
                }
            }

            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                if (modelController is SerialEntryPageModelController)
                {
                    SerialEntryPageModelController serialEntryModelController = (SerialEntryPageModelController)modelController;
                    this.waitForSerialEntrySave = true;
                    this.isSaving = true;
                    if (!serialEntryModelController.SaveAll())
                    {
                        this.waitForSerialEntrySave = false;
                        this.isSaving = false;
                    }
                    else
                    {
                        noChanges = false;
                    }
                }
            }

            if (noChanges)
            {
                this.HandleChangeManager();
                this.HandleSaved();
                return UPSerialEntryFinalSaveResult.Finished;
            }

            return UPSerialEntryFinalSaveResult.Pending;
        }

        private void RedisplayPage(UPPageModelController modelController, bool showErrors)
        {
            if (modelController == null)
            {
                modelController = this.currentModelController;
            }

            this.finalSaveCalled = false;
            List<IIdentifier> changedIdentifiers = new List<IIdentifier>();
            foreach (UPMElement actionElement in this.RightNaviagtionBarItems)
            {
                if (actionElement is UPMOrganizerAction)
                {
                    ((UPMOrganizerAction)actionElement).Enabled = true;
                    changedIdentifiers.Add(actionElement.Identifier);
                }
                else if (actionElement is UPMOrganizerActionGroup)
                {
                    ((UPMOrganizerActionGroup)actionElement).Enabled = true;
                    changedIdentifiers.Add(actionElement.Identifier);
                }
            }

            this.ModelControllerDelegate.ModelControllerDidChange(this, this.TopLevelElement, this.TopLevelElement, changedIdentifiers, null);
            if (showErrors && modelController is SerialEntryPageModelController)
            {
                ((SerialEntryPageModelController)modelController).PositionsWithError();
            }
            else if (modelController is EditPageModelController)
            {
                modelController.ModelControllerDelegate.ModelControllerDidChange(modelController, modelController.TopLevelElement, modelController.TopLevelElement, null, null);
            }
        }

        private void HandleSaved()
        {
            this.disappearingOrganizer = true;
            var savedActionName = !string.IsNullOrEmpty(this.finishActionName) ? this.finishActionName : this.ViewReference.ContextValueForKey("SavedAction");

            if (!string.IsNullOrEmpty(savedActionName))
            {
                if (this.ConflictHandlingMode)
                {
                    savedActionName = string.Empty;
                }
                else
                {
                    foreach (UPPageModelController pageModelController in this.PageModelControllers)
                    {
                        if (pageModelController is SerialEntryPageModelController
                            && ((SerialEntryPageModelController)pageModelController).RightsFilterRevocation)
                        {
                            savedActionName = string.Empty;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(savedActionName))
            {
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                if (savedActionName.StartsWith("Button:"))
                {
                    UPConfigButton button = configStore.ButtonByName(savedActionName.Substring(7));
                    this.SaveViewReference = button.ViewReference;
                }
                else if (savedActionName.StartsWith("Menu:"))
                {
                    Menu menu = configStore.MenuByName(savedActionName.Substring(5));
                    this.SaveViewReference = menu.ViewReference;
                }
                else if (savedActionName == "Return")
                {
                    this.PopToPrevious = true;
                }
                else
                {
                    Menu menu = configStore.MenuByName(savedActionName);
                    this.SaveViewReference = menu.ViewReference;
                }

                this.ExecuteSavedActionWithRecordIdentification((string)this.PageModelControllerContextValueForKey(null, "RootRecordIdentification"), false);
            }
            else
            {
                if (this.PopToRootOnSave)
                {
                    this.ModelControllerDelegate.PopToRootContentViewController();
                }
                else
                {
                    this.PopToPreviousContentViewController();
                }
            }
        }
    }
}
