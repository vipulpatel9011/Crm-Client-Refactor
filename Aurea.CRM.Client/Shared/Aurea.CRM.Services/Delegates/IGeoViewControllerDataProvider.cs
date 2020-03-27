// <copyright file="IGeoViewControllerDataProvider.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.Delegates
{
    using Aurea.CRM.Core.Structs;

    /// <summary>
    /// The UPMLocation mode type.
    /// </summary>
    public enum UPMLocationModeType
    {
        /// <summary>
        /// Automatic.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Initial.
        /// </summary>
        Initial = 1,

        /// <summary>
        /// Manual.
        /// </summary>
        Manual = 2
    }

    /// <summary>
    /// The IGeoViewControllerDataProvider interface.
    /// </summary>
    public interface IGeoViewControllerDataProvider : ISearchViewControllerDataProvider
    {
        /// <summary>
        /// Gets or sets the location mode.
        /// </summary>
        UPMLocationModeType LocationMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether any result with image field.
        /// </summary>
        bool AnyResultWithImageField { get; set; }

        /// <summary>
        /// The set user location with manual address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        void SetUserLocationWithManualAddress(string address);

        /// <summary>
        /// The start auto location.
        /// </summary>
        void StartAutoLocation();

        /// <summary>
        /// The set user location with manual location.
        /// </summary>
        /// <param name="location">
        /// New location.
        /// </param>
        void SetUserLocationWithManualLocation(Location location);
    }
}
