// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncConflictsDetailOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The Sync Conflicts Details Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Aurea.CRM.Core.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using Aurea.CRM.Core.Configuration;
    using UIModel;
    using UIModel.Identifiers;

    /// <summary>
    /// The Sync Conflicts Details Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    public class UPSyncConflictsDetailOrganizerModelController : UPOrganizerModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncConflictsDetailOrganizerModelController"/> class.
        /// </summary>
        /// <param name="syncConflict">The synchronize conflict.</param>
        public UPSyncConflictsDetailOrganizerModelController(UPMSyncConflict syncConflict)
            : base(null, new UPOrganizerInitOptions(true, true))
        {
            this.SyncConflict = syncConflict;
            this.BuildPagesFromViewReference();

            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                SyncConflictsPageModelController ctrl = pageModelController as SyncConflictsPageModelController;
                ctrl?.Page.AddChild(this.SyncConflict);
            }
        }

        /// <summary>
        /// Gets the synchronize conflict
        /// </summary>
        public UPMSyncConflict SyncConflict { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is edit organizer.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is edit organizer; otherwise, <c>false</c>.
        /// </value>
        public override bool IsEditOrganizer => false;

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public override void BuildPagesFromViewReference()
        {
            SyncConflictsPageModelController pageModelController = new SyncConflictsPageModelController();
            UPMOrganizer syncConflictOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("SyncConflictOrganizer"))
            {
                ExpandFound = true,
                DisplaysTitleText = true,
                LineCountAdditionalTitletext = 1,
                SubtitleText = LocalizedString.TextSyncConflictsTitle,
                TitleText = this.SyncConflict.MainField.StringValue,
                AdditionalTitleText = (string)this.SyncConflict.DetailField?.FieldValue
            };

            this.ShouldShowTabsForSingleTab = false;
            if (this.IsDiscardAllowed())
            {
                UPMOrganizerAction discardAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.discard"))
                {
                    LabelText = LocalizedString.Localize(LocalizationKeys.TextGroupBasic,
                        LocalizationKeys.KeyBasicSyncConflictsActionDiscardChanges, "DISCARD CONFLICT"),
                    IconName = "Icon:Trash"
                };

                discardAction.SetTargetAction(this, this.DiscardError);
                this.AddOrganizerHeaderActionItem(discardAction);
            }

            this.AddPageModelController(pageModelController);
            syncConflictOrganizer.AddPage(pageModelController.Page);
            this.TopLevelElement = syncConflictOrganizer;
        }

        /// <summary>
        /// Discards the error.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        private void DiscardError(object actionDictionary)
        {
            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                SyncConflictsPageModelController ctrl = pageModelController as SyncConflictsPageModelController;
                ctrl?.DiscardChangesForConflictAsync(this.SyncConflict);
            }

        }

        /// <summary>
        /// Reports the error.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        private void ReportError(object actionDictionary)
        {
            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                SyncConflictsPageModelController ctrl = pageModelController as SyncConflictsPageModelController;
                ctrl?.ReportErrorForConflict(this.SyncConflict);
            }
        }

        private bool IsDiscardAllowed()
        {
            var menu = ConfigurationUnitStore.DefaultStore?.MenuByName("SyncConflicts");
            return menu?.ViewReference?.ContextValueIsSet("AllowDeleting") ?? false;
        }
    }
}
