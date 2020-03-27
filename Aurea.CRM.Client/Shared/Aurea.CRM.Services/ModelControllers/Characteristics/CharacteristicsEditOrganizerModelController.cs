// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacteristicsEditOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The CharacteristicsEditOrganizerModelController
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Characteristics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Characteristics;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using GalaSoft.MvvmLight.Views;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The charateristics edit view configuration name
        /// </summary>
        public const string CharateristicsEditViewConfigurationName = "CharacteristicsEditView";
    }

    /// <summary>
    /// UPCharacteristicsEditOrganizerModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    public class UPCharacteristicsEditOrganizerModelController : UPOrganizerModelController
    {
        private UPOfflineCharacteristicsRequest conflictEditOfflineRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristicsEditOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public UPCharacteristicsEditOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options = null)
            : base(viewReference, options)
        {
        }

        /// <summary>
        /// Creates the controller with offline request.
        /// </summary>
        /// <param name="offlineRequest">The offline request.</param>
        /// <returns></returns>
        public static UPCharacteristicsEditOrganizerModelController Create(UPOfflineCharacteristicsRequest offlineRequest)
        {
            ViewReference viewReference = new ViewReference(offlineRequest.Json, Constants.CharateristicsEditViewConfigurationName);
            string recordIdentification = viewReference.ContextValueForKey("RecordId");
            if (string.IsNullOrEmpty(recordIdentification) || recordIdentification.Contains("new"))
            {
                UPCRMRecord firstRecord = offlineRequest.FirstRecord;
                if (firstRecord?.Links != null)
                {
                    foreach (UPCRMLink link in firstRecord.Links)
                    {
                        if (link.InfoAreaId == recordIdentification.InfoAreaId())
                        {
                            viewReference = new ViewReference(viewReference, recordIdentification,
                                link.RecordIdentification, null);
                        }
                    }
                }
            }

            return new UPCharacteristicsEditOrganizerModelController(offlineRequest, viewReference, UPOrganizerInitOptions.AddNoAutoBuildToOptions(null));
        }

        private UPCharacteristicsEditOrganizerModelController(UPOfflineCharacteristicsRequest offlineRequest, ViewReference viewReference, UPOrganizerInitOptions options)
            : base(viewReference, options)
        {
            this.conflictEditOfflineRequest = offlineRequest;
            this.BuildPagesFromViewReference();
        }

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public override void BuildPagesFromViewReference()
        {
            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            this.ContinueBuildPagesFromViewReference();
        }

        /// <summary>
        /// Saves the specified action dictionary.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public void Save(object actionDictionary)
        {
            List<UPCRMRecord> changedRecords = null;
            UPCharacteristics characteristics = null;
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                var characteristicsEditPageModelController = modelController as UPCharacteristicsEditPageModelController;
                if (characteristicsEditPageModelController != null)
                {
                    changedRecords = characteristicsEditPageModelController.ChangedRecords();
                    characteristics = characteristicsEditPageModelController.Characteristics;
                    break;
                }
            }

            if (this.conflictEditOfflineRequest != null)
            {
                bool onlineRequestNecessary = changedRecords?.Any(record => !(record.Deleted && record.IsNew)) ?? false;

                if (onlineRequestNecessary)
                {
                    this.conflictEditOfflineRequest.StartRequest(UPOfflineRequestMode.OnlineOnly, changedRecords, this);
                }
                else
                {
                    this.conflictEditOfflineRequest.DeleteRequest(true);
                    this.ModelControllerDelegate.PopToRootContentViewController();
                }

                return;
            }

            if (changedRecords == null || changedRecords.Count == 0)
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
            characteristics.OfflineRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, changedRecords, this);
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

        /// <summary>
        /// Offlines the request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public override void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            this.DisableAllActionItems(false);
            this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
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
            List<UPCRMRecord> records = (List<UPCRMRecord>)data;
            List<IIdentifier> changes = new List<IIdentifier>();
            changes.AddRange(records.Select(record => new RecordIdentifier(record.InfoAreaId, record.RecordId)));

            if (!string.IsNullOrEmpty(this.RecordIdentification))
            {
                changes.Add(new RecordIdentifier(this.RecordIdentification));
            }

            if (changes.Count > 0)
            {
                UPChangeManager.CurrentChangeManager.RegisterChanges(changes);
            }

            if (this.conflictEditOfflineRequest != null)
            {
                this.ModelControllerDelegate.PopToRootContentViewController();
            }
            else
            {
                this.ModelControllerDelegate.PopToPreviousContentViewController();
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
                //UIAlertView alertview = new UIAlertView(LocalizationKeys.upTextYouMadeChanges, LocalizationKeys.upTextReallyAbortAndLoseChanges, this, LocalizationKeys.upTextNO, LocalizationKeys.upTextYES, null);
                //alertview.Show();
                return false;
            }

            return true;
        }

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

        private void ContinueBuildPagesFromViewReference()
        {
            UPCharacteristicsEditPageModelController pageModelController = new UPCharacteristicsEditPageModelController(this.ViewReference, this.RecordIdentification, (UPOfflineCharacteristicsRequest)this.conflictEditOfflineRequest);
            UPMOrganizer organizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("EditCharacteristics"));
            string infoAreaId = this.RecordIdentification.InfoAreaId();
            organizer.TitleText = LocalizedString.TextEditCharacteristics;
            UPConfigTableCaption tableCaption = ConfigurationUnitStore.DefaultStore.TableCaptionByName(this.RecordIdentification.InfoAreaId());
            organizer.TitleText = tableCaption?.TableCaptionForRecordIdentification(this.RecordIdentification);
            InfoArea infoAreaConfig = ConfigurationUnitStore.DefaultStore.InfoAreaConfigById(infoAreaId);
            organizer.SubtitleText = infoAreaConfig?.SingularName;
            if (string.IsNullOrEmpty(organizer.SubtitleText))
            {
                organizer.SubtitleText = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(infoAreaId).Label;
            }

            Page page = pageModelController.Page;
            page.LabelText = "Error";
            this.AddPageModelController(pageModelController);
            organizer.AddPage(page);
            this.TopLevelElement = organizer;
            organizer.ExpandFound = true;
            this.AddOrganizerActions();
        }

        private bool HasChanges()
        {
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                var characteristicsEditPageModelController = modelController as UPCharacteristicsEditPageModelController;

                if (characteristicsEditPageModelController?.ChangedRecords()?.Count > 0)
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
            }
        }

        private void NoChangesWhileSaving()
        {
            this.ModelControllerDelegate.PopToPreviousContentViewController();
        }
    }
}
