// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InboxFileManagerMessage.cs" company="Aurea Software Gmbh">
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
//   The InBox File Manager Message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Inbox FIle Manager Message
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class InboxFileManagerMessage : INotificationMessage
    {
        /// <summary>
        /// The inbox file manager - file added to inbox message
        /// </summary>
        public const string FileAddedToInboxMessageKey = "InboxFileManagerFileAddedToInbox";

        /// <summary>
        /// The inbox file manager file removed from inbox message
        /// </summary>
        public const string FileRemovedFromInboxMessageKey = "InboxFileManagerFileRemovedFromInbox";

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; private set; }

        /// <summary>
        /// Files the added to inbox.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static InboxFileManagerMessage FileAddedToInbox(string url)
        {
            return new InboxFileManagerMessage { Message = FileAddedToInboxMessageKey, Url = url };
        }

        /// <summary>
        /// Files the removed from inbox.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static InboxFileManagerMessage FileRemovedFromInbox(string url)
        {
            return new InboxFileManagerMessage { Message = FileRemovedFromInboxMessageKey, Url = url };
        }
    }
}
