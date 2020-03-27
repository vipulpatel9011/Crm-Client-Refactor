// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGroupModelControllerDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   GRoup model controller delegate interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.Services.ModelControllers.Organizer;

    /// <summary>
    /// GRoup model controller delegate interface
    /// </summary>
    public interface IGroupModelControllerDelegate
    {
        /// <summary>
        /// The exchange content view controller.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        void ExchangeContentViewController(UPOrganizerModelController modelController);

        /// <summary>
        /// Forces the redraw.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        void ForceRedraw(object sender);

        /// <summary>
        /// Groups the model controller finished.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        void GroupModelControllerFinished(UPGroupModelController sender);

        /// <summary>
        /// Groups the model controller value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        void GroupModelControllerValueChanged(object sender, object value);

        /// <summary>
        /// Groups the model controller value for key.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GroupModelControllerValueForKey(object sender, string key);

        /// <summary>
        /// Performs the organizer action.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        void PerformOrganizerAction(object sender, ViewReference viewReference);

        /// <summary>
        /// Performs the organizer action.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        /// <param name="onlineData">
        /// if set to <c>true</c> [online data].
        /// </param>
        void PerformOrganizerAction(object sender, ViewReference viewReference, bool onlineData);

        /// <summary>
        /// The transition to content model controller.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        void TransitionToContentModelController(UPOrganizerModelController modelController);
    }
}
