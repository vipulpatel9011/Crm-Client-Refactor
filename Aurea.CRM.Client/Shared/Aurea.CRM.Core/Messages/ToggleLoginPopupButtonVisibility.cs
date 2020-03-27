// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToggleLoginPopupButtonVisibility.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Jakub Malczak
// </author>
// <summary>
//   Message toggle popup button visibility on tob bar view
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Message toggle popup button visibility on top bar view
    /// </summary>
    public class ToggleLoginPopupButtonVisibility : INotificationMessage
    {
        /// <summary>
        /// Gets or sets a value indicating whether login popup button is visible
        /// </summary>
        public bool IsVisible { get; set; }
    }
}
