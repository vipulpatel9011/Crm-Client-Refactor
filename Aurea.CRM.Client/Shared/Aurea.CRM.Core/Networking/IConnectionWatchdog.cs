// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionWatchdog.cs" company="Aurea Software Gmbh">
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
//   Defines the different reachability statuses
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Networking
{
    using System;

    /// <summary>
    /// Defines the different reachability statuses
    /// </summary>
    public enum ReachabilityStatus
    {
        /// <summary>
        /// The not reachable.
        /// </summary>
        NotReachable,

        /// <summary>
        /// The reachable via wwan.
        /// </summary>
        ReachableViaWWAN,

        /// <summary>
        /// The reachable via wi fi.
        /// </summary>
        ReachableViaWiFi
    }

    /// <summary>
    /// Interface for a connectivity watch dog
    /// </summary>
    public interface IConnectionWatchdog
    {
        /// <summary>
        /// Gets a value indicating whether this instance has internet connection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has internet connection; otherwise, <c>false</c>.
        /// </value>
        bool HasInternetConnection { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has server connection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has server connection; otherwise, <c>false</c>.
        /// </value>
        bool HasServerConnection { get; }

        /// <summary>
        /// Gets or sets the last server reachability status.
        /// </summary>
        /// <value>
        /// The last server reachability status.
        /// </value>
        ReachabilityStatus LastServerReachabilityStatus { get; set; }

        /// <summary>
        /// Connections the watch dog for server URL.
        /// Need to make this static in the implemnetation class
        /// </summary>
        /// <param name="originalServerUrl">
        /// The original server URL.
        /// </param>
        /// <returns>
        /// The <see cref="IConnectionWatchdog"/>.
        /// </returns>
        IConnectionWatchdog ConnectionWatchDogForServerURL(Uri originalServerUrl);
    }
}
