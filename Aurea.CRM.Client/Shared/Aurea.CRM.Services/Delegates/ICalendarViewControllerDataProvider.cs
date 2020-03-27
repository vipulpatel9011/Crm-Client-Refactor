// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICalendarViewControllerDataProvider.cs" company="Aurea Software Gmbh">
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
//   The Calendar view controller data provider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Services.ModelControllers;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// The ICalendarViewControllerDataProvider interface.
    /// </summary>
    public interface ICalendarViewControllerDataProvider : ISearchViewControllerDataProvider
    {
        /// <summary>
        /// Gets the calendar page.
        /// </summary>
        UPMCalendarPage CalendarPage { get; }

        /// <summary>
        /// Gets the rep filter.
        /// </summary>
        UPMFilter RepFilter { get; }

        /// <summary>
        /// Gets the iPad calendar filter.
        /// </summary>
        UPMCatalogFilter IPadCalendarFilter { get; }

        /// <summary>
        /// The register add start date.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        void RegisterAddStartDate(DateTime date);

        /// <summary>
        /// The set current calendar view type.
        /// </summary>
        /// <param name="viewType">
        /// The view type.
        /// </param>
        void SetCurrentCalendarViewType(UPCalendarViewType viewType);

        /// <summary>
        /// The sender needs calendar item details.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        void SenderNeedsCalendarItemDetails(/*UPCalendarViewController*/ object sender, ICalendarItem item);

        /// <summary>
        /// The create new calendar item for date with button.
        /// </summary>
        /// <param name="date">
        /// The _date.
        /// </param>
        /// <param name="button">
        /// The button.
        /// </param>
        void CreateNewCalendarItemForDate(DateTime date, UPConfigButton button);
    }
}
