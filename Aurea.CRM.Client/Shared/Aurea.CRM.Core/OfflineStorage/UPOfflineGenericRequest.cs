// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineGenericRequest.cs" company="Aurea Software Gmbh">
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
//   Offline Generic Request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    /// <summary>
    /// Offline Generic Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineRequest" />
    public class UPOfflineGenericRequest : UPOfflineRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineGenericRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        /// <param name="type">The type.</param>
        /// <param name="processType">Type of the process.</param>
        public UPOfflineGenericRequest(int requestNr, OfflineRequestType type, OfflineRequestProcess processType)
        {
            this.RequestType = type;
            this.ProcessType = processType;
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public override OfflineRequestType RequestType { get; }

        /// <summary>
        /// Gets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public override OfflineRequestProcess ProcessType { get; }
    }
}
