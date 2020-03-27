// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelControllerUIDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The ModelControllerUIDelegate interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Services.ModelControllers.Organizer;

    /// <summary>
    /// Multi Organizer Mode
    /// </summary>
    public enum MultiOrganizerMode
    {
        /// <summary>
        /// The new if needed
        /// </summary>
        NewIfNeeded = 1,

        /// <summary>
        /// The stay in current organizer
        /// </summary>
        StayInCurrentOrganizer = 2,

        /// <summary>
        /// The always new working organizer
        /// </summary>
        AlwaysNewWorkingOrganizer = 3
    }

    /// <summary>
    /// The ModelControllerUIDelegate interface.
    /// </summary>
    public interface IModelControllerUIDelegate
    {
        /// <summary>
        /// The pop to previous content view controller.
        /// </summary>
        void PopToPreviousContentViewController();

        /// <summary>
        /// The pop to root content view controller.
        /// </summary>
        void PopToRootContentViewController();

        /// <summary>
        /// The pop to new organizer model controller.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool PopToNewOrganizerModelController(UPOrganizerModelController modelController);

        /// <summary>
        /// The transition to content model controller.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int TransitionToContentModelController(UPOrganizerModelController modelController);

        /// <summary>
        /// Transitions to content model controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        /// <param name="stayInCurrentOrganizer">The stay in current organizer.</param>
        /// <returns></returns>
        int TransitionToContentModelController(UPOrganizerModelController modelController, MultiOrganizerMode stayInCurrentOrganizer);

        /// <summary>
        /// The exchange content view controller.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        void ExchangeContentViewController(UPOrganizerModelController modelController);

        /// <summary>
        /// The switch to page at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        void SwitchToPageAtIndex(int index);

        /// <summary>
        /// Handles the errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        void HandleErrors(List<Exception> errors);

        /// <summary>
        /// The create pdf data.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        byte[] CreatePdfData(int tag);

#if PORTING
        void SendToPrinterPrintButtonView(int tag, UIView printButton);
#endif

        /// <summary>
        /// The stop all editing.
        /// </summary>
        void StopAllEditing();

#if PORTING
        bool SendMailModal(UPMail mail, bool modal);
#endif

        /// <summary>
        /// The show modal sync progress view with possible cancel operation.
        /// </summary>
        /// <param name="possibleCancel">
        /// The possible cancel.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        void ShowModalSyncProgressViewWithPossibleCancelOperation(bool possibleCancel, int operation);
    }
}
