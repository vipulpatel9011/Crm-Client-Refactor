// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   Document Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Services.ModelControllers.Documents;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Document Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.DetailOrganizerModelController" />
    public class DocumentOrganizerModelController : DetailOrganizerModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public DocumentOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options = null)
            : base(viewReference, options)
        {
        }

        /// <summary>
        /// Builds the detail organizer pages.
        /// </summary>
        protected override void BuildDetailOrganizerPages()
        {
            UPMOrganizer detailOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("Documemnts"));
            detailOrganizer.ExpandFound = true;
            this.TopLevelElement = detailOrganizer;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
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

            detailOrganizer.SubtitleText = header != null ? header.Label : LocalizedString.TextProcessDocuments;

            this.ShouldShowTabsForSingleTab = false;
            DocumentPageModelController docPageModelController = new DocumentPageModelController(this.ViewReference);
            this.AddPageModelController(docPageModelController);
        }
    }
}
