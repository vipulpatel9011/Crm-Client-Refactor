// <copyright file="UPCRMFavoritesDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.CRM.Delegates
{
    using System;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// The PCRMFavoritesDelegate interface.
    /// </summary>
    public interface UPCRMFavoritesDelegate
    {
        /// <summary>
        /// The favorites did finish with result.
        /// </summary>
        /// <param name="favorites">
        /// The favorites.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        void FavoritesDidFinishWithResult(UPCRMFavorites favorites, object result);

        /// <summary>
        /// The favorites did fail with error.
        /// </summary>
        /// <param name="favorites">
        /// The favorites.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void FavoritesDidFailWithError(UPCRMFavorites favorites, Exception error);
    }
}
