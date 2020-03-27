// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizerManagerMessage.cs" company="Aurea Software Gmbh">
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
//   Keep track of message keys & messages sent by Organizer Manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Organizer Manager Message Keys
    /// </summary>
    public static class OrganizerManagerMessageKey
    {
        /// <summary>
        /// The will leave organizer notification key
        /// </summary>
        public const string WillLeaveOrganizer = "OrganizerManagerWillLeaveOrganizer";

        /// <summary>
        /// The did leave organizer notification key
        /// </summary>
        public const string DidLeaveOrganizer = "OrganizerManagerDidLeaveOrganizer";

        /// <summary>
        /// The will enter organizer notification key
        /// </summary>
        public const string WillEnterOrganizer = "OrganizerManagerWillEnterOrganizer";

        /// <summary>
        /// The did enter organizer notification key
        /// </summary>
        public const string DidEnterOrganizer = "OrganizerManagerDidEnterOrganizer";

        /// <summary>
        /// The will add organizer notification key
        /// </summary>
        public const string WillAddOrganizer = "OrganizerManagerWillAddOrganizer";

        /// <summary>
        /// The did add organizer notification key
        /// </summary>
        public const string DidAddOrganizer = "OrganizerManagerDidAddOrganizer";

        /// <summary>
        /// The will close organizer notification key
        /// </summary>
        public const string WillCloseOrganizer = "OrganizerManagerWillCloseOrganizer";

        /// <summary>
        /// The did close organizer notification key
        /// </summary>
        public const string DidCloseOrganizer = "OrganizerManagerDidCloseOrganizer";
    }

    /// <summary>
    /// Organizer Manager Message
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class OrganizerManagerMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the message key.
        /// </summary>
        /// <value>
        /// The message key.
        /// </value>
        public string MessageKey { get; private set; }

        /// <summary>
        /// Gets the controller identifier.
        /// </summary>
        /// <value>
        /// The controller identifier.
        /// </value>
        public int ControllerId { get; private set; }

        /// <summary>
        /// Creates the specified message key.
        /// </summary>
        /// <param name="messageKey">The message key.</param>
        /// <param name="controllerId">The controller identifier.</param>
        /// <returns>
        /// The <see cref="OrganizerManagerMessage" />.
        /// </returns>
        public static OrganizerManagerMessage Create(string messageKey, int controllerId)
        {
            return new OrganizerManagerMessage { MessageKey = messageKey, ControllerId = controllerId };
        }
    }
}
