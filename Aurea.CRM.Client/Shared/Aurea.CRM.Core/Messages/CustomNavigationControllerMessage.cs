// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomNavigationControllerMessage.cs" company="Aurea Software Gmbh">
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
//   Keep track of message keys & messages sent for custom navigation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Custom Navigation Controller Message Keys
    /// </summary>
    public static class CustomNavigationControllerMessageKey
    {
        /// <summary>
        /// The did push view controller notification
        /// </summary>
        public const string DidPushViewController = "CustomNavigationControllerDidPushViewController";

        /// <summary>
        /// The did switch view controller notification
        /// </summary>
        public const string DidSwitchViewController = "CustomNavigationControllerDidSwitchViewController";

        /// <summary>
        /// The did pop view controller notificaton
        /// </summary>
        public const string DidPopViewController = "CustomNavigationControllerDidPopViewController";
    }

    /// <summary>
    /// Custom Navigation Controller Message
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class CustomNavigationControllerMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the message key.
        /// </summary>
        /// <value>
        /// The message key.
        /// </value>
        public string MessageKey { get; private set; }

        /// <summary>
        /// Creates the specified message key.
        /// </summary>
        /// <param name="messageKey">The message key.</param>
        /// <returns>
        /// The <see cref="CustomNavigationControllerMessage" />.
        /// </returns>
        public static CustomNavigationControllerMessage Create(string messageKey)
        {
            return new CustomNavigationControllerMessage { MessageKey = messageKey };
        }
    }
}
