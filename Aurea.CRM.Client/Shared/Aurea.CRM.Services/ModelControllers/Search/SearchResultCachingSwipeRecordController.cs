// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResultCachingSwipeRecordController.cs" company="Aurea Software Gmbh">
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
//   The Search Result Caching Swipe Record Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Search Result Caching Swipe Record Controller
    /// </summary>
    /// <seealso cref="ISwipePageRecordController" />
    public class UPSearchResultCachingSwipeRecordController : ISwipePageRecordController
    {
        private readonly int cacheSize;
        private readonly ISwipePageDataSourceController modelController;
        private readonly List<UPSwipePageRecordItem> tableCaptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSearchResultCachingSwipeRecordController"/> class.
        /// </summary>
        /// <param name="mc">The mc.</param>
        public UPSearchResultCachingSwipeRecordController(ISwipePageDataSourceController mc)
        {
            this.cacheSize = 250;
            this.tableCaptions = new List<UPSwipePageRecordItem>();
            this.modelController = mc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSearchResultCachingSwipeRecordController"/> class.
        /// </summary>
        public UPSearchResultCachingSwipeRecordController()
        {
        }

        /// <summary>
        /// Gets idex offset
        /// </summary>
        public int CurrentIndex { get; private set; }

        /// <summary>
        /// Builds the cache starting from recordId
        /// </summary>
        /// <param name="record">record</param>
        public void BuildCache(RecordIdentifier record = null)
        {
            var currentRecordIndex = 0;

            var cachedCaptions = this.modelController.LoadTableCaptionsFromIndexToIndex(-this.cacheSize, this.cacheSize)
                .ToList();

            if (record != null && cachedCaptions.Any())
            {
                var recordId = $"{record.InfoAreaId}.{record.RecordId}";

                var currentRecord = cachedCaptions.FirstOrDefault(c => c.RecordIdentification == recordId);

                if (currentRecord != null)
                {
                    currentRecordIndex = cachedCaptions.IndexOf(currentRecord);
                }
            }

            this.tableCaptions.AddRange(cachedCaptions);
            this.CurrentIndex = currentRecordIndex;
        }

        /// <summary>
        /// Switches to detail at index offset.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="uiDelegate">The UI delegate.</param>
        public void SwitchToIndex(int index, IModelControllerUIDelegate uiDelegate)
        {
            if (index >= 0 && index < this.tableCaptions.Count)
            {
                this.CurrentIndex = index;

                var item = this.tableCaptions[this.CurrentIndex];
                var organizerModelController = this.modelController.DetailOrganizerForRecordIdentification(item.RecordIdentification, item.OnlineData);

                organizerModelController.ParentSwipePageRecordController = this;
                uiDelegate.ExchangeContentViewController(organizerModelController);
            }
        }
    }
}
