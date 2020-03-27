// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The History Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// History Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    public class HistoryOrganizerModelController : UPOrganizerModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public HistoryOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options = null)
            : base(viewReference, options)
        {
        }

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public override void BuildPagesFromViewReference()
        {
            UPMOrganizer detailOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("History"));
            this.TopLevelElement = detailOrganizer;
            detailOrganizer.ExpandFound = true;
            string organizerColorKey = this.ViewReference.ContextValueForKey("OrganizerColor");
            if (!string.IsNullOrEmpty(organizerColorKey))
            {
                this.Organizer.OrganizerColor = AureaColor.ColorWithString(organizerColorKey);
            }

            string headerName = this.ViewReference.ContextValueForKey("HeaderName");
            IConfigurationUnitStore store = ConfigurationUnitStore.DefaultStore;
            UPConfigHeader header = store.HeaderByName(headerName);
            if (!string.IsNullOrEmpty(header.Label))
            {
                detailOrganizer.TitleText = header.Label;
            }

            this.ShouldShowTabsForSingleTab = false;
            UPHistorySearchPageModelController docPageModelController = new UPHistorySearchPageModelController(this.ViewReference);
            this.AddPageModelController(docPageModelController);
        }
    }
}
