// <copyright file="ContactTimesOrganizerModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.ModelControllers.ContactTimes
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Contact times organizer model controller implementation
    /// </summary>
    public class ContactTimesOrganizerModelController : EditOrganizerModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactTimesOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">View reference</param>
        /// <param name="options">Options</param>
        public ContactTimesOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options)
            : base(viewReference, options)
        {
            this.BuildEditOrganizerPage();
        }

        /// <inheritdoc/>
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

        private void BuildEditOrganizerPage()
        {
            var organizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("EditQuestionnaire"))
            {
                ExpandFound = true
            };
            this.TopLevelElement = organizer;
            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            this.InfoAreaId = this.RecordIdentification.InfoAreaId();
            var tableCaption = ConfigurationUnitStore.DefaultStore.TableCaptionByName(this.RecordIdentification.InfoAreaId());
            organizer.TitleText = tableCaption.TableCaptionForRecordIdentification(this.RecordIdentification);
            organizer.SubtitleText = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(this.InfoAreaId).Label;

            var pageModelController = new ContactTimesEditPageModelController(this.ViewReference);
            this.AddPageModelController(pageModelController);
            organizer.AddPage(pageModelController.Page);
            this.AddOrganizerActions();
        }
    }
}
