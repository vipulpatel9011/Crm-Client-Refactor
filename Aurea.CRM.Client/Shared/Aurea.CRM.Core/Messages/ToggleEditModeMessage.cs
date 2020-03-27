// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToggleEditModeMessage.cs" company="Aurea Software Gmbh">
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
//   Message that will notify the change of edit mode
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Message that will notify the change of edit mode
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class ToggleEditModeMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the disabl edit mode message.
        /// </summary>
        /// <value>
        /// The disabl edit mode message.
        /// </value>
        public static ToggleEditModeMessage DisablEditModeMessage => new ToggleEditModeMessage { Enabled = false };

        /// <summary>
        /// Gets the enabl edit mode message.
        /// </summary>
        /// <value>
        /// The enabl edit mode message.
        /// </value>
        public static ToggleEditModeMessage EnablEditModeMessage => new ToggleEditModeMessage { Enabled = true };

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ToggleEditModeMessage"/> is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }
    }
}
