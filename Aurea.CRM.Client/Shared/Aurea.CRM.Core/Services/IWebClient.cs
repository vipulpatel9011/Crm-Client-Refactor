// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebClient.cs" company="Aurea Software Gmbh">
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
//   Interfce for a web client
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Services
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interfce for a web client
    /// </summary>
    public interface IWebClient
    {
        /// <summary>
        /// Fetches the web response.
        /// </summary>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<string> FetchWebResponse(Uri url);
    }
}
