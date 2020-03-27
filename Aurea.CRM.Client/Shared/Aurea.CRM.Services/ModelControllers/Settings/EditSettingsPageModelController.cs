// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditSettingsPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Edit Settings Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Settings
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    /// <summary>
    /// Edit Settings Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Edit.EditPageModelController" />
    /// <seealso cref="IChangeConfigurationRequestDelegate" />
    public class EditSettingsPageModelController : EditPageModelController, IChangeConfigurationRequestDelegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditSettingsPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public EditSettingsPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.BuildPage();
        }

        /// <summary>
        /// Instantiates the page.
        /// </summary>
        /// <returns></returns>
        public override Page InstantiatePage()
        {
            if (this.Page != null)
            {
                return new EditPage(this.Page.Identifier);
            }

            string layoutName = this.ViewReference.ContextValueForKey("LayoutName");
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            WebConfigLayout layout = configStore.WebConfigLayoutByName(layoutName);
            return new EditPage(StringIdentifier.IdentifierWithStringId(layout.UnitName));
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public override void BuildPage()
        {
            string layoutName = this.ViewReference.ContextValueForKey("LayoutName");
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            WebConfigLayout layout = configStore.WebConfigLayoutByName(layoutName);
            if (layout == null)
            {
                return;
            }

            EditPage page = new EditPage(StringIdentifier.IdentifierWithStringId("Configuration"));
            page.LabelText = LocalizedString.TextTabOverview;
            page.Invalid = true;
            this.TopLevelElement = page;
            int tabCount;
            tabCount = layout.TabCount;
            for (int i = 0; i < tabCount; i++)
            {
                UPGroupModelController groupModelController = UPGroupModelController.SettingsEditGroupModelController(layout, i);
                if (groupModelController != null)
                {
                    this.GroupModelControllerArray.Add(groupModelController);
                }
            }
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="oldPage">The old page.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElementForPage(Page oldPage)
        {
            EditPage editPage = new EditPage(oldPage.Identifier);
            this.FillPageWithResultRow(editPage, null, UPRequestOption.BestAvailable);
            return editPage;
        }

        /// <summary>
        /// Changeds the fields.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, UPEditFieldContext> ChangedFields()
        {
            Dictionary<string, UPEditFieldContext> dict = null;
            foreach (UPGroupModelController groupModelController in this.GroupModelControllerArray)
            {
                SettingsEditViewGroupModelController editGroupController = groupModelController as SettingsEditViewGroupModelController;
                if (editGroupController != null)
                {
                    foreach (UPEditFieldContext fieldContext in editGroupController.EditFieldContexts)
                    {
                        if (fieldContext.WasChanged())
                        {
                            if (dict != null)
                            {
                                dict[fieldContext.Key] = fieldContext;
                            }
                            else
                            {
                                dict = new Dictionary<string, UPEditFieldContext> { { fieldContext.Key, fieldContext } };
                            }
                        }
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            Dictionary<string, UPEditFieldContext> changedFields = this.ChangedFields();
            if (changedFields == null || changedFields.Count == 0)
            {
                return;
            }

            ServiceInfo serviceInfo = ServerSession.CurrentSession.ServiceInfoForServiceName("ChangeConfiguration");
            ChangeConfigurationServerOperation request = new ChangeConfigurationServerOperation(changedFields, this);
            if (string.CompareOrdinal(serviceInfo.Version, "1.1") <= 0)
            {
                UPMProgressStatus stillLoading = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("savingIdentifier"));
                UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
                statusField.FieldValue = LocalizedString.Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesEditSavingChangesProgressMessage);
                stillLoading.StatusMessageField = statusField;
                this.Page.Status = stillLoading;
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
        /// Changes the configuration request did finish with result.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="result">The result.</param>
        public void ChangeConfigurationRequestDidFinishWithResult(ChangeConfigurationServerOperation sender, object result)
        {
            UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { StringIdentifier.IdentifierWithStringId("Configuration") });
            this.Page.Status = null;
            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, null);
        }
    }
}
