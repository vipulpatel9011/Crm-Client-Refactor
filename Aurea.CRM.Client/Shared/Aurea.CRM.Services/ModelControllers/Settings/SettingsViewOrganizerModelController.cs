// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The Settings View Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Settings
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// Settings View Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.DetailOrganizerModelController" />
    public class SettingsViewOrganizerModelController : DetailOrganizerModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public SettingsViewOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options = null)
            : base(viewReference, options)
        {
        }

        /// <summary>
        /// Builds the detail organizer pages.
        /// </summary>
        public void BuildDetailOrganizerPages()
        {
            UPMOrganizer detailOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("Details"));
            this.TopLevelElement = detailOrganizer;
            detailOrganizer.ExpandFound = true;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string configName = this.ViewReference.ContextValueForKey("LayoutName");
            WebConfigLayout layout = configStore.WebConfigLayoutByName(configName);
            if (layout == null)
            {
                return;
            }

            string organizerColorKey = this.ViewReference.ContextValueForKey("OrganizerColor");
            if (!string.IsNullOrEmpty(organizerColorKey))
            {
                this.Organizer.OrganizerColor = AureaColor.ColorWithString(organizerColorKey);
            }

            string headerName = this.ViewReference.ContextValueForKey("HeaderName");
            UPConfigHeader header = null;
            if (!string.IsNullOrEmpty(headerName))
            {
                header = configStore.HeaderByName(headerName);
            }

            if (header == null)
            {
                headerName = "SYSTEMINFO.Expand";
                header = configStore.HeaderByName(headerName);
            }

            if (header != null)
            {
                detailOrganizer.SubtitleText = header.Label;
                this.AddActionButtonsFromHeaderRecordIdentification(header, null);
            }
            else
            {
                detailOrganizer.SubtitleText = LocalizedString.TextSettings;
                UPMOrganizerAction action = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.edit"));
                action.SetTargetAction(this, this.SwitchToEdit);
                action.LabelText = LocalizedString.TextEdit;
                action.IconName = "Button:Edit";
                this.AddOrganizerHeaderActionItem(action);
            }

            SettingsViewPageModelController detailPageModelController = new SettingsViewPageModelController(this.ViewReference);
            Page overviewPage = detailPageModelController.Page;
            this.AddPageModelController(detailPageModelController);
            detailOrganizer.AddPage(overviewPage);
        }

        /// <summary>
        /// Switches to edit.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        protected override void SwitchToEdit(object actionDictionary)
        {
            SettingsEditViewOrganizerModelController organizerModelController = new SettingsEditViewOrganizerModelController(this.ViewReference, null);
            this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
        }
    }
}
