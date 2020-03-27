// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageBusyMessage.cs" company="Aurea Software Gmbh">
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
//   Message that will update the page busyness
//   i.e. when busy, the content page will be disabled for user nteractions
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Message that will update the page busyness
    /// i.e. when busy, the content page will be disabled for user nteractions
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class PageBusyMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the end work message.
        /// </summary>
        /// <value>
        /// The end work message.
        /// </value>
        public static PageBusyMessage EndWorkMessage => new PageBusyMessage { IsBusy = false, CanCancel = false };

        /// <summary>
        /// Gets the Start work message.
        /// </summary>
        /// <value>
        /// The Start work message.
        /// </value>
        public static PageBusyMessage StartWorkMessage => new PageBusyMessage { IsBusy = true, CanCancel = false };

        /// <summary>
        /// Gets the start work can cancel message.
        /// </summary>
        /// <value>
        /// The start work can cancel message.
        /// </value>
        public static PageBusyMessage StartWorkCanCancelMessage => new PageBusyMessage { IsBusy = true, CanCancel = true };

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool? IsBusy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance will ignore false IsBusy .
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is persistent; otherwise, <c>false</c>.
        /// </value>
        public bool? IsPersistent { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can cancel.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can cancel; otherwise, <c>false</c>.
        /// </value>
        public bool? CanCancel { get; set; }

        /// <summary>
        /// Sets the start work message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="canCancel">if set to <c>true</c> [can cancel].</param>
        /// <returns>
        /// An instance of PageBusyMessage
        /// </returns>
        public static PageBusyMessage SetStartWorkMessage(string message, bool canCancel)
        {
            return new PageBusyMessage
            {
                IsBusy = true,
                Message = message,
                CanCancel = canCancel
            };
        }
    }
}
