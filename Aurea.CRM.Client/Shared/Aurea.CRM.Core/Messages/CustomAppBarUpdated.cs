// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomAppBarUpdated.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Message that will update the custom app bar buttons
//   This message will only notify the relevant subscribers on the
//   update done to the custom App bar buttons; In order to update the
//   buttons, first create the CustomAppCommandsViewModel instance and register with
//   the Dependency container (i.e. ServiceLocatorBuilder.RegisterCustomCommandBar(.))
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Message that will update the custom app bar buttons
    /// This message will only notify the relevant subscribers on the
    /// update done to the custom App bar buttons; In order to update the
    /// buttons, first create the CustomAppCommandsViewModel instance and register with
    /// the Dependency container (i.e. ServiceLocatorBuilder.RegisterCustomCommandBar(.))
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class CustomAppBarMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the refresh custom application bar message.
        /// </summary>
        /// <value>
        /// The refresh custom application bar message.
        /// </value>
        public static CustomAppBarMessage RefreshCustomAppBar => new CustomAppBarMessage { IsVisible = true };

        /// <summary>
        /// Gets the remove custom application bar message.
        /// </summary>
        /// <value>
        /// The remove custom application bar message.
        /// </value>
        public static CustomAppBarMessage RemoveCustomAppBar => new CustomAppBarMessage { IsVisible = false };

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible { get; set; }
    }
}
