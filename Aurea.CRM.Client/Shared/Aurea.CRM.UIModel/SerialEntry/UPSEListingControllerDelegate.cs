// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEListingControllerDelegate.cs" company="Aurea Software Gmbh">
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
//   UPSEListingControllerDelegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;

    /// <summary>
    /// UPSEListingControllerDelegate
    /// </summary>
    public interface UPSEListingControllerDelegate
    {
        /// <summary>
        /// Listings the controller context did return owner.
        /// </summary>
        /// <param name="listing">The listing.</param>
        /// <param name="context">The context.</param>
        /// <param name="owner">The owner.</param>
        void ListingControllerContextDidReturnOwner(UPSEListingController listing, object context, UPSEListingOwner owner);

        /// <summary>
        /// Listings the controller context did return listing for owner.
        /// </summary>
        /// <param name="listing">The listing.</param>
        /// <param name="context">The context.</param>
        /// <param name="owner">The owner.</param>
        void ListingControllerContextDidReturnListingForOwner(UPSEListingController listing, object context, UPSEListingOwner owner);

        /// <summary>
        /// Listings the controller context did fail with error.
        /// </summary>
        /// <param name="listing">The listing.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        void ListingControllerContextDidFailWithError(UPSEListingController listing, object context, Exception error);
    }
}
