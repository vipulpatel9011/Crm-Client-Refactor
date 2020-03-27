// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSearchPage.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm table style.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.Filters.MultiSelect;

    using Core.Structs;

    /// <summary>
    /// The upm table style.
    /// </summary>
    public enum UPMTableStyle
    {
        /// <summary>
        /// The upm standard table style.
        /// </summary>
        UPMStandardTableStyle, // Just a Label on an clear background

        /// <summary>
        /// The upm global search table style.
        /// </summary>
        UPMGlobalSearchTableStyle, // GlobalSearchStyle Icon label and a colored bottom bar
    }

    /// <summary>
    /// Search types on search page
    /// </summary>
    public enum SearchPageSearchType
    {
        /// <summary>
        /// The offline search.
        /// </summary>
        OfflineSearch = 1,

        /// <summary>
        /// The online search.
        /// </summary>
        OnlineSearch = 2
    }

    /// <summary>
    /// The search page mode.
    /// </summary>
    [Flags]
    public enum SearchPageMode
    {
        /// <summary>
        /// The ignore virtual.
        /// </summary>
        IgnoreVirtual = 1,

        /// <summary>
        /// The show color on default.
        /// </summary>
        ShowColorOnDefault = 2,

        /// <summary>
        /// The show color on virtual info area.
        /// </summary>
        ShowColorOnVirtualInfoArea = 4,

        /// <summary>
        /// The ignore colors.
        /// </summary>
        IgnoreColors = 8,

        /// <summary>
        /// The force optimize for speed.
        /// </summary>
        ForceOptimizeForSpeed = 16
    }

    /// <summary>
    /// The search page view type.
    /// </summary>
    public enum SearchPageViewType
    {
        /// <summary>
        /// The list.
        /// </summary>
        List = 1,

        /// <summary>
        /// The map.
        /// </summary>
        Map = 2,

        /// <summary>
        /// The calendar first.
        /// </summary>
        CalendarFirst = 3,

        /// <summary>
        /// The calendar day.
        /// </summary>
        CalendarDay = 3,

        /// <summary>
        /// The calendar week.
        /// </summary>
        CalendarWeek = 4,

        /// <summary>
        /// The calendar month.
        /// </summary>
        CalendarMonth = 5,

        /// <summary>
        /// The calendar list.
        /// </summary>
        CalendarList = 6,

        /// <summary>
        /// The calendar time.
        /// </summary>
        CalendarTime = 7,

        /// <summary>
        /// The calendar year.
        /// </summary>
        CalendarYear = 8,

        /// <summary>
        /// The calendar last.
        /// </summary>
        CalendarLast = 8,

        /// <summary>
        /// The geo.
        /// </summary>
        Geo = 9,

        /// <summary>
        /// The calendar week.
        /// </summary>
        CalendarWorkWeek = 10,
    }

    /// <summary>
    /// States of the search page results
    /// </summary>
    public enum SearchPageResultState
    {
        /// <summary>
        /// OK, when search result has at least one row
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Only online. Locally data not available (or RequestMode = Online is configured ), and there is no server connection
        /// </summary>
        OnlyOnline = 1,

        /// <summary>
        /// The no search character. It has not been sought (and it is configured that at least one character must be entered)
        /// </summary>
        NoSearchCharacter = 2,

        /// <summary>
        /// It is an (empty ) Online Result - > Offline result is empty
        /// </summary>
        EmptyOnline = 3,

        /// <summary>
        /// Server connection available - > locally No data found
        /// </summary>
        NoLokalData = 4,

        /// <summary>
        /// No server connection available - > No data found locally
        /// </summary>
        OfflineNoLokalData = 5
    }

    /// <summary>
    /// The upm search page.
    /// </summary>
    public class UPMSearchPage : Page
    {
        private List<UPMFilter> availableFilters;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSearchPage"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMSearchPage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto switch to offline.
        /// </summary>
        public bool AutoSwitchToOffline { get; set; }

        /// <summary>
        /// Gets or sets the available filters.
        /// </summary>
        public List<UPMFilter> AvailableFilters
        {
            get => this.availableFilters;
            set
            {
                this.availableFilters = value;
                this.SelectableFilters = value?.Select(i => new SelectableItem(i)).ToList();
            }
        }

        /// <summary>
        /// Gets or sets filters that can be selected
        /// </summary>
        public List<SelectableItem> SelectableFilters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether available online search.
        /// </summary>
        public bool AvailableOnlineSearch { get; set; }

        /// <summary>
        /// Gets or sets the available view types.
        /// </summary>
        public List<SearchPageViewType> AvailableViewTypes { get; set; }

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        public TableCellStyle CellStyle { get; set; }

        /// <summary>
        /// Gets or sets the default view type.
        /// </summary>
        public SearchPageViewType DefaultViewType { get; set; }

        /// <summary>
        /// Gets or sets the current view type        
        /// </summary>
        public SearchPageViewType CurrentViewType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether hide search bar.
        /// </summary>
        public bool HideSearchBar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether hide section index.
        /// </summary>
        public bool HideSectionIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether hide text search.
        /// </summary>
        public bool HideTextSearch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether initially online.
        /// </summary>
        public bool InitiallyOnline { get; set; }

        /// <summary>
        /// Gets the number of result sections.
        /// </summary>
        public int NumberOfResultSections => this.Children.Count;

        /// <summary>
        /// Gets or sets the offline wait time.
        /// </summary>
        public double OfflineWaitTime { get; set; }

        /// <summary>
        /// Gets or sets the online wait time.
        /// </summary>
        public double OnlineWaitTime { get; set; }

        /// <summary>
        /// Gets the result sections.
        /// </summary>
        public List<UPMResultSection> ResultSections => this.Children.Cast<UPMResultSection>().ToList();

        /// <summary>
        /// Gets or sets the result state.
        /// </summary>
        public SearchPageResultState ResultState { get; set; }

        /// <summary>
        /// Gets or sets the row action.
        /// </summary>
        public UPMAction RowAction { get; set; }

        /// <summary>
        /// Gets or sets the search action.
        /// </summary>
        public UPMAction SearchAction { get; set; }

        /// <summary>
        /// Gets or sets the search geo address.
        /// </summary>
        public string SearchGeoAddress { get; set; }

        /// <summary>
        /// Gets or sets the search current location
        /// </summary>
        public Location CurrentLocation { get; set; }

        /// <summary>
        /// Gets or sets the search placeholder.
        /// </summary>
        public string SearchPlaceholder { get; set; }

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Gets or sets the search type.
        /// </summary>
        public SearchPageSearchType SearchType { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        public UPMTableStyle Style { get; set; }

        /// <summary>
        /// Gets or sets the view type.
        /// </summary>
        public SearchPageViewType ViewType { get; set; }

        /// <summary>
        /// The add result section.
        /// </summary>
        /// <param name="resultSection">
        /// The result section.
        /// </param>
        public void AddResultSection(UPMResultSection resultSection)
        {
            this.AddChild(resultSection);
        }

        /// <summary>
        /// The copy data from.
        /// </summary>
        /// <param name="otherPage">
        /// The other page.
        /// </param>
        public virtual void CopyDataFrom(UPMSearchPage otherPage)
        {
            this.SearchText = otherPage.SearchText;
            this.SearchType = otherPage.SearchType;
            this.ViewType = otherPage.ViewType;
            this.SearchPlaceholder = otherPage.SearchPlaceholder;
            this.AvailableFilters = otherPage.AvailableFilters;
            this.AvailableViewTypes = otherPage.AvailableViewTypes;

            this.LabelText = otherPage.LabelText;
            this.SearchGeoAddress = otherPage.SearchGeoAddress;

            this.CurrentLocation = otherPage.CurrentLocation;
            this.HideTextSearch = otherPage.HideTextSearch;
            this.AvailableOnlineSearch = otherPage.AvailableOnlineSearch;
            this.Style = otherPage.Style;
        }

        /// <summary>
        /// The remove all result sections.
        /// </summary>
        public void RemoveAllResultSections()
        {
            this.RemoveAllChildren();
        }

        /// <summary>
        /// The remove result section.
        /// </summary>
        /// <param name="resultSection">
        /// The result section.
        /// </param>
        public void RemoveResultSection(UPMResultSection resultSection)
        {
            this.Children.Remove(resultSection);
        }

        /// <summary>
        /// The result section at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMResultSection"/>.
        /// </returns>
        public UPMResultSection ResultSectionAtIndex(int index)
        {
            return this.Children[index] as UPMResultSection;
        }
    }
}
