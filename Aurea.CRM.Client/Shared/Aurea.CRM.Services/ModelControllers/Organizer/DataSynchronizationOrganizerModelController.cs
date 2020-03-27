// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSynchronizationOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   Data Synchronization Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Data Synchronization Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    public class UPDataSynchronizationOrganizerModelController : UPOrganizerModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPDataSynchronizationOrganizerModelController"/> class.
        /// </summary>
        public UPDataSynchronizationOrganizerModelController()
            : base(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPDataSynchronizationOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public UPDataSynchronizationOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options)
            : base(viewReference, options)
        {
        }

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public override void BuildPagesFromViewReference()
        {
            UPDataSyncPageModelController pageModelController = new UPDataSyncPageModelController();
            UPMOrganizer dataSynchronizationOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("DataSynchronization"));
            this.ShouldShowTabsForSingleTab = false;
            this.AddPageModelController(pageModelController);
            dataSynchronizationOrganizer.AddPage(pageModelController.Page);
            this.TopLevelElement = dataSynchronizationOrganizer;
        }
    }
}
