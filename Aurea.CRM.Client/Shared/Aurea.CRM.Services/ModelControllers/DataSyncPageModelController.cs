// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSyncPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Data Sync Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Pages;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Views;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Data Sync Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.UPPageModelController" />
    public class UPDataSyncPageModelController : UPPageModelController, IUPDataSyncPageModelController
    {
        private string syncMessage;
        private int currentStep;
        private int totalSteps;
        private int lastSyncConflictOrganizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPDataSyncPageModelController"/> class.
        /// </summary>
        public UPDataSyncPageModelController()
            : base(null)
        {
            this.BuildPage();

            Messenger.Default.Register<SyncManagerMessage>(this, this.SyncStatusChanged);

            this.lastSyncConflictOrganizer = UPMultipleOrganizerManager.UPORGANIZER_MANAGER_NO_NAV_CONTROLLER;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPDataSyncPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPDataSyncPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
        }

        /// <summary>
        /// Gets the data synchronize page.
        /// </summary>
        /// <value>
        /// The data synchronize page.
        /// </value>
        public IUPMDataSyncPage DataSyncPage => (UPMDataSyncPage)this.TopLevelElement;

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The <see cref="UPMElement" />.
        /// </returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            return (UPMDataSyncPage)this.TopLevelElement;
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        private void BuildPage()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPMDataSyncPage page = new UPMDataSyncPage(null);
            if (ServerSession.CurrentSession == null)
            {
                return;
            }

            UPSyncManager syncManager = ServerSession.CurrentSession.SyncManager;
            page.LastSyncDate = syncManager.LastSyncDate;
            page.CanStartIncrementalSync = true;
            this.syncMessage = string.Empty;

            if (syncManager.FullSyncRunning)
            {
                this.syncMessage = LocalizedString.Localize(
                    LocalizationKeys.TextGroupBasic,
                    LocalizationKeys.KeyBasicSyncOrganizerFullSyncRunningMessage);

                page.CanStartIncrementalSync = false;
            }
            else if (syncManager.IncrementalSyncRunning)
            {
                this.syncMessage = LocalizedString.Localize(
                    LocalizationKeys.TextGroupBasic,
                    LocalizationKeys.KeyBasicSyncOrganizerIncrementalSyncRunningMessage);

                page.CanStartIncrementalSync = false;
            }
            else if (syncManager.UpSyncRunning)
            {
                this.syncMessage = LocalizedString.Localize(
                    LocalizationKeys.TextGroupBasic,
                    LocalizationKeys.KeyBasicSyncOrganizerUpSyncRunningMessage);

                page.CanStartIncrementalSync = false;
            }
            else if (syncManager.MetadataSyncRunning)
            {
                this.syncMessage = LocalizedString.Localize(
                    LocalizationKeys.TextGroupBasic,
                    LocalizationKeys.KeyBasicSyncOrganizerConfigurationSyncRunningMessage);

                page.CanStartIncrementalSync = false;
            }
            else if (syncManager.ResourcesSyncRunning)
            {
                this.syncMessage = "Resources Sync is running";
                page.CanStartIncrementalSync = false;
            }

            page.CanPerformConfigurationSync = configStore.ConfigValueIsSetDefaultValue("Sync.ConfigOnOff", true);
            page.CanPerformLanguageChange = configStore.ConfigValueIsSetDefaultValue("Sync.LanguageOnOff", true);
            page.FullSyncRequirementStatus = syncManager.FullSyncRequirementStatus;

            switch (page.FullSyncRequirementStatus)
            {
                case FullSyncRequirementStatus.Recommended:
                    DateTime blockOnlineAccessDate = DateExtensions.AddTimeSpanToUtcNow(syncManager.TimeIntervalFromNowUntilOfflineOnline());
                    page.FullSyncRequirementStatusText = string.Format(
                        LocalizedString.Localize(
                            LocalizationKeys.TextGroupBasic,
                            LocalizationKeys.KeyBasicSyncOrganizerFullSyncRecommendedWarningMessage).ToDotnetStringFormatTemplate(),
                            blockOnlineAccessDate.ToString("d"));
                    break;

                case FullSyncRequirementStatus.MandatoryForOnlineAccess:
                    DateTime blockAccessDate = DateExtensions.AddTimeSpanToUtcNow(syncManager.TimeIntervalFromNowUntilBlock());
                    page.FullSyncRequirementStatusText = string.Format(
                        LocalizedString.Localize(
                            LocalizationKeys.TextGroupBasic,
                            LocalizationKeys.KeyBasicSyncOrganizerFullSyncMandatoryForOnlineErrorMessage).ToDotnetStringFormatTemplate(),
                            blockAccessDate.ToString("d"));
                    break;

                default:
                    page.FullSyncRequirementStatusText = string.Empty;
                    break;
            }

            this.TopLevelElement = page;
            this.BuildSyncMessage();
        }

        /// <summary>
        /// Builds the synchronize message.
        /// </summary>
        private void BuildSyncMessage()
        {
            UPMDataSyncPage page = (UPMDataSyncPage)this.TopLevelElement;
            page.CurrentSyncMessage = this.totalSteps > 0 ? $"{this.syncMessage} ({this.currentStep}/{this.totalSteps})" : this.syncMessage;
        }

        /// <summary>
        /// Views will appear handler.
        /// </summary>
        public override void ViewWillAppear()
        {
            //this.idleTimeManager.Activate();
            UPSyncManager syncManager = ServerSession.CurrentSession.SyncManager;
            if (syncManager.FullSyncRequirementStatus == FullSyncRequirementStatus.Resumable)
            {
                //UIAlertView alert = new UIAlertView("Full sync resumable", "Resume?", null, "No", "Yes", null);
                //alert.Show();
            }
        }

        /// <summary>
        /// View will disappear handler.
        /// </summary>
        public override void ViewWillDisappear()
        {
            //this.idleTimeManager.Deactivate();
        }

        /// <summary>
        /// Checks the connectivity for full synchronize.
        /// </summary>
        /// <returns></returns>
        private bool CheckConnectivityForFullSync()
        {
            if (ServerSession.CurrentSession.ConnectedToServerForFullSync == false)
            {
                //await SimpleIoc.Default.GetInstance<IDialogService>()
                //    .ShowMessage(LocalizationKeys.upTextErrorActionNotPossible, LocalizationKeys.upTextErrorMessageNoInternet);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks the connectivity.
        /// </summary>
        /// <returns></returns>
        private bool CheckConnectivity()
        {
            if (!ServerSession.CurrentSession.ConnectedToServer)
            {
                //await SimpleIoc.Default.GetInstance<IDialogService>()
                //    .ShowMessage(LocalizationKeys.upTextErrorActionNotPossible, LocalizationKeys.upTextErrorMessageNoInternet);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Performs the full synchronize.
        /// </summary>
        public async void PerformFullSync()
        {
            UPMultipleOrganizerManager multiManager = UPMultipleOrganizerManager.CurrentOrganizerManager;
            if (multiManager.HasEditingNavController)
            {
               var result = await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(
                    LocalizedString.TextCloseOpenEditOrganizer,
                    LocalizedString.TextEditOrganizerWillBeClosed,
                    LocalizedString.TextOk,
                    LocalizedString.TextSwitchToEditOrganizer,
                    null);

                if (!result)
                {
                    return;
                }

                var navControllerKey = multiManager.EditingNavControllerId;
                multiManager.SwitchedToNavController(navControllerKey);
            }

            this.PerformFullSyncWithLanguage(null);
        }

        /// <summary>
        /// Performs the full synchronize with language.
        /// </summary>
        /// <param name="language">The language.</param>
        public void PerformFullSyncWithLanguage(ServerLanguage language)
        {
            if (this.CheckConnectivityForFullSync())
            {
                bool possibleCancel = true;
                IServerSession crmSession = ServerSession.CurrentSession;
                if (language != null && crmSession.LanguageKey != language.Key)
                {
                    crmSession.UpdateSessionWithNewLanguage(language);
                    possibleCancel = false;
                }
                else
                {
                    crmSession.SyncManager.PerformFullSync();
                }

                this.ModelControllerDelegate?.ShowModalSyncProgressViewWithPossibleCancelOperation(possibleCancel, 0);
                HistoryManager.DefaultHistoryManager.DeleteHistory();
                HistoryManager.ReleaseHistoryManager();
            }
        }

        /// <summary>
        /// Performs the data refresh.
        /// </summary>
        public void PerformDataRefresh()
        {
            if (this.CheckConnectivity())
            {
                UPSyncManager syncManager = ServerSession.CurrentSession.SyncManager;
                syncManager.PerformIncrementalSync();
            }
        }

        /// <summary>
        /// Performs the metadata refresh.
        /// </summary>
        public void PerformMetadataRefresh()
        {
            if (this.CheckConnectivity())
            {
                UPSyncManager syncManager = ServerSession.CurrentSession.SyncManager;
                syncManager.PerformMetadataSync();
                HistoryManager.ReleaseHistoryManager();
            }
        }

        /// <summary>
        /// Performs the synchronize conflicts.
        /// </summary>
        public void PerformSyncConflicts()
        {
            UPMultipleOrganizerManager multiManager = UPMultipleOrganizerManager.CurrentOrganizerManager;
            if (this.lastSyncConflictOrganizer != UPMultipleOrganizerManager.UPORGANIZER_MANAGER_NO_NAV_CONTROLLER
                && multiManager.HasNavControllerWithId(this.lastSyncConflictOrganizer))
            {
                multiManager.SwitchedToNavController(this.lastSyncConflictOrganizer);
            }
            else
            {
                UPSyncConflictsOrganizerModelController syncConflictsOrganizerModelController = new UPSyncConflictsOrganizerModelController();
                this.lastSyncConflictOrganizer = this.ModelControllerDelegate.TransitionToContentModelController(syncConflictsOrganizerModelController, MultiOrganizerMode.AlwaysNewWorkingOrganizer);
            }
        }

        private void SyncStatusChanged(SyncManagerMessage notification)
        {
            switch (notification.MessageKey)
            {
                case SyncManagerMessageKey.DidStartFullSync:
                case SyncManagerMessageKey.DidFinishFullSync:
                case SyncManagerMessageKey.DidFailFullSync:
                case SyncManagerMessageKey.DidStartIncrementalSync:
                case SyncManagerMessageKey.DidFinishIncrementalSync:
                case SyncManagerMessageKey.DidFailIncrementalSync:
                case SyncManagerMessageKey.DidStartUpSync:
                case SyncManagerMessageKey.DidFinishUpSync:
                case SyncManagerMessageKey.DidFailUpSync:
                case SyncManagerMessageKey.DidStartMetadataSync:
                case SyncManagerMessageKey.DidFinishMetadataSync:
                case SyncManagerMessageKey.DidFailMetadataSync:
                case SyncManagerMessageKey.DidCancelFullSync:
                case SyncManagerMessageKey.DidStartResourcesSync:
                case SyncManagerMessageKey.DidFinishResourcesSync:
                case SyncManagerMessageKey.DidProgressSync:
                    {
                        this.currentStep = 0;
                        this.totalSteps = 0;

                        this.BuildPage();

                        var oldPage = this.TopLevelElement as UPMDataSyncPage;

                        if (oldPage != null)
                        {
                            this.InformAboutDidChangeTopLevelElement(oldPage, this.TopLevelElement, null, null);
                        }

                        break;
                    }

                default:
                    break;
            }
        }

        private void SyncProvidedProgress(object notification)
        {
            //this.currentStep = notification.UserInfo.ObjectForKey(kUPSyncManagerCurrentStepNumber).IntegerValue();
            //this.totalSteps = notification.UserInfo.ObjectForKey(kUPSyncManagerTotalStepNumber).IntegerValue();
            this.BuildSyncMessage();
        }

        //void AlertViewDidDismissWithButtonIndex(UIAlertView _alertView, int buttonIndex)
        //{
        //    this.alertView = null;
        //}
    }
}
