// <copyright file="IFavoriteModelControllerDelegate.cs" company="Aurea Software Gmbh">
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
    using System;
    using Aurea.CRM.Services.ModelControllers;

    /// <summary>
    /// The PFavoriteModelControllerDelegate interface.
    /// </summary>
    public interface IFavoriteModelControllerDelegate
    {
        /// <summary>
        /// The favorite model controller did change favorite.
        /// </summary>
        /// <param name="favoriteModelController">
        /// The favorite model controller.
        /// </param>
        /// <param name="favoriteRecordIdentification">
        /// The favorite record identification.
        /// </param>
        void FavoriteModelControllerDidChangeFavorite(UPFavoriteModelController favoriteModelController, string favoriteRecordIdentification);

        /// <summary>
        /// The favorite model controller favorite record identification.
        /// </summary>
        /// <param name="favoriteModelController">
        /// The favorite model controller.
        /// </param>
        /// <param name="favoriteRecordIdentification">
        /// The favorite record identification.
        /// </param>
        void FavoriteModelControllerFavoriteRecordIdentification(UPFavoriteModelController favoriteModelController, string favoriteRecordIdentification);

        /// <summary>
        /// The favorite model controller did fail with error.
        /// </summary>
        /// <param name="favoriteModelController">
        /// The favorite model controller.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void FavoriteModelControllerDidFailWithError(UPFavoriteModelController favoriteModelController, Exception error);
    }
}
