// <copyright file="Region.cs" company="Aurea Software Gmbh">
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
    /// Region data
    /// </summary>
    public class Region
    {
        // Semi-axes of WGS-84 geoidal reference
        private const double WGS84a = 6378137.0; // Major semiaxis [m]
        private const double WGS84b = 6356752.3; // Minor semiaxis [m]

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="horRadius">Horizontal Radius in meters</param>
        /// <param name="verRadius">Vertical Radius in meters</param>
        public Region(Location loc, double horRadius, double verRadius)
        {
            this.Center = loc;
            this.SetRegionWithDistance(horRadius, verRadius, loc.Latitude, loc.Longitude);
        }

        /// <summary>
        /// Gets center location
        /// </summary>
        public Location Center { get; private set; }

        /// <summary>
        /// Gets longitude delta
        /// </summary>
        public double LongitudeDelta { get; private set; }

        /// <summary>
        /// Gets latitude delta
        /// </summary>
        public double LatitudeDelta { get; private set; }

        // Taken from: http://stackoverflow.com/questions/3269202/latitude-and-longitude-bounding-box-for-c
        // Earth radius at a given latitude, according to the WGS-84 ellipsoid [m]
        private static double WGS84EarthRadius(double lat)
        {
            // http://en.wikipedia.org/wiki/Earth_radius
            var an = WGS84a * WGS84a * Math.Cos(lat);
            var bn = WGS84b * WGS84b * Math.Sin(lat);
            var ad = WGS84a * Math.Cos(lat);
            var bd = WGS84b * Math.Sin(lat);
            return Math.Sqrt(((an * an) + (bn * bn)) / ((ad * ad) + (bd * bd)));
        }

        private static double Deg2Rad(double degrees)
        {
            return Math.PI * degrees / 180.0;
        }

        private static double Rad2Deg(double radians)
        {
            return 180.0 * radians / Math.PI;
        }

        /// <summary>
        /// Sets region with distance
        /// </summary>
        /// <param name="horRadius">Horizontal radius</param>
        /// <param name="verRadius">Vertical radius</param>
        /// <param name="latitude">Latitude value</param>
        /// <param name="longitude">Longitude value</param>
        private void SetRegionWithDistance(double horRadius, double verRadius, double latitude, double longitude)
        {
            var lat = Deg2Rad(latitude);
            var lon = Deg2Rad(longitude);
            var radius = WGS84EarthRadius(lat);
            var pradius = radius * Math.Cos(lat);

            this.LatitudeDelta = Rad2Deg(horRadius / radius);
            this.LongitudeDelta = Rad2Deg(verRadius / pradius);
        }
    }
}
