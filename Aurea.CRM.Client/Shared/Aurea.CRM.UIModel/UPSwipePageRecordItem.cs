// <copyright file="UPSwipePageRecordItem.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel
{
    /// <summary>
    /// Swipe Page Record Item
    /// </summary>
    public class UPSwipePageRecordItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSwipePageRecordItem"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="subtitle">The subtitle.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="onlineData">if set to <c>true</c> [online data].</param>
        public UPSwipePageRecordItem(string title, string subtitle, string recordIdentification, bool onlineData)
        {
            this.Title = title;
            this.Subtitle = subtitle;
            this.OnlineData = onlineData;
            this.RecordIdentification = recordIdentification;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; }

        /// <summary>
        /// Gets the subtitle.
        /// </summary>
        /// <value>
        /// The subtitle.
        /// </value>
        public string Subtitle { get; }

        /// <summary>
        /// Gets a value indicating whether [online data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [online data]; otherwise, <c>false</c>.
        /// </value>
        public bool OnlineData { get; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; }
    }
}
