// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionWatchDogMessage.cs" company="Aurea Software Gmbh">
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
//   Keep track of message keys used by Connection Watchdog
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// ConnectionWatchDogMessageKey
    /// </summary>
    public static class ConnectionWatchDogMessageKey
    {
        /// <summary>
        /// The did establish server connectivity message key
        /// </summary>
        public const string DidEstablishServerConnectivity = "DidEstablishServerConnectivity";

        /// <summary>
        /// The did loose server connectivity message key
        /// </summary>
        public const string DidLooseServerConnectivity = "DidLooseServerConnectivity";

        /// <summary>
        /// The did change connectivity quality message key
        /// </summary>
        public const string DidChangeConnectivityQuality = "DidChangeConnectivityQuality";
    }

    /// <summary>
    /// ConnectionWatchDogMessage
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class ConnectionWatchDogMessage : INotificationMessage
    {
        /// <summary>
        /// Gets or sets the message key.
        /// </summary>
        /// <value>
        /// The message key.
        /// </value>
        public string MessageKey { get; set; }
    }
}
