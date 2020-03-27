// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Settings View Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Settings
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// The Settings View Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.DetailPageModelController" />
    public class SettingsViewPageModelController : DetailPageModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public SettingsViewPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
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

            MDetailPage page = new MDetailPage(StringIdentifier.IdentifierWithStringId("Configuration"));
            page.LabelText = LocalizedString.TextTabOverview;
            page.Invalid = true;
            this.TopLevelElement = page;
            int tabCount = layout.TabCount;
            for (int i = 0; i < tabCount; i++)
            {
                UPGroupModelController groupModelController = UPGroupModelController.SettingsGroupModelController(layout, i);
                if (groupModelController != null)
                {
                    this.GroupModelControllerArray.Add(groupModelController);
                }
            }
        }

        /// <summary>
        /// Updates the element for page.
        /// </summary>
        /// <param name="oldDetailPage">The old detail page.</param>
        /// <returns>
        /// The <see cref="UPMElement" />.
        /// </returns>
        public override UPMElement UpdatedElementForPage(Page oldDetailPage)
        {
            MDetailPage detailPage = new MDetailPage(oldDetailPage.Identifier);
            this.FillPageWithResultRow(detailPage, null, UPRequestOption.Default);
            return detailPage;
        }
    }
}
