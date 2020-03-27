// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncConflictsPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Sync Conflicts Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Views;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Sync Conflicts Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.UPPageModelController" />
    /// <seealso cref="Aurea.CRM.Services.Delegates.ISwipePageDataSourceController" />
    public class SyncConflictsPageModelController : UPPageModelController, ISwipePageDataSourceController
    {
        /// <summary>
        /// The discarded synchronize coflict
        /// </summary>
        private UPMSyncConflict discardedSyncConflict;

        /// <summary>
        /// The swipe record controller start position
        /// </summary>
        private int swipeRecordControllerStartPosition;

        /// <summary>
        /// The old number of conflicts
        /// </summary>
        private int oldNumberOfConflicts;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncConflictsPageModelController"/> class.
        /// </summary>
        public SyncConflictsPageModelController()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncConflictsPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public SyncConflictsPageModelController(ViewReference viewReference)
            : base(null)
        {
            this.ShowAllOfflineRequests = false;
            this.AllowDeleting = true;
            this.oldNumberOfConflicts = -1;
            if (viewReference != null)
            {
                this.ShowAllOfflineRequests = viewReference.ContextValueIsSet("ShowAllOfflineRequests");

                this.AllowDeleting = viewReference.ContextValueIsSet("AllowDeleting");
            }

            this.BuildPage();
            Messenger.Default.Register<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishUpSync, this.SyncDidFinish);
        }

        /// <summary>
        /// Gets the synchronize conflicts page.
        /// </summary>
        /// <value>
        /// The synchronize conflicts page.
        /// </value>
        public UPMSyncConflictsPage SyncConflictsPage => (UPMSyncConflictsPage)this.Page;

        /// <summary>
        /// Gets a value indicating whether [show all offline requests].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show all offline requests]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAllOfflineRequests { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [allow deleting].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow deleting]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowDeleting { get; private set; }

        void Dealloc()
        {
            Messenger.Default.Unregister(this);
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public void BuildPage()
        {
            UPMSyncConflictsPage page = new UPMSyncConflictsPage(StringIdentifier.IdentifierWithStringId("SyncConflictsPage"));
            UPMProgressStatus progressStatus = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("savingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
            statusField.FieldValue = LocalizedString.TextLoadingData;
            progressStatus.StatusMessageField = statusField;
            page.Status = progressStatus;
            page.Invalid = true;
            this.TopLevelElement = page;
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The <see cref="UPMElement" />.
        /// </returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            UPMSyncConflictsPage page = new UPMSyncConflictsPage(StringIdentifier.IdentifierWithStringId("SyncConflictsPage"));
            page.SyncConflictEmail = ConfigurationUnitStore.DefaultStore.ConfigValue("Sync.ConflictEmailAddress");
            IOfflineStorage offlineStorage = UPOfflineStorage.DefaultStorage;
            var offlineRequests = this.ShowAllOfflineRequests ? offlineStorage.OfflineRequests : offlineStorage.ConflictRequests;

            if (offlineRequests == null || offlineRequests.Count == 0)
            {
                this.AddNoConflictsFoundPage(page);
                return page;
            }

            foreach (UPOfflineRequest request in offlineRequests)
            {
                IIdentifier identifier;
                if (!string.IsNullOrEmpty(request.IdentifyingRecordIdentification))
                {
                    identifier = new RecordIdentifier(request.IdentifyingRecordIdentification);
                }
                else
                {
                    identifier = StringIdentifier.IdentifierWithStringId($"request_{request.RequestNr}");
                }

                UPMSyncConflictWithContext syncConflict = new UPMSyncConflictWithContext(identifier);
                request.LoadFromOfflineStorage();
                syncConflict.OfflineRequest = request;
                syncConflict.CanBeFixed = request.FixableByUser;
                syncConflict.CanBeReported = !string.IsNullOrEmpty(page.SyncConflictEmail) && syncConflict.OfflineRequest.HasXml;
                if (!string.IsNullOrEmpty(request.ImageName))
                {
                    //SyncConflict.Icon = UIImage.ImageNamed(request.ImageName);    // CRM-5007
                }

                UPMStringField titleLineField = new UPMStringField(null);
                titleLineField.StringValue = request.TitleLine;
                syncConflict.MainField = titleLineField;
                string detailsLine = request.DetailsLine;
                if (!string.IsNullOrEmpty(detailsLine))
                {
                    UPMStringField detailsLineField = new UPMStringField(null);
                    detailsLineField.StringValue = detailsLine;
                    syncConflict.DetailField = detailsLineField;
                }

                if (!this.ShowAllOfflineRequests)
                {
                    UPMErrorStatus error = UPMErrorStatus.ErrorStatusWithMessageDetails(request.Error, request.Response);
                    syncConflict.AddStatus(error);
                }

                List<UPOfflineRequest> dependingRequests = request.DependentRequests;
                if (dependingRequests != null)
                {
                    foreach (UPOfflineRequest dependentRequest in dependingRequests)
                    {
                        string description = $"{dependentRequest.TitleLine}-{dependentRequest.DetailsLine}";
                        UPMWarnStatus warning = UPMWarnStatus.WarnStatusWithMessageDetails(description, null);
                        syncConflict.AddStatus(warning);
                    }
                }

                page.AddSyncConflict(syncConflict);
            }

            if (this.oldNumberOfConflicts >= 0 && this.oldNumberOfConflicts != offlineRequests.Count)
            {
                Messenger.Default.Send(SyncManagerMessage.Create(SyncManagerMessageKey.NumberOfConflictsChanged));
            }

            this.oldNumberOfConflicts = offlineRequests.Count;
            return page;
        }

        /// <summary>
        /// Adds the no conflicts found page.
        /// </summary>
        /// <param name="conflictPage">The conflict page.</param>
        public void AddNoConflictsFoundPage(UPMSyncConflictsPage conflictPage)
        {
            UPMInfoMessageStatus infoMessageStatus = new UPMInfoMessageStatus(StringIdentifier.IdentifierWithStringId("noConflictsFoundIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("noConflictsFoundFieldIdentifier"));
            statusField.FieldValue = LocalizedString.Localize(
                LocalizationKeys.TextGroupBasic,
                this.ShowAllOfflineRequests ? LocalizationKeys.KeyBasicSyncConflictsNoOfflineRecordsFound : LocalizationKeys.KeyBasicSyncConflictsNoConflictsFound);

            infoMessageStatus.DetailMessageField = statusField;
            conflictPage.Status = infoMessageStatus;
        }

        /// <summary>
        /// Reports the error for conflict.
        /// </summary>
        /// <param name="syncConflict">The synchronize conflict.</param>
        public void ReportErrorForConflict(UPMSyncConflict syncConflict)
        {
            UPMSyncConflictWithContext syncConflictWithContext = (UPMSyncConflictWithContext)syncConflict;
            this.SendReportForOfflineRequest(syncConflictWithContext.OfflineRequest);
        }

        /// <summary>
        /// Discards the changes for conflict.
        /// </summary>
        /// <param name="syncConflict">The synchronize conflict.</param>
        public async void DiscardChangesForConflictAsync(UPMSyncConflict syncConflict)
        {
            this.discardedSyncConflict = syncConflict;

            await SimpleIoc.Default.GetInstance<IDialogService>()
                .ShowMessage(
                LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncConflictsDiscardChangesMessage),
                LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncConflictsDiscardChangesTitle),
                LocalizedString.TextYes,
                LocalizedString.TextCancel,
                    c =>
                    {
                        if (c)
                        {
                            this.DoDiscard();
                            Messenger.Default.Send(
                                new NotificationMessage(NavigateToOrganizerMessage.NavigateBack),
                                NavigateToOrganizerMessage.NavigateBack);
                        }
                        else
                        {
                            this.discardedSyncConflict = null;
                        }
                    });
        }

        /// <summary>
        /// Edits the conflict for conflict.
        /// </summary>
        /// <param name="syncConflict">The synchronize conflict.</param>
        public void EditConflictForConflict(UPMSyncConflict syncConflict)
        {
            UPMSyncConflictWithContext syncConflictWithContext = (UPMSyncConflictWithContext)syncConflict;
            syncConflictWithContext.OfflineRequest.ConflictHandlingMode = true;
            UPOrganizerModelController organizerModelController = UPOrganizerModelController.OrganizerFromOfflineRequest(syncConflictWithContext.OfflineRequest);
            if (organizerModelController != null)
            {
                this.SyncConflictsPage.Invalid = true;
                this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController, MultiOrganizerMode.StayInCurrentOrganizer);
            }
        }

        /// <summary>
        /// Retries all conflicts.
        /// </summary>
        public void RetryAllConflicts()
        {
            UPOfflineStorage.DefaultStorage.ClearAllErrors();
            UPSyncManager syncManager = ServerSession.CurrentSession.SyncManager;
            syncManager.PerformUpSync();
        }

        /// <summary>
        /// Reports all error.
        /// </summary>
        public void ReportAllError()
        {
            this.SendReport(UPOfflineStorage.DefaultStorage.OfflineRequestXml());
        }

        /// <summary>
        /// Sends the report.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        public void SendReport(string xmlString)
        {
#if PORTING
            UPMail mail = new UPMail();
            mail.Subject = "Sync-conflict reports";
            mail.AddRecipient(((UPMSyncConflictsPage)this.Page).SyncConflictEmail);
            string filename = $"SyncConflicts_{ServerSession.CurrentSession().UserName}_{StringExtensions.CrmValueFromDate(DateTime.UtcNow)}.xml";
            UPMailAttachment attachment = new UPMailAttachment(xmlString, "application/xml", filename);
            mail.AddAttachment(attachment);
            this.ModelControllerDelegate.SendMailModal(mail, false);
#endif
        }

        /// <summary>
        /// Sends the report for offline request.
        /// </summary>
        /// <param name="request">The request.</param>
        void SendReportForOfflineRequest(UPOfflineRequest request)
        {
#if PORTING
            string xmlData = request.Xml();
            UPMail mail = new UPMail();
            mail.Subject = NSString.StringWithFormat("SyncConflict from %@ at %@ of type %@", ServerSession.CurrentSession().UserName, request.ServerDateTime, request.ProcessType);
            mail.AddRecipient(((UPMSyncConflictsPage)this.Page).SyncConflictEmail);
            string filename = NSString.StringWithFormat("SyncConflict_%@_%@_%ld.xml", ServerSession.CurrentSession().UserName, request.ProcessType, request.RequestNr);
            UPMailAttachment attachment = new UPMailAttachment(xmlData, "application/xml", filename);
            mail.AddAttachment(attachment);
            this.ModelControllerDelegate.SendMailModal(mail, false);
#endif
        }

        /// <summary>
        /// The synchronize did finish.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void SyncDidFinish(object notification)
        {
            Page oldPage = this.Page;
            this.BuildPage();
            this.InformAboutDidChangeTopLevelElement(oldPage, this.Page, null, null);
            this.UpdateElementForCurrentChanges(null);
        }

        /// <summary>
        /// Does the discard.
        /// </summary>
        private void DoDiscard()
        {
            UPMSyncConflictWithContext syncConflictWithContext = (UPMSyncConflictWithContext)this.discardedSyncConflict;
            if (syncConflictWithContext.OfflineRequest.DeleteRequest(true) != 0)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError("Offline request could not be deleted because of database error");
            }
            else
            {
                this.SyncConflictsPage.RemoveConflict(this.discardedSyncConflict);
                if (this.SyncConflictsPage.NumberOfSyncConflicts == 0)
                {
                    this.AddNoConflictsFoundPage(this.SyncConflictsPage);
                }

                this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, new List<IIdentifier> { this.discardedSyncConflict.Identifier }, UPChangeHints.ChangeHintsWithHint("DiscardConflict"));
                Messenger.Default.Send(SyncManagerMessage.Create(SyncManagerMessageKey.NumberOfConflictsChanged));
            }
        }

        /// <summary>
        /// Switches to detail.
        /// </summary>
        /// <param name="_syncConflict">The synchronize conflict.</param>
        public void SwitchToDetail(UPMSyncConflict _syncConflict)
        {
            var organizerModelController = new UPSyncConflictsDetailOrganizerModelController(_syncConflict);

            for (int index = 0; index < this.SyncConflictsPage.Children.Count; index++)
            {
                UPMSyncConflict syncConflict = this.SyncConflictsPage.SyncConflictAtIndex(index);
                if (syncConflict.Identifier.MatchesIdentifier(_syncConflict.Identifier))
                {
                    this.swipeRecordControllerStartPosition = index;
                    break;
                }
            }

            bool moreThanOneConflict = this.HasMoreThanOneConflict();
            if (moreThanOneConflict)
            {
                UPSearchResultCachingSwipeRecordController cachingRecordController = new UPSearchResultCachingSwipeRecordController(this);
                cachingRecordController.BuildCache();
                organizerModelController.ParentSwipePageRecordController = cachingRecordController;
            }

            this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
        }

        /// <summary>
        /// Determines whether [has more than one conflict].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has more than one conflict]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasMoreThanOneConflict()
        {
            return this.SyncConflictsPage.Children.Count > 1;
        }

        /// <summary>
        /// Loads the index of the table captions from index to index.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        /// <returns></returns>
        public List<UPSwipePageRecordItem> LoadTableCaptionsFromIndexToIndex(int fromIndex, int toIndex)
        {
            List<UPSwipePageRecordItem> result = new List<UPSwipePageRecordItem>();
            fromIndex += this.swipeRecordControllerStartPosition;
            toIndex += this.swipeRecordControllerStartPosition;
            for (int index = fromIndex; index < toIndex; index++)
            {
                UPMSyncConflict syncConflict = this.SyncConflictsPage.SyncConflictAtIndex(index);
                if (syncConflict != null)
                {
                    UPSwipePageRecordItem item = new UPSwipePageRecordItem((string)syncConflict.MainField.FieldValue,
                        (string)syncConflict.DetailField.FieldValue, syncConflict.Identifier.ToString(), false);
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// The detail organizer for record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="onlineData">
        /// The online data.
        /// </param>
        /// <returns>
        /// The <see cref="UPOrganizerModelController"/>.
        /// </returns>
        public UPOrganizerModelController DetailOrganizerForRecordIdentification(string recordIdentification, bool onlineData)
        {
            UPSyncConflictsDetailOrganizerModelController organizerModelController = null;
            for (int i = 0; i < this.SyncConflictsPage.Children.Count; i++)
            {
                UPMSyncConflict syncConflict = this.SyncConflictsPage.SyncConflictAtIndex(i);
                if (syncConflict.Identifier.ToString() == recordIdentification)
                {
                    organizerModelController = new UPSyncConflictsDetailOrganizerModelController(syncConflict)
                    {
                        NavControllerId = this.ParentOrganizerModelController.NavControllerId
                    };
                    break;
                }
            }

            return organizerModelController;
        }
    }
}
