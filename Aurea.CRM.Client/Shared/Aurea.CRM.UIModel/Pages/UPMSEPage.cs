// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMWebContentGroup.cs" company="Aurea Software Gmbh">
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
//   The Web Content Group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Pages
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.SerialEntry;

    /// <summary>
    /// UPMSEPage
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Pages.Page" />
    public class UPMSEPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSEPage"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSEPage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether [mini details enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mini details enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool MiniDetailsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [list quantity change enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [list quantity change enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool ListQuantityChangeEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show shopping cart].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show shopping cart]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowShoppingCart { get; set; }

        /// <summary>
        /// Gets or sets the number of displayed rows.
        /// </summary>
        /// <value>
        /// The number of displayed rows.
        /// </value>
        public int NumberOfDisplayedRows { get; set; }

        /// <summary>
        /// Gets or sets the product catalogs.
        /// </summary>
        /// <value>
        /// The product catalogs.
        /// </value>
        public List<UPMDocument> ProductCatalogs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [product catalog fullscreen].
        /// </summary>
        /// <value>
        /// <c>true</c> if [product catalog fullscreen]; otherwise, <c>false</c>.
        /// </value>
        public bool ProductCatalogFullscreen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [conflict handling].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [conflict handling]; otherwise, <c>false</c>.
        /// </value>
        public bool ConflictHandling { get; set; }

        /// <summary>
        /// Gets or sets the parent information panel.
        /// </summary>
        /// <value>
        /// The parent information panel.
        /// </value>
        public UPMSEParentInfoPanel ParentInfoPanel { get; set; }

        /// <summary>
        /// Gets or sets the summary group.
        /// </summary>
        /// <value>
        /// The summary group.
        /// </value>
        public UPMSESummaryGroup SummaryGroup { get; set; }

        /// <summary>
        /// Gets or sets the cancel action.
        /// </summary>
        /// <value>
        /// The cancel action.
        /// </value>
        public UPMAction CancelAction { get; set; }

        /// <summary>
        /// Gets or sets the close action.
        /// </summary>
        /// <value>
        /// The close action.
        /// </value>
        public UPMAction CloseAction { get; set; }

        /// <summary>
        /// Gets or sets the finish action.
        /// </summary>
        /// <value>
        /// The finish action.
        /// </value>
        public UPMAction FinishAction { get; set; }

        /// <summary>
        /// Gets or sets the summary title.
        /// </summary>
        /// <value>
        /// The summary title.
        /// </value>
        public string SummaryTitle { get; set; }

        /// <summary>
        /// Gets or sets all items button title.
        /// </summary>
        /// <value>
        /// All items button title.
        /// </value>
        public string AllItemsButtonTitle { get; set; }

        /// <summary>
        /// Gets or sets the summary button title.
        /// </summary>
        /// <value>
        /// The summary button title.
        /// </value>
        public string SummaryButtonTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disable duplicate row].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable duplicate row]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableDuplicateRow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disable delete row].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable delete row]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableDeleteRow { get; set; }

        /// <summary>
        /// Gets or sets the selected filter.
        /// </summary>
        /// <value>
        /// The selected filter.
        /// </value>
        public UPMFilter SelectedFilter { get; set; }

        /// <summary>
        /// Gets or sets the available filters.
        /// </summary>
        /// <value>
        /// The available filters.
        /// </value>
        public List<UPMFilter> AvailableFilters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show sum line].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show sum line]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSumLine { get; set; }

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        /// <value>
        /// The search text.
        /// </value>
        public string SearchText { get; set; }

        /// <summary>
        /// Gets or sets the search action.
        /// </summary>
        /// <value>
        /// The search action.
        /// </value>
        public UPMAction SearchAction { get; set; }

        /// <summary>
        /// Gets or sets the search placeholder.
        /// </summary>
        /// <value>
        /// The search placeholder.
        /// </value>
        public string SearchPlaceholder { get; set; }

        /// <summary>
        /// Gets or sets the additional search filters.
        /// </summary>
        /// <value>
        /// The additional search filters.
        /// </value>
        public List<UPMFilter> AdditionalSearchFilters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hide section index].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hide section index]; otherwise, <c>false</c>.
        /// </value>
        public bool HideSectionIndex { get; set; }

        /// <summary>
        /// Gets or sets the available position filters.
        /// </summary>
        /// <value>
        /// The available position filters.
        /// </value>
        public List<UPMFilter> AvailablePositionFilters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [scan mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [scan mode]; otherwise, <c>false</c>.
        /// </value>
        public bool ScanMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [scan add quantity].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [scan add quantity]; otherwise, <c>false</c>.
        /// </value>
        public bool ScanAddQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [list image view available].
        /// </summary>
        /// <value>
        /// <c>true</c> if [list image view available]; otherwise, <c>false</c>.
        /// </value>
        public bool ListImageViewAvailable { get; set; }

        /// <summary>
        /// Gets or sets the scanner search action.
        /// </summary>
        /// <value>
        /// The scanner search action.
        /// </value>
        public UPMAction ScannerSearchAction { get; set; }

        /// <summary>
        /// Gets or sets the close button text.
        /// </summary>
        /// <value>
        /// The close button text.
        /// </value>
        public string CloseButtonText { get; set; }

        /// <summary>
        /// Gets or sets the offline wait time.
        /// </summary>
        public double OfflineWaitTime { get; set; }

        /// <summary>
        /// Gets or sets the online wait time.
        /// </summary>
        public double OnlineWaitTime { get; set; }

        /// <summary>
        /// Removes all filters.
        /// </summary>
        public void RemoveAllFilters()
        {
            this.AvailableFilters?.Clear();
        }

        /// <summary>
        /// Removes all position filters.
        /// </summary>
        public void RemoveAllPositionFilters()
        {
            this.AvailablePositionFilters?.Clear();
        }

        /// <summary>
        /// Adds the available filter.
        /// </summary>
        /// <param name="availableFilter">The available filter.</param>
        public void AddAvailableFilter(UPMFilter availableFilter)
        {
            if (this.AvailableFilters == null)
            {
                this.AvailableFilters = new List<UPMFilter>();
            }

            this.AvailableFilters.Add(availableFilter);
        }

        /// <summary>
        /// Adds the available position filter.
        /// </summary>
        /// <param name="availablePositionFilter">The available position filter.</param>
        public void AddAvailablePositionFilter(UPMFilter availablePositionFilter)
        {
            if (this.AvailablePositionFilters == null)
            {
                this.AvailablePositionFilters = new List<UPMFilter>();
            }

            this.AvailablePositionFilters.Add(availablePositionFilter);
        }
    }
}
