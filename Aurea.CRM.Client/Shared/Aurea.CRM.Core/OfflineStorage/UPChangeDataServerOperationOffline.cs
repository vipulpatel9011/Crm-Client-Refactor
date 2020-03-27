// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPChangeDataServerOperationOffline.cs" company="Aurea Software Gmbh">
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
//   Change Data Server Operation Offline
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.OperationHandling.Data;

    /// <summary>
    /// Offline data change server operation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.Data.UPChangeDataServerOperation" />
    public class UPChangeDataServerOperationOffline : UPChangeDataServerOperation
    {
        /// <summary>
        /// Gets or sets the offline delegate.
        /// </summary>
        /// <value>
        /// The offline delegate.
        /// </value>
        public UPOfflineRequestDelegate OfflineDelegate { get; set; }

        /// <summary>
        /// Gets or sets the request mode.
        /// </summary>
        /// <value>
        /// The request mode.
        /// </value>
        public UPOfflineRequestMode RequestMode { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public object Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPChangeDataServerOperationOffline"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <param name="requestControlKey">The request control key.</param>
        /// <param name="requestNumber">The request number.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPChangeDataServerOperationOffline(List<UPCRMRecord> records, string requestControlKey, int requestNumber, IChangeDataRequestHandler theDelegate)
            : base(records, requestControlKey, requestNumber, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPChangeDataServerOperationOffline"/> class.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="requestControlKey">The request control key.</param>
        /// <param name="requestNumber">The request number.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPChangeDataServerOperationOffline(UPCRMRecord record, string requestControlKey, int requestNumber, IChangeDataRequestHandler theDelegate)
            : base(record, requestControlKey, requestNumber, theDelegate)
        {
        }
    }
}
