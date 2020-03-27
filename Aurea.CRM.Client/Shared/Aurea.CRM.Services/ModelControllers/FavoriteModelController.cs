// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FavoriteModelController.cs" company="Aurea Software Gmbh">
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
//   Favorite Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Services.Delegates;

    /// <summary>
    /// Favorite Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.UPMModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMFavoritesDelegate" />
    public class UPFavoriteModelController : UPMModelController, UPCRMFavoritesDelegate
    {
        /// <summary>
        /// The CRM favorites is
        /// </summary>
        private UPCRMFavorites crmFavoritesIs;

        /// <summary>
        /// The CRM favorites add
        /// </summary>
        private UPCRMFavorites crmFavoritesAdd;

        /// <summary>
        /// The CRM favorites delete
        /// </summary>
        private UPCRMFavorites crmFavoritesDelete;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFavoriteModelController"/> class.
        /// </summary>
        public UPFavoriteModelController()
        {
            this.crmFavoritesIs = new UPCRMFavorites(UPOfflineRequestMode.OnlineConfirm, this);
            this.crmFavoritesAdd = new UPCRMFavorites(UPOfflineRequestMode.OnlineConfirm, this);
            this.crmFavoritesDelete = new UPCRMFavorites(UPOfflineRequestMode.OnlineConfirm, this);
        }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IFavoriteModelControllerDelegate TheDelegate { get; set; }

        /// <summary>
        /// Changes the favorite value.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="favorite">if set to <c>true</c> [favorite].</param>
        public void ChangeFavoriteValue(string recordIdentification, bool favorite)
        {
            if (favorite)
            {
                this.crmFavoritesAdd.RequestAddAsFavorite(recordIdentification);
            }
            else
            {
                this.crmFavoritesDelete.RequestDeleteFromFavorites(recordIdentification);
            }
        }

        /// <summary>
        /// Determines whether the specified record identification is favorite.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        public void IsFavorite(string recordIdentification)
        {
            this.crmFavoritesIs.RequestIsFavorite(recordIdentification);
        }

        /// <summary>
        /// Favoriteses the did finish with result.
        /// </summary>
        /// <param name="crmFavorites">The CRM favorites.</param>
        /// <param name="data">The result.</param>
        public void FavoritesDidFinishWithResult(UPCRMFavorites crmFavorites, object data)
        {
            if (crmFavorites == this.crmFavoritesIs)
            {
                var result = (UPCRMResult)data;
                this.TheDelegate.FavoriteModelControllerDidChangeFavorite(this, result.RowCount == 1 ? result.ResultRowAtIndex(0).RecordIdentificationAtIndex(0) : null);
            }
            else if (crmFavorites == this.crmFavoritesAdd)
            {
                string favoriteRecordIdentification = null;

                var rows = data as List<UPCRMRecord>;
                if (rows != null && rows.Count >= 1)
                {
                    favoriteRecordIdentification = rows[0].RecordIdentification;
                }

                this.TheDelegate.FavoriteModelControllerDidChangeFavorite(this, favoriteRecordIdentification);
            }
            else if (crmFavorites == this.crmFavoritesDelete)
            {
                this.TheDelegate.FavoriteModelControllerDidChangeFavorite(this, null);
            }
        }

        /// <summary>
        /// Favoriteses the did fail with error.
        /// </summary>
        /// <param name="crmFavorites">The CRM favorites.</param>
        /// <param name="error">The error.</param>
        public void FavoritesDidFailWithError(UPCRMFavorites crmFavorites, Exception error)
        {
            this.TheDelegate.FavoriteModelControllerDidFailWithError(this, error);
        }
    }
}
