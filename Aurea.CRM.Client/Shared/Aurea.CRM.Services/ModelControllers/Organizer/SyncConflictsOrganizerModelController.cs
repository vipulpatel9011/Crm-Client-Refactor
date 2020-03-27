// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncConflictsOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The Sync Conflict Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Sync Conflicts Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    public class UPSyncConflictsOrganizerModelController : UPOrganizerModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncConflictsOrganizerModelController"/> class.
        /// </summary>
        public UPSyncConflictsOrganizerModelController()
            : base(ConfigurationUnitStore.DefaultStore.MenuByName("SyncConflicts")?.ViewReference)
        {
            this.Organizer.SubtitleText = LocalizedString.TextSyncConflictsTitle;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is edit organizer.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is edit organizer; otherwise, <c>false</c>.
        /// </value>
        public override bool IsEditOrganizer => false;

        /// <summary>
        /// Gets the synchronize page model controller.
        /// </summary>
        /// <value>
        /// The synchronize page model controller.
        /// </value>
        public SyncConflictsPageModelController SyncPageModelController => this.PageModelControllers.OfType<SyncConflictsPageModelController>().FirstOrDefault();

        /// <summary>
        /// Gets a value indicating whether [close organizer when leaving].
        /// </summary>
        /// <value>
        /// <c>true</c> if the multipleOrganizerManager will close this organizer when switchted to another.
        /// </value>
        public override bool CloseOrganizerWhenLeaving
        {
            get
            {
                SyncConflictsPageModelController pageModelController = this.SyncPageModelController;
                if (pageModelController != null && !pageModelController.SyncConflictsPage.Invalid)
                {
                    return pageModelController.SyncConflictsPage.NumberOfSyncConflicts == 0;
                }

                return false;
            }
        }

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public override void BuildPagesFromViewReference()
        {
            SyncConflictsPageModelController pageModelController = new SyncConflictsPageModelController();
            UPMOrganizer syncConflictOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("SyncConflictsOrganizer"))
            {
                ExpandFound = true,
                SubtitleText = LocalizedString.TextSyncConflictsTitle
            };
            this.ShouldShowTabsForSingleTab = false;
            UPMOrganizerAction retryAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.retry"));
            UPMOrganizerAction reportErrorAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.report"));
            string retryText = LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncConflictsRetryAll, "Retry All");
            retryAction.LabelText = retryText;
            retryAction.IconName = "Icon:Record";
            retryAction.SetTargetAction(this, this.RetryAll);
            this.AddOrganizerHeaderActionItem(retryAction);
            IOfflineStorage offlineStorage = UPOfflineStorage.DefaultStorage;
            if (!string.IsNullOrEmpty(ConfigurationUnitStore.DefaultStore.ConfigValue("Sync.ConflictEmailAddress"))
                && offlineStorage.ConflictRequests?.Count > 1
                && !string.IsNullOrEmpty(offlineStorage.OfflineRequestXml()))
            {
                string sendRequestXmlText = LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncConflictsReportAllErrors, "Report all errors");
                reportErrorAction.LabelText = sendRequestXmlText;
                reportErrorAction.SetTargetAction(this, this.ReportAllErrors);
                this.AddOrganizerHeaderActionItem(reportErrorAction);
            }

            this.AddPageModelController(pageModelController);
            syncConflictOrganizer.AddPage(pageModelController.Page);
            this.TopLevelElement = syncConflictOrganizer;
        }

        /// <summary>
        /// Retries all.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        private void RetryAll(object actionDictionary)
        {
            SyncConflictsPageModelController syncPageModelController = this.SyncPageModelController;
            syncPageModelController.RetryAllConflicts();
        }

        /// <summary>
        /// Reports all errors.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        private void ReportAllErrors(object actionDictionary)
        {
            SyncConflictsPageModelController syncPageModelController = this.SyncPageModelController;
            syncPageModelController.ReportAllError();
        }
    }
}
