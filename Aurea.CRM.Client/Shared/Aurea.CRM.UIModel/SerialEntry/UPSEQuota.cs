// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEQuota.cs" company="Aurea Software Gmbh">
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
//   UPSEQuota
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPSEQuota
    /// </summary>
    public class UPSEQuota
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEQuota"/> class.
        /// </summary>
        /// <param name="_itemNumber">The item number.</param>
        /// <param name="_quotaHandler">The quota handler.</param>
        public UPSEQuota(string _itemNumber, UPSEQuotaHandler _quotaHandler)
        {
            this.ItemNumber = _itemNumber;
            this.QuotaHandler = _quotaHandler;
            this.QuotaDate = _quotaHandler.Date;
        }

        /// <summary>
        /// Gets the item number.
        /// </summary>
        /// <value>
        /// The item number.
        /// </value>
        public string ItemNumber { get; private set; }

        /// <summary>
        /// Gets the quota date.
        /// </summary>
        /// <value>
        /// The quota date.
        /// </value>
        public DateTime QuotaDate { get; private set; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Gets the issued items.
        /// </summary>
        /// <value>
        /// The issued items.
        /// </value>
        public int IssuedItems { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the first end date.
        /// </summary>
        /// <value>
        /// The first end date.
        /// </value>
        public DateTime? FirstEndDate { get; private set; }

        /// <summary>
        /// Gets the quota handler.
        /// </summary>
        /// <value>
        /// The quota handler.
        /// </value>
        public UPSEQuotaHandler QuotaHandler { get; private set; }

        /// <summary>
        /// Gets the first start date.
        /// </summary>
        /// <value>
        /// The first start date.
        /// </value>
        public DateTime? FirstStartDate { get; private set; }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        public void ApplyResultRow(UPCRMResultRow resultRow)
        {
            this.ApplyRecordIdentificationValues(resultRow.RootRecordIdentification, resultRow.ValuesWithFunctions());
        }

        /// <summary>
        /// Applies the record identification values.
        /// </summary>
        /// <param name="_recordIdentification">The record identification.</param>
        /// <param name="values">The values.</param>
        public void ApplyRecordIdentificationValues(string _recordIdentification, Dictionary<string, object> values)
        {
            //string endDateString = values.ValueOrDefault("EndDate") as string;
            //if (string.IsNullOrEmpty(endDateString) || endDateString.Length < 5)
            //{
            //    string yearString = values.ValueOrDefault("Year") as string;
            //    if (!string.IsNullOrEmpty(yearString))
            //    {
            //        endDateString = NSString.StringWithFormat("%04ld0101", (long)yearString.IntegerValue + 1);
            //    }
            //}

            //DateTime currentEndDate = endDateString.DateFromCrmValue();
            //if (currentEndDate == null)
            //{
            //    return;
            //}

            //string startDateString = values["StartDate"] as string;
            //DateTime currentStartDate = startDateString.DateFromCrmValue();
            //if (currentStartDate != null && (this.FirstStartDate == null || this.FirstStartDate.Compare(currentStartDate) == NSOrderedDescending))
            //{
            //    this.FirstStartDate = currentStartDate;
            //}

            //if (this.FirstEndDate == null || this.FirstEndDate.Compare(currentEndDate) == NSOrderedDescending)
            //{
            //    this.FirstEndDate = currentEndDate;
            //}

            //bool applyRecord = false;
            //if (currentEndDate.Compare(this.QuotaDate) == NSOrderedDescending)
            //{
            //    NSCalendar calendar = NSCalendar.CurrentCalendar();
            //    DateTimeComponents dateComponents = DateTimeComponents.TheNew();
            //    dateComponents.Year = -1;
            //    applyRecord = calendar.DateByAddingComponentsToDateOptions(dateComponents, currentEndDate, 0).Compare(this.QuotaDate) != NSOrderedDescending;
            //    if (applyRecord && this.QuotaHandler.CalendarYearPeriods)
            //    {
            //        DateTimeComponents currentEndDateComponents = calendar.ComponentsFromDate(NSCalendarUnitYear | NSCalendarUnitMonth | NSCalendarUnitDay, currentEndDate);
            //        if (currentEndDateComponents.Day != 1 && currentEndDateComponents.Month != 1)
            //        {
            //            DateTimeComponents quotaComponents = calendar.ComponentsFromDate(NSCalendarUnitYear, this.QuotaDate);
            //            if (quotaComponents.Year != currentEndDateComponents.Year)
            //            {
            //                applyRecord = false;
            //            }
            //        }
            //    }
            //}

            //if (applyRecord)
            //{
            //    this.EndDate = currentEndDate;
            //    this.RecordIdentification = _recordIdentification;
            //    string issuedItemString = values.ValueOrDefault("Items") as string;
            //    this.IssuedItems = !string.IsNullOrEmpty(issuedItemString) ? Convert.ToInt32(issuedItemString) : 0;
            //}
        }

        /// <summary>
        /// Gets a value indicating whether this instance has valid quota record.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has valid quota record; otherwise, <c>false</c>.
        /// </value>
        public bool HasValidQuotaRecord => !string.IsNullOrEmpty(this.RecordIdentification);

        DateTime QuotaStartDateForPeriod()
        {
            return DateTime.UtcNow;
            //NSCalendar calendar = NSCalendar.CurrentCalendar();
            //DateTimeComponents plusOneYear = DateTimeComponents.TheNew();
            //plusOneYear.Year = 1;
            //DateTime? startDate = this.FirstStartDate, nextDate;
            //if (this.QuotaHandler.CalendarYearPeriods && startDate)
            //{
            //    DateTimeComponents firstDateComponents = calendar.ComponentsFromDate(NSCalendarUnitYear | NSCalendarUnitMonth | NSCalendarUnitDay, startDate);
            //    firstDateComponents.Year++;
            //    firstDateComponents.Month = 1;
            //    firstDateComponents.Day = 1;
            //    nextDate = calendar.DateFromComponents(firstDateComponents);
            //}
            //else
            //{
            //    if (startDate != null)
            //    {
            //        nextDate = calendar.DateByAddingComponentsToDateOptions(plusOneYear, startDate, 0);
            //    }
            //    else if (this.FirstEndDate != null)
            //    {
            //        nextDate = this.FirstEndDate;
            //    }
            //}

            //if (nextDate == null)
            //{
            //    return this.QuotaDate;
            //}

            //while (nextDate.Compare(this.QuotaDate) == NSOrderedAscending)
            //{
            //    startDate = nextDate;
            //    nextDate = calendar.DateByAddingComponentsToDateOptions(plusOneYear, startDate, 0);
            //}

            //return startDate;
        }

        DateTime QuotaEndDateForPeriod()
        {
            return DateTime.UtcNow;
            //DateTimeComponents plusOneYear = DateTimeComponents.TheNew();
            //plusOneYear.Year = 1;
            //DateTime startDate;
            //NSCalendar calendar = NSCalendar.CurrentCalendar();
            //if (this.FirstEndDate != null)
            //{
            //    startDate = this.FirstEndDate.Value;
            //    while (startDate.Compare(this.QuotaDate) == NSOrderedAscending)
            //    {
            //        startDate = calendar.DateByAddingComponentsToDateOptions(plusOneYear, startDate, 0);
            //    }

            //    return startDate;
            //}

            //if (this.QuotaHandler.CalendarYearPeriods)
            //{
            //    DateTimeComponents firstDateComponents = calendar.ComponentsFromDate(NSYearCalendarUnit | NSMonthCalendarUnit | NSDayCalendarUnit, this.QuotaDate);
            //    firstDateComponents.Year++;
            //    firstDateComponents.Month = 1;
            //    firstDateComponents.Day = 1;
            //    return calendar.DateFromComponents(firstDateComponents);
            //}

            //return calendar.DateByAddingComponentsToDateOptions(plusOneYear, this.QuotaDate, 0);
        }

        DateTime? LastEndDateForPeriodCount(int periodCount)
        {
            //NSCalendar calendar = NSCalendar.CurrentCalendar();
            //if (this.FirstStartDate != null)
            //{
            //    DateTimeComponents components = DateTimeComponents.TheNew();
            //    components.Year = periodCount;
            //    return calendar.DateByAddingComponentsToDateOptions(components, this.FirstStartDate, 0);
            //}
            //else if (this.FirstEndDate != null)
            //{
            //    DateTimeComponents components = DateTimeComponents.TheNew();
            //    components.Year = periodCount - 1;
            //    return calendar.DateByAddingComponentsToDateOptions(components, this.FirstEndDate, 0);
            //}
            //else
            //{
            //    DateTimeComponents components = DateTimeComponents.TheNew();
            //    components.Year = 1;
            //    return calendar.DateByAddingComponentsToDateOptions(components, DateTime.TheNew(), 0);
            //}

            return null;
        }

        /// <summary>
        /// Periods the creation allowed with maximum period count.
        /// </summary>
        /// <param name="periodCount">The period count.</param>
        /// <returns></returns>
        public bool PeriodCreationAllowedWithMaxPeriodCount(int periodCount)
        {
            //if (this.RecordIdentification.Length == 0)
            //{
            //    if (this.FirstEndDate == null)
            //    {
            //        return periodCount > 0 ? true : false;
            //    }

            //    DateTime? lastEndDate = this.LastEndDateForPeriodCount(periodCount);
            //    if (lastEndDate.Compare(this.QuotaDate) == NSOrderedDescending)
            //    {
            //        return true;
            //    }
            //}

            return false;
        }

        /// <summary>
        /// Offlines the record for count.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public UPCRMRecord OfflineRecordForCount(int count)
        {
            if (this.QuotaHandler.QuotaEditFieldControl == null)
            {
                return null;
            }

            UPCRMRecord record;
            if (!string.IsNullOrEmpty(this.RecordIdentification))
            {
                record = new UPCRMRecord(this.RecordIdentification, "UpdateOffline", null);
            }
            else
            {
                record = UPCRMRecord.CreateNew(this.QuotaHandler.QuotaEditFieldControl.InfoAreaId);
                record.Mode = "NewOffline";
                UPConfigFilter templateFilter = this.QuotaHandler.QuotaTemplateFilter;
                if (templateFilter != null && this.QuotaHandler.FilterParameters.Count > 0)
                {
                    templateFilter = templateFilter.FilterByApplyingValueDictionaryDefaults(this.QuotaHandler.FilterParameters, true);
                }

                if (templateFilter != null)
                {
                    record.ApplyValuesFromTemplateFilter(templateFilter);
                }

                foreach (UPConfigFieldControlField field in this.QuotaHandler.QuotaEditFieldControl.Fields)
                {
                    if (field.Function == "StartDate")
                    {
                        string dateString = StringExtensions.CrmValueFromDate(this.FirstStartDate != null ? this.QuotaStartDateForPeriod() : DateTime.UtcNow);

                        record.AddValue(new UPCRMFieldValue(dateString, record.InfoAreaId, field.FieldId));
                    }
                    else if (field.Function == "EndDate")
                    {
                        string dateString = StringExtensions.CrmValueFromDate(this.QuotaEndDateForPeriod());
                        record.AddValue(new UPCRMFieldValue(dateString, record.InfoAreaId, field.FieldId));
                    }
                    else if (field.Function == this.QuotaHandler.ItemNumberFunctionName)
                    {
                        record.AddValue(new UPCRMFieldValue(this.ItemNumber, record.InfoAreaId, field.FieldId));
                    }
                }

                record.AddLink(new UPCRMLink(this.QuotaHandler.LinkRecord.RecordIdentification, this.QuotaHandler.QuotaLinkId));
            }

            foreach (UPConfigFieldControlField field in this.QuotaHandler.QuotaEditFieldControl.Fields)
            {
                if (field.Function == "Items" || field.Function.StartsWith("Items:"))
                {
                    record.AddValue(new UPCRMFieldValue(count.ToString(), record.InfoAreaId, field.FieldId, true));
                }
            }

            return record;
        }
    }
}
