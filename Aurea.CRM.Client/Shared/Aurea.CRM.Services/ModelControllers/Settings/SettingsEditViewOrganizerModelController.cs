// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsEditViewOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The Settings Edit View Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Settings
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    /// <summary>
    /// Settings Edit View Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.EditOrganizerModelController" />
    /// <seealso cref="IChangeConfigurationRequestDelegate" />
    public class SettingsEditViewOrganizerModelController : EditOrganizerModelController, IChangeConfigurationRequestDelegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsEditViewOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="initOptions">The initialize options.</param>
        public SettingsEditViewOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions initOptions)
            : base(viewReference, initOptions)
        {
        }

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public override void BuildPagesFromViewReference()
        {
            UPMOrganizer detailOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("Details"));
            this.TopLevelElement = detailOrganizer;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.ConfigName = this.ViewReference.ContextValueForKey("LayoutName");
            WebConfigLayout layout = configStore.WebConfigLayoutByName(this.ConfigName);
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

            detailOrganizer.TitleText = header != null ? header.Label : LocalizedString.TextSettings;

            EditSettingsPageModelController detailPageModelController = new EditSettingsPageModelController(this.ViewReference);
            Page overviewPage = detailPageModelController.Page;
            this.AddPageModelController(detailPageModelController);
            detailOrganizer.AddPage(overviewPage);
            this.AddOrganizerActions();
            detailOrganizer.ExpandFound = true;
        }

        /// <summary>
        /// Changed ields.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, UPEditFieldContext> ChangedFields()
        {
            Dictionary<string, UPEditFieldContext> changedFields = null;
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                EditSettingsPageModelController editModelController = modelController as EditSettingsPageModelController;

                Dictionary<string, UPEditFieldContext> changedFieldsOnPage = editModelController?.ChangedFields();
                if (changedFieldsOnPage != null)
                {
                    if (changedFields == null)
                    {
                        changedFields = changedFieldsOnPage;
                    }
                    else
                    {
                        foreach (UPEditFieldContext context in changedFieldsOnPage.Values)
                        {
                            changedFields[context.Key] = context;
                        }
                    }
                }
            }

            if (changedFields == null || changedFields.Count == 0)
            {
                return null;
            }

            return changedFields;
        }

        /// <summary>
        /// Saves the specified action dictionary.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public override void Save(object actionDictionary)
        {
            this.DisableAllActionItems(true);
            Dictionary<string, UPEditFieldContext> changedFields = this.ChangedFields();
            if (changedFields == null || changedFields.Count == 0)
            {
                this.ModelControllerDelegate.PopToPreviousContentViewController();
                return;
            }

            ServiceInfo serviceInfo = ServerSession.CurrentSession.ServiceInfoForServiceName("ChangeConfiguration");
            ChangeConfigurationServerOperation request = new ChangeConfigurationServerOperation(changedFields, this);
            if (string.CompareOrdinal(serviceInfo.Version, "1.1") <= 0)
            {
                UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("savingIdentifier"));
                UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
                statusField.FieldValue = LocalizedString.Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesEditSavingChangesProgressMessage);
                stillLoadingError.StatusMessageField = statusField;
                this.Organizer.Status = stillLoadingError;
                this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, null);
                ServerSession.CurrentSession.ExecuteRequest(request);
            }
            else
            {
                request.StoreWebConfigParametersLocally();
                UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { StringIdentifier.IdentifierWithStringId("Configuration") });
                this.ModelControllerDelegate.PopToPreviousContentViewController();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has changes.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        protected override bool HasChanges => this.ChangedFields() != null;

        /// <summary>
        /// Changes the configuration request did finish with result.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="result">The result.</param>
        public void ChangeConfigurationRequestDidFinishWithResult(ChangeConfigurationServerOperation sender, object result)
        {
            UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { StringIdentifier.IdentifierWithStringId("Configuration") });
            this.ModelControllerDelegate.PopToPreviousContentViewController();
        }
    }
}
