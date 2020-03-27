// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOrganizerModelControllerTestDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The POrganizerModelControllerTestDelegate interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Services.Delegates
{
    using System;

    /// <summary>
    /// The POrganizerModelControllerTestDelegate interface.
    /// </summary>
    public interface UPOrganizerModelControllerTestDelegate
    {
        /// <summary>
        /// The organizer model controller did finish action.
        /// </summary>
        /// <param name="organizerModelController">
        /// The organizer model controller.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        void OrganizerModelControllerDidFinishAction(object organizerModelController, object action);

        /// <summary>
        /// The organizer model controller did finish with error.
        /// </summary>
        /// <param name="organizerModelController">
        /// The organizer model controller.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void OrganizerModelControllerDidFinishWithError(object organizerModelController, Exception error);
    }
}
