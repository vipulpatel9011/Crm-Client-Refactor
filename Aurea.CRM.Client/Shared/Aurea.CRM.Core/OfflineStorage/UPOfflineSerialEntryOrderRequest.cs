// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineSerialEntryOrderRequest.cs" company="Aurea Software Gmbh">
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
//   The Offline Serial Entry Order Request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Offline Serial Entry Order Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineSerialEntryRequest" />
    public class UPOfflineSerialEntryOrderRequest : UPOfflineSerialEntryRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineSerialEntryOrderRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        public UPOfflineSerialEntryOrderRequest(int requestNr)
            : base(requestNr)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineSerialEntryOrderRequest"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPOfflineSerialEntryOrderRequest(ViewReference viewReference)
            : base(viewReference)
        {
        }

        /// <summary>
        /// Gets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public override OfflineRequestProcess ProcessType => OfflineRequestProcess.SerialEntryOrder;

        /// <summary>
        /// Gets the default title line.
        /// </summary>
        /// <value>
        /// The default title line.
        /// </value>
        public override string DefaultTitleLine => "Order";
    }
}
