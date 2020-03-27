// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalendarGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The Calendar Group Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// The Calendar Group Model Controller
    /// </summary>
    /// <seealso cref="UPGroupModelController" />
    /// <seealso cref="Aurea.CRM.UIModel.Groups.ICalendarGroupDelegate" />
    public class UPCalendarGroupModelController : UPGroupModelController, ICalendarGroupDelegate
    {
        /// <summary>
        /// Gets the current date string.
        /// </summary>
        /// <value>
        /// The current date string.
        /// </value>
        public string CurrentDateString { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [output date range].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [output date range]; otherwise, <c>false</c>.
        /// </value>
        public bool OutputDateRange { get; private set; }

        /// <summary>
        /// Gets or sets the last calendar selection date.
        /// </summary>
        /// <value>
        /// The last calendar selection date.
        /// </value>
        public DateTime? LastCalendarSelectionDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCalendarGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPCalendarGroupModelController(FormItem formItem, IIdentifier identifier, IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
            this.FormItem = formItem;

            this.ExplicitTabIdentifier = identifier;

            if (formItem.Options?.ContainsKey(@"OutputRange") == true && Convert.ToInt32(formItem.Options[@"OutputRange"]) != 0)
            {
                this.OutputDateRange = true;
            }

            string _currentValue = null;

            if (!string.IsNullOrEmpty(formItem.ValueName))
            {
                _currentValue = this.Delegate.GroupModelControllerValueForKey(this, "$" + formItem.ValueName);
            }

            if (!string.IsNullOrEmpty(_currentValue))
            {
                this.CurrentDateString = _currentValue;
            }
            else
            {
                this.CurrentDateString = formItem.ViewReference.ContextValueForKey(@"Date");

                if (string.IsNullOrEmpty(this.CurrentDateString))
                {
                    this.CurrentDateString = StringExtensions.CrmValueFromDate(DateTime.Now);

                    if (this.OutputDateRange)
                    {
                        this.CurrentDateString = string.Format("{0};{0}", this.CurrentDateString);
                    }
                }
                else
                {
                    this.CurrentDateString = this.CurrentDateString.ReplaceDateVariables();
                }
            }
        }

        /// <summary>
        /// Applies the context.
        /// </summary>
        /// <param name="contextDictionary">The context dictionary.</param>
        /// <returns></returns>
        public override UPMGroup ApplyContext(Dictionary<string, object> contextDictionary)
        {
            base.ApplyContext(contextDictionary);
            DateTime? currentDate = this.CurrentDateString.DateFromCrmValue();

            if (this.OutputDateRange)
            {
                var chunks = this.CurrentDateString.Split(';');
                currentDate = chunks[0].DateFromCrmValue();
            }

            UPMCalendarGroup calendarGroup = new UPMCalendarGroup(this.ExplicitTabIdentifier)
            {
                Date = currentDate.Value,
                Delegate = this
            };

            this.Group = calendarGroup;
            this.ControllerState = GroupModelControllerState.Finished;

            return calendarGroup;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value => this.CurrentDateString;

        /// <summary>
        /// Gets a value indicating whether is output range enabled.
        /// </summary>
        public bool IsOutputRangeEnabled => this.OutputDateRange;

        /// <summary>
        /// Calendars the group did select date.
        /// </summary>
        /// <param name="calendarGroup">The calendar group.</param>
        /// <param name="date">The date.</param>
        public void CalendarGroupDidSelectDate(UPMCalendarGroup calendarGroup, DateTime? date)
        {
            if (this.OutputDateRange)
            {
                this.CalendarGroupDidSelectFromDateToDate(calendarGroup, date, date);
                return;
            }

            string dateString = StringExtensions.CrmValueFromDate(date);
            this.CurrentDateString = dateString;

            // TODO: Very important
            // dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, 0), ^{

            if (!ServerSession.CurrentSession.IsEnterprise)
            {
                this.Delegate.GroupModelControllerValueChanged(this, this.CurrentDateString);
            }
            else
            {
                // [Enterprise only] The code bellow avoids requesting multiple time the database when fast changing dates on dashboard calendar

                DateTime currentDate = DateTime.Now;

                if (this.LastCalendarSelectionDate != null)
                {
                    var interval = this.LastCalendarSelectionDate - currentDate;
                    this.LastCalendarSelectionDate = currentDate;

                    if (interval.Value.Milliseconds < 50)
                    {
                        // [NSThread sleepForTimeInterval:0.5];
                    }
                }
                else
                {
                    this.LastCalendarSelectionDate = currentDate;
                }

                if (dateString == this.CurrentDateString)
                {
                    this.Delegate.GroupModelControllerValueChanged(this, this.CurrentDateString);
                }
            }
            // });
        }

        /// <summary>
        /// The calendar group did select from date to date.
        /// </summary>
        /// <param name="calendarGroup">The calendar group.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        public void CalendarGroupDidSelectFromDateToDate(UPMCalendarGroup calendarGroup, DateTime? fromDate, DateTime? toDate)
        {
            if (!this.OutputDateRange)
            {
                this.CalendarGroupDidSelectDate(calendarGroup, fromDate);
                return;
            }

            this.CurrentDateString = $"{StringExtensions.CrmValueFromDate(fromDate)};{StringExtensions.CrmValueFromDate(toDate)}";
            this.Delegate.GroupModelControllerValueChanged(this, this.CurrentDateString);
        }

        public void CalendarGroupDidSelectDate(UPMCalendarGroup calendarGroup, DateTime date)
        {
        }

        public void CalendarGroupDidSelectFromDateToDate(UPMCalendarGroup calendarGroup, DateTime fromDate, DateTime toDate)
        {
            
        }
    }
}
