// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineSettingsRequest.cs" company="Aurea Software Gmbh">
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
//   The Offline Settings Request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    /// <summary>
    /// Offline Settings Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineRequest" />
    public class UPOfflineSettingsRequest : UPOfflineRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineSettingsRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        public UPOfflineSettingsRequest(int requestNr)
            : base(requestNr)
        {
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public override OfflineRequestType RequestType => OfflineRequestType.Settings;

        /// <summary>
        /// Gets the default title line.
        /// </summary>
        /// <value>
        /// The default title line.
        /// </value>
        public override string DefaultTitleLine => "Settings";
    }
}
