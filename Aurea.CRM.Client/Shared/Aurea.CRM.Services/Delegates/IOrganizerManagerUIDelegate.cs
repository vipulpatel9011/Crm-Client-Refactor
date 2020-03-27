// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganizerManagerUIDelegate.cs" company="Aurea Software Gmbh">
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
//   Organizer Manager UI Delegate interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    using Aurea.CRM.Services.ModelControllers.Organizer;

    /// <summary>
    /// Organizer Manager UI Delegate interface
    /// </summary>
    public interface IOrganizerManagerUIDelegate
    {
        /// <summary>
        /// Adds the new nav controller for identifier.
        /// </summary>
        /// <param name="navControllerId">The nav controller identifier.</param>
        void AddNewNavControllerForId(int navControllerId);

        /// <summary>
        /// Adds the new working nav controller for identifier with model controller.
        /// </summary>
        /// <param name="navControllerId">The nav controller identifier.</param>
        /// <param name="modelController">The model controller.</param>
        void AddNewWorkingNavControllerForIdWithModelController(int navControllerId, UPOrganizerModelController modelController);

        /// <summary>
        /// Switches to nav controller from nav conroller.
        /// </summary>
        /// <param name="toIndex">To index.</param>
        /// <param name="fromIndex">From index.</param>
        void SwitchToNavControllerFromNavConroller(int toIndex, int fromIndex);

        /// <summary>
        /// Closes the nav controller.
        /// </summary>
        /// <param name="index">The index.</param>
        void CloseNavController(int index);

        /// <summary>
        /// Gets current organizer model controller
        /// </summary>
        UPOrganizerModelController CurrentOrganizerModelController { get; }

        /// <summary>
        /// Sets the focus view visible.
        /// </summary>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        void SetFocusViewVisible(bool visible);
    }
}
