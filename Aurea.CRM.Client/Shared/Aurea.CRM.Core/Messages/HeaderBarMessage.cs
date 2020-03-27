// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderBarMessage.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Serdar Tepeyurt
// </author>
// <summary>
//   Message that will update the header bar on detail pages
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Messages
{
    using System;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;

    /// <summary>
    /// Message that will update the header bar on detail pages
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Messages.INotificationMessage" />
    public class HeaderBarMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the refresh custom application bar message.
        /// </summary>
        /// <value>
        /// The refresh custom application bar message.
        /// </value>
        public static HeaderBarMessage HeaderBarVisible => new HeaderBarMessage { IsVisible = true };

        /// <summary>
        /// Gets the refresh custom application bar message.
        /// </summary>
        /// <value>
        /// The refresh custom application bar message.
        /// </value>
        public static HeaderBarMessage HeaderBarCollapsed => new HeaderBarMessage { IsVisible = false };

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsTitleUpdate { get; set; }

        /// <summary>
        /// Gets or sets organizer instance
        /// </summary>
        public HeaderData Data { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="HeaderBarMessage"/> by given <see cref="HeaderData"/> and returns
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns><see cref="HeaderBarMessage"/></returns>
        public static HeaderBarMessage SetHeaderBarDetails(HeaderData data)
        {
            return new HeaderBarMessage { IsVisible = true, IsTitleUpdate = false, Data = data };
        }

        /// <summary>
        /// Creates an instance of <see cref="HeaderBarMessage"/> by given <see cref="HeaderData"/> and returns
        /// </summary>
        /// <param name="title">Header bar title</param>
        /// <returns><see cref="HeaderBarMessage"/></returns>
        public static HeaderBarMessage HeaderBarTitleUpdate(string title)
        {
            return new HeaderBarMessage
            {
                IsTitleUpdate = true,
                IsVisible = true,
                Data = new HeaderData { Title = title }
            };
        }
    }
}
