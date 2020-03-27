// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToggleOfflineModeMessage.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Azat Jalilov
// </author>
// <summary>
//   Message that will notify the change of offline mode
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Message that will notify the change of offline mode
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class ToggleOfflineModeMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the disabl offline mode message.
        /// </summary>
        /// <value>
        /// The disable offline mode message.
        /// </value>
        public static ToggleOfflineModeMessage DisablOfflineModeMessage => new ToggleOfflineModeMessage { Enabled = false };

        /// <summary>
        /// Gets the enable offline mode message.
        /// </summary>
        /// <value>
        /// The enable offline mode message.
        /// </value>
        public static ToggleOfflineModeMessage EnablOfflineModeMessage => new ToggleOfflineModeMessage { Enabled = true };

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ToggleOfflineModeMessage"/> is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }
    }
}
