// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToastrMessage.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Aniket Ramekar
// </author>
// <summary>
//   Message that represents a Toastr message
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Message showing Toastr message
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class ToastrMessage : INotificationMessage
    {
        /// <summary>
        /// Gets or sets the message to show
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Gets or sets the detailed version of the message
        /// </summary>
        public string DetailedMessage { get; set; }
    }
}
