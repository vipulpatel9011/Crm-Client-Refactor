// <copyright file="SettingsPopupMessage.cs" company="Aurea Software Gmbh">
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
//   Message for openning/closing settings popup
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Message for openning/closing settings popup
    /// </summary>
    public class SettingsPopupMessage : INotificationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPopupMessage"/> class.
        /// </summary>
        /// <param name="isOpen">if a popup should be shown</param>
        public SettingsPopupMessage(bool isOpen)
        {
            this.IsOpen = isOpen;
        }

        /// <summary>
        /// Gets message for oppening settings popup
        /// </summary>
        public static SettingsPopupMessage SettingsOpenPopupMessage => new SettingsPopupMessage(true);

        /// <summary>
        /// Gets message for closing settings popup
        /// </summary>
        public static SettingsPopupMessage SettingsClosePopupMessage => new SettingsPopupMessage(false);

        /// <summary>
        /// Gets a value indicating whether popup should be opened
        /// </summary>
        public bool IsOpen { get; private set; }
    }
}
