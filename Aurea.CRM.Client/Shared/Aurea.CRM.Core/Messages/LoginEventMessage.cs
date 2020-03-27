// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginEventMessage.cs" company="Aurea Software Gmbh">
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
//   A message used to notify login status
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// A message used to notify login status
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class LoginEventMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the login message.
        /// </summary>
        /// <value>
        /// The login message.
        /// </value>
        public static LoginEventMessage LoginMessage => new LoginEventMessage { IsLogedIn = true };

        /// <summary>
        /// Gets the logout and terminate message.
        /// </summary>
        /// <value>
        /// The logout and terminate message.
        /// </value>
        public static LoginEventMessage LogoutAndTerminateMessage
            => new LoginEventMessage { IsLogedIn = false, TerminateApp = true };

        /// <summary>
        /// Gets the logout message.
        /// </summary>
        /// <value>
        /// The logout message.
        /// </value>
        public static LoginEventMessage LogoutMessage => new LoginEventMessage { IsLogedIn = false };

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loged in.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loged in; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogedIn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [terminate application].
        /// </summary>
        /// <value>
        /// <c>true</c> if [terminate application]; otherwise, <c>false</c>.
        /// </value>
        public bool TerminateApp { get; set; }
    }
}
