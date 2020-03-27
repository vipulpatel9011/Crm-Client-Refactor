// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialEntryOrganizerWithoutRootModelController.cs" company="Aurea Software Gmbh">
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
//   SerialEntryOrganizerWithoutRootModelController
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.SerialEntry
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// SerialEntryOrganizerWithoutRootModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.SerialEntry.SerialEntryOrganizerModelController" />
    public class SerialEntryOrganizerWithoutRootModelController : SerialEntryOrganizerModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerialEntryOrganizerWithoutRootModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public SerialEntryOrganizerWithoutRootModelController(ViewReference viewReference, UPOrganizerInitOptions options)
            : this(viewReference)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialEntryOrganizerWithoutRootModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="buildPageModelControllers">if set to <c>true</c> [build page model controllers].</param>
        public SerialEntryOrganizerWithoutRootModelController(ViewReference viewReference, bool buildPageModelControllers = true)
            : base(viewReference)
        {
            this.ShouldShowTabsForSingleTab = false;
            string configName = viewReference.ContextValueForKey("DestinationConfigName");
            UPConfigExpand expandConfig = ConfigurationUnitStore.DefaultStore.ExpandByName(configName);
            if (expandConfig != null)
            {
                this.ExpandConfig = expandConfig;
            }

            if (buildPageModelControllers)
            {
                this.AddPageModelControllers();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialEntryOrganizerWithoutRootModelController"/> class.
        /// </summary>
        /// <param name="offlineRequest">The offline request.</param>
        public SerialEntryOrganizerWithoutRootModelController(UPOfflineSerialEntryRequest offlineRequest)
            : this(new ViewReference(offlineRequest.Json, "SerialEntry"), false)
        {
            this.OfflineRequest = offlineRequest;
            this.PopToRootOnSave = true;    // Sync Conflict Page need PopToRootView
            this.ConflictHandlingMode = true;

            this.AddPageModelControllers();
        }

        /// <summary>
        /// Adds the page model controllers.
        /// </summary>
        public override void AddPageModelControllers()
        {
            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            this.RecordIdentification = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(this.RecordIdentification);
            ViewReference viewReference = this.ViewReference;
            if (this.OfflineRequest != null)
            {
                if (this.RecordIdentification.Contains("new"))
                {
                    UPCRMRecord rootRecord = ((UPOfflineRecordRequest)this.OfflineRequest).FirstRecordWithInfoAreaId(this.RecordIdentification.InfoAreaId());
                    if (rootRecord != null)
                    {
                        viewReference = new ViewReference(this.ViewReference, this.RecordIdentification, rootRecord.RecordIdentification, null);
                        this.RecordIdentification = rootRecord.RecordIdentification;
                    }
                }
            }

            UPMOrganizer editOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("Edit"));
            this.TopLevelElement = editOrganizer;
            SerialEntryPageModelController tmpSerialEntryPageModelController = new SerialEntryPageModelController(viewReference, (UPOfflineSerialEntryRequest)this.OfflineRequest);
            //tmpSerialEntryPageModelController.AddObserverForKeyPathOptionsContext(this, "hasRunningChangeRequests", NSKeyValueObservingOptionNew, null);
            tmpSerialEntryPageModelController.Delegate = this;
            Page overviewPage = tmpSerialEntryPageModelController.Page;
            string organizerHeaderText;
            string organizerDetailText = null;

            if (this.ExpandConfig != null)
            {
                UPConfigHeader header = ConfigurationUnitStore.DefaultStore.HeaderByNameFromGroup("Edit", this.ExpandConfig.HeaderGroupName);
                if (header != null)
                {
                    organizerDetailText = header.Label;
                    tmpSerialEntryPageModelController.Page.LabelText = organizerDetailText;
                }
            }

            if (string.IsNullOrEmpty(organizerDetailText))
            {
                if (!string.IsNullOrEmpty(this.InfoAreaId))
                {
                    organizerDetailText = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(this.InfoAreaId).Label;
                }
                else
                {
                    organizerDetailText = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(this.RecordIdentification.InfoAreaId()).Label;
                }
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string rootRecordIdentification = this.RecordIdentification;
            if (string.IsNullOrEmpty(rootRecordIdentification))
            {
                rootRecordIdentification = this.LinkRecordIdentification;
            }

            UPConfigTableCaption tableCaption = configStore.TableCaptionByName(rootRecordIdentification.InfoAreaId());
            string recordTableCaption = null;
            if (tableCaption != null)
            {
                recordTableCaption = tableCaption.TableCaptionForRecordIdentification(rootRecordIdentification);
            }

            if (string.IsNullOrEmpty(recordTableCaption))
            {
                if (!string.IsNullOrEmpty(organizerDetailText))
                {
                    organizerHeaderText = organizerDetailText;
                    organizerDetailText = null;
                }
                else
                {
                    organizerHeaderText = rootRecordIdentification;
                }
            }
            else
            {
                organizerHeaderText = recordTableCaption;
            }

            this.Organizer.TitleText = organizerHeaderText;
            this.Organizer.SubtitleText = organizerDetailText;
            this.AddPageModelController(tmpSerialEntryPageModelController);
            this.Organizer.AddPage(overviewPage);
            this.AddRemainingPageModelController();
            this.AddOrganizerActions();
            editOrganizer.ExpandFound = true;
        }

        /// <summary>
        /// Adds the organizer actions.
        /// </summary>
        public override void AddOrganizerActions()
        {
            if (this.ConflictHandlingMode)
            {
                UPMOrganizerAction cancelAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.cancel"));
                cancelAction.SetTargetAction(this, this.Cancel);
                cancelAction.LabelText = LocalizedString.TextCancel;
                this.AddLeftNavigationBarActionItem(cancelAction);
            }

            this.RightAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.create"));
            this.RightAction.LabelText = this.OfflineRequest != null ? LocalizedString.TextSave : LocalizedString.TextClose;
            this.RightAction.SetTargetAction(this, this.Save);
            this.AddRightNavigationBarActionItem(this.RightAction);
        }
    }
}
