// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCalendarGroup.cs" company="Aurea Software Gmbh">
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
//   The CalendarGroupDelegate interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// The CalendarGroupDelegate interface.
    /// </summary>
    public interface ICalendarGroupDelegate
    {
        /// <summary>
        /// Gets a value indicating whether is output range enabled.
        /// </summary>
        bool IsOutputRangeEnabled { get; }

        /// <summary>
        /// The calendar group did select date.
        /// </summary>
        /// <param name="calendarGroup">
        /// The calendar group.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        void CalendarGroupDidSelectDate(UPMCalendarGroup calendarGroup, DateTime? date);

        /// <summary>
        /// The calendar group did select from date to date.
        /// </summary>
        /// <param name="calendarGroup">
        /// The calendar group.
        /// </param>
        /// <param name="fromDate">
        /// The from date.
        /// </param>
        /// <param name="toDate">
        /// The to date.
        /// </param>
        void CalendarGroupDidSelectFromDateToDate(UPMCalendarGroup calendarGroup, DateTime? fromDate, DateTime? toDate);
        void CalendarGroupDidSelectDate(UPMCalendarGroup calendarGroup, DateTime date);
        void CalendarGroupDidSelectFromDateToDate(UPMCalendarGroup calendarGroup, DateTime fromDate, DateTime toDate);
    }

    /// <summary>
    /// The upm calendar group.
    /// </summary>
    public class UPMCalendarGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCalendarGroup"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMCalendarGroup(IIdentifier identifier)
            : base(identifier)
        {
            UPMStringField field = new UPMStringField(identifier);
            this.AddChild(field);
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        public ICalendarGroupDelegate Delegate { get; set; }
    }
}
