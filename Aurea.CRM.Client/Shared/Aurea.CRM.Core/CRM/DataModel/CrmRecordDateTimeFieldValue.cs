// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmRecordDateTimeFieldValue.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   CRM record Date Time Field Value class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// CRM record date time field value
    /// </summary>
    public class UPCRMRecordDateTimeFieldValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordDateTimeFieldValue"/> class.
        /// </summary>
        /// <param name="dateFieldId">
        /// The _date field identifier.
        /// </param>
        /// <param name="timeFieldId">
        /// The _time field identifier.
        /// </param>
        public UPCRMRecordDateTimeFieldValue(int dateFieldId, int timeFieldId)
        {
            this.DateFieldId = dateFieldId;
            this.TimeFieldId = timeFieldId;
        }

        /// <summary>
        /// Gets the date field identifier.
        /// </summary>
        /// <value>
        /// The date field identifier.
        /// </value>
        public int DateFieldId { get; private set; }

        /// <summary>
        /// Gets or sets the date field value.
        /// </summary>
        /// <value>
        /// The date field value.
        /// </value>
        public UPCRMFieldValue DateFieldValue { get; set; }

        /// <summary>
        /// Gets the time field identifier.
        /// </summary>
        /// <value>
        /// The time field identifier.
        /// </value>
        public int TimeFieldId { get; private set; }

        /// <summary>
        /// Gets or sets the time field value.
        /// </summary>
        /// <value>
        /// The time field value.
        /// </value>
        public UPCRMFieldValue TimeFieldValue { get; set; }

        /// <summary>
        /// Applies the time zone record.
        /// </summary>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ApplyTimeZoneRecord(UPCRMTimeZone timeZone, UPCRMRecord record)
        {
            DateTime? oldDate = null;
            DateTime? date = null;
            if (this.DateFieldValue != null)
            {
                if (this.TimeFieldValue != null)
                {
                    if (!string.IsNullOrEmpty(this.DateFieldValue.OldValue) && !string.IsNullOrEmpty(this.TimeFieldValue.OldValue))
                    {
                        oldDate = timeZone.DateFromCurrentDataMMDateStringTimeString(
                            this.DateFieldValue.OldValue,
                            this.TimeFieldValue.OldValue);
                    }

                    if (!string.IsNullOrEmpty(this.DateFieldValue.Value) && !string.IsNullOrEmpty(this.TimeFieldValue.Value))
                    {
                        date = timeZone.DateFromCurrentDataMMDateStringTimeString(
                            this.DateFieldValue.Value,
                            this.TimeFieldValue.Value);
                    }
                }
                else if (!string.IsNullOrEmpty(this.DateFieldValue.TimeOriginalValue))
                {
                    if (!string.IsNullOrEmpty(this.DateFieldValue.OldValue))
                    {
                        oldDate = timeZone.DateFromCurrentDataMMDateStringTimeString(
                            this.DateFieldValue.OldValue,
                            this.DateFieldValue.TimeOriginalValue);
                    }

                    if (!string.IsNullOrEmpty(this.DateFieldValue.Value))
                    {
                        date = timeZone.DateFromCurrentDataMMDateStringTimeString(
                            this.DateFieldValue.Value,
                            this.DateFieldValue.TimeOriginalValue);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(this.TimeFieldValue.DateOriginalValue))
            {
                /* -> dateFieldValue == nil && timeFieldValue != nil */
                if (!string.IsNullOrEmpty(this.TimeFieldValue.OldValue))
                {
                    oldDate = timeZone.DateFromCurrentDataMMDateStringTimeString(
                        this.TimeFieldValue.DateOriginalValue,
                        this.TimeFieldValue.OldValue);
                }

                if (!string.IsNullOrEmpty(this.TimeFieldValue.Value))
                {
                    date = timeZone.DateFromCurrentDataMMDateStringTimeString(
                        this.TimeFieldValue.DateOriginalValue,
                        this.TimeFieldValue.Value);
                }
            }

            string changedOldDate = null;
            if (oldDate != null)
            {
                changedOldDate = timeZone.GetAdjustedClientDataMMDate(oldDate.Value);
                var changedOldTime = timeZone.GetAdjustedClientDataMMTime(oldDate.Value);
                if (!string.IsNullOrEmpty(this.DateFieldValue?.OldValue))
                {
                    this.DateFieldValue.OldValue = changedOldDate;
                }

                if (!string.IsNullOrEmpty(this.TimeFieldValue?.OldValue))
                {
                    this.TimeFieldValue.OldValue = changedOldTime;
                }
            }

            if (date != null)
            {
                var changedDate = timeZone.GetAdjustedClientDataMMDate(date.Value);
                var changedTime = timeZone.GetAdjustedClientDataMMTime(date.Value);
                if (!string.IsNullOrEmpty(this.DateFieldValue?.Value))
                {
                    this.DateFieldValue.ChangeValue = changedDate;
                }
                else if (this.DateFieldValue == null && !string.IsNullOrEmpty(changedOldDate)
                         && !string.IsNullOrEmpty(changedDate) && !changedOldDate.Equals(changedDate))
                {
                    var v = new UPCRMFieldValue(
                        changedDate,
                        changedOldDate,
                        this.TimeFieldValue.InfoAreaId,
                        this.DateFieldId,
                        this.TimeFieldValue.OnlyOffline);
                    record.AddValue(v);
                }

                if (string.IsNullOrEmpty(this.TimeFieldValue.Value))
                {
                    this.TimeFieldValue.ChangeValue = changedTime;
                }
            }

            return true;
        }
    }
}
