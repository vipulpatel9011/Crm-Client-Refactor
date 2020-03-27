// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmFavorites.cs" company="Aurea Software Gmbh">
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
//   The Favorites class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Delegates;
    using OfflineStorage;
    using OperationHandling;
    using Query;

    /// <summary>
    /// The Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The favorites infoarea Id
        /// </summary>
        public const string FAVORITES_INFOAREAID = "FV";

        /// <summary>
        /// The favorites linkid
        /// </summary>
        public const int FAVORITES_LINKID = -1;

        /// <summary>
        /// The favorites filter name
        /// </summary>
        public const string FAVORITES_FILTERNAME = "FV.currentRep";

        /// <summary>
        /// The favorites template filter name
        /// </summary>
        public const string FAVORITES_TEMPLATEFILTERNAME = "FV.new";
    }

    /// <summary>
    /// The Favorites class implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPCRMFavorites : UPOfflineRequestDelegate, ISearchOperationHandler
    {
        /// <summary>
        /// The current query
        /// </summary>
        private UPContainerMetaInfo currentQuery;

        /// <summary>
        /// The current request
        /// </summary>
        private UPOfflineRecordRequest currentRequest;

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPCRMFavoritesDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets the offline request mode.
        /// </summary>
        /// <value>
        /// The offline request mode.
        /// </value>
        public UPOfflineRequestMode OfflineRequestMode { get; private set; }

        /// <summary>
        /// Gets the rep filter.
        /// </summary>
        /// <value>
        /// The rep filter.
        /// </value>
        public UPConfigFilter RepFilter { get; private set; }

        /// <summary>
        /// Gets the template filter.
        /// </summary>
        /// <value>
        /// The template filter.
        /// </value>
        public UPConfigFilter TemplateFilter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFavorites"/> class.
        /// </summary>
        /// <param name="offlineRequestMode">The offline request mode.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPCRMFavorites(UPOfflineRequestMode offlineRequestMode, UPCRMFavoritesDelegate theDelegate)
        {
            this.OfflineRequestMode = offlineRequestMode;
            this.TheDelegate = theDelegate;
            this.RepFilter = ConfigurationUnitStore.DefaultStore.FilterByName(Constants.FAVORITES_FILTERNAME);
            this.RepFilter = this.RepFilter?.FilterByApplyingReplacements(UPConditionValueReplacement.DefaultParameters);
            this.TemplateFilter = ConfigurationUnitStore.DefaultStore.FilterByName(Constants.FAVORITES_TEMPLATEFILTERNAME);
            this.TemplateFilter = this.TemplateFilter?.FilterByApplyingReplacements(UPConditionValueReplacement.DefaultParameters);
        }

        /// <summary>
        /// Check if favorite.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns>True if favorite</returns>
        public bool RequestIsFavorite(string recordIdentification)
        {
            this.currentQuery = new UPContainerMetaInfo(new List<UPCRMField>(), Constants.FAVORITES_INFOAREAID);
            this.currentQuery.ApplyFilter(this.RepFilter);
            this.currentQuery.SetLinkRecordIdentification(recordIdentification, Constants.FAVORITES_LINKID);
            return this.currentQuery.Find(UPRequestOption.BestAvailable, this) != null;
        }

        /// <summary>
        /// Requests add as favorite.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public bool RequestAddAsFavorite(string recordIdentification)
        {
            this.currentRequest = new UPOfflineRecordRequest();
            UPCRMRecord record = UPCRMRecord.CreateNew(Constants.FAVORITES_INFOAREAID);
            if (this.TemplateFilter != null)
            {
                record.ApplyValuesFromTemplateFilter(this.TemplateFilter);
            }

            record.AddLink(new UPCRMLink(recordIdentification, Constants.FAVORITES_LINKID));
            return this.currentRequest.StartRequest(this.OfflineRequestMode, new List<UPCRMRecord> { record }, this);
        }

        /// <summary>
        /// Requests delete from favorites.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public bool RequestDeleteFromFavorites(string recordIdentification)
        {
            this.currentRequest = new UPOfflineRecordRequest();
            UPCRMRecord record = new UPCRMRecord(recordIdentification, "Delete", null);
            return this.currentRequest.StartRequest(this.OfflineRequestMode, new List<UPCRMRecord> { record }, this);
        }

        /// <summary>
        /// Offline request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, Dictionary<string, object> result)
        {
            this.currentRequest = null;
            this.TheDelegate?.FavoritesDidFinishWithResult(this, data);
        }

        /// <summary>
        /// Offline request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            this.currentRequest = null;
            this.TheDelegate?.FavoritesDidFailWithError(this, error);
        }

        /// <summary>
        /// Offline request did finish multi request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void OfflineRequestDidFinishMultiRequest(UPOfflineRequest request)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.currentQuery = null;
            this.TheDelegate?.FavoritesDidFailWithError(this, error);
        }

        /// <summary>
        /// Search operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.currentQuery = null;
            this.TheDelegate?.FavoritesDidFinishWithResult(this, result);
        }

        /// <summary>
        /// Search operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
