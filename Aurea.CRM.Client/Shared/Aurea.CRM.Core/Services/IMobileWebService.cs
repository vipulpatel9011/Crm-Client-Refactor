// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMobileWebService.cs" company="Aurea Software Gmbh">
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
//   Defines the access methods to the Update Mobile Web Services
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Defines the access methods to the Update Mobile Web Services
    /// </summary>
    public interface IMobileWebService
    {
        /// <summary>
        /// The load remote data.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task LoadRemoteData(RemoteData remoteData, CancellationToken token);
    }
}
