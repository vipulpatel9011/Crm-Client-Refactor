// <copyright file="ILocationService.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.Platform
{
    using Aurea.CRM.Core.Structs;

    /// <summary>
    /// Location service interface
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Gets or sets delegate
        /// </summary>
        ILocationServiceDelegate CurrentDelegate { get; set; }

        /// <summary>
        /// Gets current location
        /// </summary>
        Location CurrentLocation { get; }

        /// <summary>
        /// Starts getting location process from low-level API and registers delegate
        /// </summary>
        /// <param name="delegate">Delegate instance</param>
        void GetCurrentLocation(ILocationServiceDelegate @delegate);

        /// <summary>
        /// Starts updating location
        /// </summary>
        void StartUpdatingLocation();

        /// <summary>
        /// Stops updating location
        /// </summary>
        void StopUpdatingLocation();
    }

    /// <summary>
    /// Location service delegate interface
    /// </summary>
    public interface ILocationServiceDelegate
    {
        /// <summary>
        /// Informs the delegate about location change
        /// </summary>
        /// <param name="location">Location info</param>
        void LocationResult(Location location);

        /// <summary>
        /// Informs the delegate about location change
        /// </summary>
        /// <param name="location">Location info</param>
        void LocationError(string error);
    }
}
