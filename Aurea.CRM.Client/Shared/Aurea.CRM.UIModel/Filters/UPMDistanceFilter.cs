// <copyright file="UPMDistanceFilter.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.UIModel.Filters
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Structs;
    using GalaSoft.MvvmLight.Ioc;


    /// <summary>
    /// Distance Filter implementation
    /// </summary>
    public class UPMDistanceFilter : UPMFilter, ILocationServiceDelegate
    {
        private ILocationService locationManager;
        private Region region;
        private int radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDistanceFilter"/> class.
        /// </summary>
        /// <param name="identifier">Identifier</param>
        public UPMDistanceFilter(IIdentifier identifier)
            : base(identifier, UPMFilterType.Distance)
        {
            this.Radius = 0;
            this.locationManager = SimpleIoc.Default.GetInstance<ILocationService>();
            this.locationManager.GetCurrentLocation(this);
            this.Invalid = true;
        }

        /// <summary>
        /// Gets or sets the parameter name
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the radius value
        /// </summary>
        public int Radius
        {
            get
            {
                return this.radius;
            }

            set
            {
                this.radius = value;
                if (!this.Invalid)
                {
                    this.region = new Region(this.CurrentUserLocation, this.radius, this.radius);
                }
            }
        }

        /// <summary>
        /// Gets or sets current user location
        /// </summary>
        public Location CurrentUserLocation { get; set; }

        /// <summary>
        /// Gets or sets image name
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// Gets or sets the color key
        /// </summary>
        public string ColorKey { get; set; }

        /// <summary>
        /// Gets Min Longitude value
        /// </summary>
        public double GetGPSXMin => this.Invalid ? 0 : this.region.Center.Longitude - this.region.LongitudeDelta;

        /// <summary>
        /// Gets Max Longitude value
        /// </summary>
        public double GetGPSXMax => this.Invalid ? 0 : this.region.Center.Longitude + this.region.LongitudeDelta;

        /// <summary>
        /// Gets Min Latitude value
        /// </summary>
        public double GetGPSYMin => this.Invalid ? 0 : this.region.Center.Latitude - this.region.LatitudeDelta;

        /// <summary>
        /// Gets Max Latitude value
        /// </summary>
        public double GetGPSYMax => this.Invalid ? 0 : this.region.Center.Latitude + this.region.LatitudeDelta;

        /// <summary>
        /// Gets radius display value
        /// </summary>
        public string RadiusDisplayValue => this.radius > 5000
            ? LocalizedString.TextDistanceFilterKmValue.Replace("%@", (this.radius / 1000.0).ToString("0.##"))
            : LocalizedString.TextDistanceFilterMValue.Replace("%@", this.radius.ToString("0.##"));

        /// <inheritdoc/>
        public void LocationResult(Location location)
        {
            this.CurrentUserLocation = location;
            this.region = new Region(this.CurrentUserLocation, this.radius, this.radius);
            this.Invalid = false;
        }

        /// <inheritdoc/>
        public void LocationError(string error)
        {
            this.Invalid = true;
        }
    }
}
