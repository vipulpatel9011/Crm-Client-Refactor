// <copyright file="FilterChangedMessage.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Filter RowState Changed Event
    /// </summary>
    public class FilterChangedMessage : INotificationMessage
    {
        /// <summary>
        /// Gets or sets the Filter Row that has changed
        /// </summary>
        public object FilterRow { get; set; }

        /// <summary>
        /// Gets or sets the message type
        /// </summary>
        public FilterMessageType MessageType { get; set; }
    }
}
