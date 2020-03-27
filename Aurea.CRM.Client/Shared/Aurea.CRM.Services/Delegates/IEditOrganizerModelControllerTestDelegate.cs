// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEditOrganizerModelControllerTestDelegate.cs" company="Aurea Software Gmbh">
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
//   The EditOrganizer Model Controller Test Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    using System;
    using Aurea.CRM.Services.ModelControllers;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.Services.ModelControllers.Group;

    /// <summary>
    /// The EditOrganizer Model Controller Test Delegate
    /// </summary>
    public interface IEditOrganizerModelControllerTestDelegate
    {
        /// <summary>
        /// Organizers the model controller finished with page model controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        /// <param name="editPageModelController">The edit page model controller.</param>
        void OrganizerModelControllerFinishedWithPageModelController(UPMModelController modelController, EditPageModelController editPageModelController);

        /// <summary>
        /// Organizers the model controller did fail with error.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        /// <param name="error">The error.</param>
        void OrganizerModelControllerDidFailWithError(UPMModelController modelController, Exception error);

        /// <summary>
        /// Pages the model controller finished.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        void PageModelControllerFinished(UPPageModelController pageModelController);

        /// <summary>
        /// Pages the model controller did fail with error.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        /// <param name="error">The error.</param>
        void PageModelControllerDidFailWithError(UPPageModelController pageModelController, Exception error);

        /// <summary>
        /// Pages the model controller finished group controller.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        /// <param name="groupController">The group controller.</param>
        void PageModelControllerFinishedGroupController(UPPageModelController pageModelController, UPGroupModelController groupController);
    }
}
