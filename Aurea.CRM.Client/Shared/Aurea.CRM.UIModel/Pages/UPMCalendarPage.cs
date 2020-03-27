// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCalendarPage.cs" company="Aurea Software Gmbh">
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
//   The calendar page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Pages
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The calendar page.
    /// </summary>
    public class UPMCalendarPage : UPMSearchPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCalendarPage"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMCalendarPage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the add appointment actions.
        /// </summary>
        public List<UPConfigButton> AddAppointmentActions { get; set; }

        /// <summary>
        /// Gets or sets the calendar from date.
        /// </summary>
        public DateTime? CalendarFromDate { get; set; }

        /// <summary>
        /// Gets or sets the calendar to date.
        /// </summary>
        public DateTime? CalendarToDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether desc sort.
        /// </summary>
        public bool DescSort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether include system calendar.
        /// </summary>
        public bool IncludeSystemCalendar { get; set; }

        /// <summary>
        /// Gets or sets the popover title.
        /// </summary>
        public string PopoverTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show system calendar.
        /// </summary>
        public bool ShowSystemCalendar { get; set; }

        /// <summary>
        /// Gets or sets the calendar items.
        /// </summary>
        /// <value>
        /// The calendar items.
        /// </value>
        public List<ICalendarItem> CalendarItems { get; set; }

        /// <summary>
        /// Copies data from another page.
        /// </summary>
        /// <param name="otherPage">The other page.</param>
        public override void CopyDataFrom(UPMSearchPage otherPage)
        {
            base.CopyDataFrom(otherPage);

            UPMCalendarPage calendarPage = otherPage as UPMCalendarPage;

            if (calendarPage != null)
            {
                this.CalendarFromDate = calendarPage.CalendarFromDate;
                this.CalendarToDate = calendarPage.CalendarToDate;
                this.IncludeSystemCalendar = calendarPage.IncludeSystemCalendar;
                this.ShowSystemCalendar = calendarPage.ShowSystemCalendar;
                this.AddAppointmentActions = calendarPage.AddAppointmentActions;
                this.DescSort = calendarPage.DescSort;
                this.DefaultViewType = calendarPage.DefaultViewType;
            }
        }
    }
}
