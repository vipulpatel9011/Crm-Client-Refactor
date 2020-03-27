// <copyright file="Location.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.Structs
{
    using System;

    /// <summary>
    /// Location data
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        /// <param name="lng">Longitude</param>
        /// <param name="lat">Latitude</param>
        public Location(double lng, double lat)
        {
            this.Longitude = lng;
            this.Latitude = lat;
        }

        /// <summary>
        /// Gets longitude
        /// </summary>
        public double Longitude { get; private set; }

        /// <summary>
        /// Gets latitude
        /// </summary>
        public double Latitude { get; private set; }

        /// <summary>
        /// Calculates distance between two geolocations
        /// </summary>
        /// <param name="targetLoc">Target location</param>
        /// <returns>Distance value in meters</returns>
        public double DistanceFromLocation(Location targetLoc)
        {
            var baseRad = Math.PI * this.Latitude / 180;
            var targetRad = Math.PI * targetLoc.Latitude / 180;
            var theta = this.Longitude - targetLoc.Longitude;
            var thetaRad = Math.PI * theta / 180;

            double dist = (Math.Sin(baseRad) * Math.Sin(targetRad)) + (Math.Cos(baseRad) * Math.Cos(targetRad) * Math.Cos(thetaRad));
            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist * 1609.344;
        }
    }
}
