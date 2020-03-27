// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICalendarItem.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The calendar item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// The calendar item type.
    /// </summary>
    public enum UPCalendarItemType
    {
        /// <summary>
        /// The calendar item type color 0.
        /// </summary>
        Color0 = 0,

        /// <summary>
        /// The calendar item type color 1.
        /// </summary>
        Color1 = 1,

        /// <summary>
        /// The calendar item type color 2.
        /// </summary>
        Color2 = 2,

        /// <summary>
        /// The calendar item type color 3.
        /// </summary>
        Color3 = 3,

        /// <summary>
        /// The calendar item type color 4.
        /// </summary>
        Color4 = 4,

        /// <summary>
        /// The calendar item type color 5.
        /// </summary>
        Color5 = 5,

        /// <summary>
        /// The calendar item type color 6.
        /// </summary>
        Color6 = 6,

        /// <summary>
        /// The calendar item type color 7.
        /// </summary>
        Color7 = 7,

        /// <summary>
        /// The calendar item type color count.
        /// </summary>
        ColorCount = 8,

        /// <summary>
        /// The calendar item type system calendar.
        /// </summary>
        SystemCalendar = 0xFFFF
    }

    /// <summary>
    /// The PCalendarItem interface.
    /// </summary>
    public interface ICalendarItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether all day.
        /// </summary>
        bool AllDay { get; set; }

        /// <summary>
        /// Gets or sets the calendar search.
        /// </summary>
        UPCalendarSearch CalendarSearch { get; set; }

        /// <summary>
        /// Gets the color.
        /// </summary>
        AureaColor Color { get; }

        /// <summary>
        /// Gets the company label field.
        /// </summary>
        UPMField CompanyLabelField { get; }

        /// <summary>
        /// Gets or sets the crm result row.
        /// </summary>
        UPCRMResultRow CrmResultRow { get; set; }

        /// <summary>
        /// Gets or sets the edit action.
        /// </summary>
        UPMAction EditAction { get; set; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        DateTime EndDate { get; }

        /// <summary>
        /// Gets or sets the go to action.
        /// </summary>
        UPMAction GoToAction { get; set; }

        /// <summary>
        /// Gets a value indicating whether has end time.
        /// </summary>
        bool HasEndTime { get; }

        /// <summary>
        /// Gets a value indicating whether has time.
        /// </summary>
        bool HasTime { get; }

        /// <summary>
        /// Gets the identification.
        /// </summary>
        string Identification { get; }

        /// <summary>
        /// Gets the image name.
        /// </summary>
        string ImageName { get; }

        /// <summary>
        /// Gets the person label field.
        /// </summary>
        UPMField PersonLabelField { get; }

        /// <summary>
        /// Gets the rep label field.
        /// </summary>
        UPMField RepLabelField { get; }

        /// <summary>
        /// Gets or sets the result row.
        /// </summary>
        UPMResultRow ResultRow { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        DateTime StartDate { get; set; }

        /// <summary>
        /// Gets the status label field.
        /// </summary>
        UPMField StatusLabelField { get; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        string Subject { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        UPCalendarItemType Type { get; }
    }
}
