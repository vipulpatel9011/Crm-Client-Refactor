// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerTimeZone.cs" company="Aurea Software Gmbh">
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
//   Date and time formatter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Globalization;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// CRM time zone
    /// </summary>
    public class UPCRMTimeZone
    {
        private TimeZoneInfo clientDataTimeZone;
        private TimeZoneInfo currentTimeZone;
        private CultureInfo clientCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMTimeZone"/> class.
        /// </summary>
        public UPCRMTimeZone()
            : this((TimeZoneInfo)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMTimeZone"/> class.
        /// </summary>
        /// <param name="clientCulture">
        /// Name of the time zone.
        /// </param>
        public UPCRMTimeZone(CultureInfo clientCulture)
            : this((TimeZoneInfo)null, clientCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMTimeZone"/> class.
        /// </summary>
        /// <param name="timeZoneName">
        /// Name of the time zone.
        /// </param>
        /// <param name="clientCulture">
        /// Client culture information.
        /// </param>
        public UPCRMTimeZone(string timeZoneName, CultureInfo clientCulture = null)
            : this(FindTimeZoneByName(timeZoneName), clientCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMTimeZone"/> class.
        /// </summary>
        /// <param name="clientDataTimeZone">
        /// The client data time zone.
        /// </param>
        /// <param name="clientCulture">
        /// Name of the time zone.
        /// </param>
        public UPCRMTimeZone(TimeZoneInfo clientDataTimeZone, CultureInfo clientCulture = null)
        {
            this.clientCulture = clientCulture;
            this.ClientDataTimeZone = clientDataTimeZone;
            this.CurrentTimeZone = TimeZoneInfo.Local;
            this.NeedsTimeZoneAdjustment = this.ClientDataTimeZone?.DisplayName != this.CurrentTimeZone.DisplayName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMTimeZone"/> class.
        /// </summary>
        /// <param name="clientDataTimeZoneName">
        /// Name of the client data time zone.
        /// </param>
        /// <param name="currentTimeZoneName">
        /// Name of the current time zone.
        /// </param>
        public UPCRMTimeZone(string clientDataTimeZoneName, string currentTimeZoneName)
        {
            this.ClientDataTimeZone = FindTimeZoneByName(clientDataTimeZoneName);
            this.CurrentTimeZone = FindTimeZoneByName(currentTimeZoneName);
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static UPCRMTimeZone Current => ServerSession.CurrentSession?.TimeZone;

        /// <summary>
        /// Gets or sets the client data time zone.
        /// </summary>
        /// <value>
        /// The client data time zone.
        /// </value>
        public TimeZoneInfo ClientDataTimeZone
        {
            get
            {
                return this.clientDataTimeZone;
            }

            set
            {
                this.clientDataTimeZone = value;

                this.ClientDataDateTimeFormatter = new DateTimeFormatter(this.clientDataTimeZone, "yyyyMMdd HHmm");
                this.ClientDataDateFormatter = new DateTimeFormatter(this.clientDataTimeZone, "yyyyMMdd");
                this.ClientDataTimeFormatter = new DateTimeFormatter(this.clientDataTimeZone, "HHmm");
                this.ClientDataTimeSecFormatter = new DateTimeFormatter(this.clientDataTimeZone, "HHmmss");
            }
        }

        /// <summary>
        /// Gets or sets the current time zone.
        /// </summary>
        /// <value>
        /// The current time zone.
        /// </value>
        public TimeZoneInfo CurrentTimeZone
        {
            get
            {
                return this.currentTimeZone;
            }

            set
            {
                this.currentTimeZone = value;

                var standardDateFormat = "yyyy-MM-dd";
                var standardTimeFormat = "hhmm";
                var standardDateTimeFormat = "yyyyMMdd HHmm";

                // If the client culture has been provided for the session load the values 
                // of client's OS culture
                if (this.clientCulture != null)
                {
                    standardDateFormat = this.clientCulture.DateTimeFormat.ShortDatePattern;
                    standardTimeFormat = this.clientCulture.DateTimeFormat.ShortTimePattern;
                    standardDateTimeFormat = this.clientCulture.DateTimeFormat.FullDateTimePattern;
                }

                this.CurrentDateFormatter = new DateTimeFormatter(this.currentTimeZone, "yyyyMMdd");
                this.CurrentTimeFormatter = new DateTimeFormatter(this.currentTimeZone, standardTimeFormat);
                this.CurrentTimeSecFormatter = new DateTimeFormatter(this.currentTimeZone, "HHmmss");
                this.CurrentDateTimeFormatter = new DateTimeFormatter(this.currentTimeZone, standardDateTimeFormat);

                this.StandardDateFormatter = new DateTimeFormatter(this.currentTimeZone, standardDateFormat);
                this.EditDateFormatter = new DateTimeFormatter(this.currentTimeZone, "dddd, MMMM d, yyyy");
                this.ShortDateFormatter = new DateTimeFormatter(this.currentTimeZone, standardDateFormat);
                this.StandardTimeFormatter = new DateTimeFormatter(this.currentTimeZone, standardTimeFormat);
            }
        }

        /// <summary>
        /// Gets the client data date formatter.
        /// </summary>
        /// <value>
        /// The client data date formatter.
        /// </value>
        public DateTimeFormatter ClientDataDateFormatter { get; private set; }

        /// <summary>
        /// Gets the client data date time formatter.
        /// </summary>
        /// <value>
        /// The client data date time formatter.
        /// </value>
        public DateTimeFormatter ClientDataDateTimeFormatter { get; private set; }

        /// <summary>
        /// Gets the client data time formatter.
        /// </summary>
        /// <value>
        /// The client data time formatter.
        /// </value>
        public DateTimeFormatter ClientDataTimeFormatter { get; private set; }

        /// <summary>
        /// Gets the client data time sec formatter.
        /// </summary>
        /// <value>
        /// The client data time sec formatter.
        /// </value>
        public DateTimeFormatter ClientDataTimeSecFormatter { get; private set; }

        /// <summary>
        /// Gets the current date formatter.
        /// </summary>
        /// <value>
        /// The current date formatter.
        /// </value>
        public DateTimeFormatter CurrentDateFormatter { get; private set; }

        /// <summary>
        /// Gets the current date time formatter.
        /// </summary>
        /// <value>
        /// The current date time formatter.
        /// </value>
        public DateTimeFormatter CurrentDateTimeFormatter { get; private set; }

        /// <summary>
        /// Gets the current time formatter.
        /// </summary>
        /// <value>
        /// The current time formatter.
        /// </value>
        public DateTimeFormatter CurrentTimeFormatter { get; private set; }

        /// <summary>
        /// Gets the current time sec formatter.
        /// </summary>
        /// <value>
        /// The current time sec formatter.
        /// </value>
        public DateTimeFormatter CurrentTimeSecFormatter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [needs time zone adjustment].
        /// </summary>
        /// <value>
        /// <c>true</c> if [needs time zone adjustment]; otherwise, <c>false</c>.
        /// </value>
        public bool NeedsTimeZoneAdjustment { get; private set; }

        /// <summary>
        /// Gets the edit date formatter.
        /// </summary>
        /// <value>
        /// Edit date formatter.
        /// </value>
        public DateTimeFormatter EditDateFormatter { get; private set; }

        /// <summary>
        /// Gets the short date formatter.
        /// </summary>
        /// <value>
        /// Short date formatter.
        /// </value>
        public DateTimeFormatter ShortDateFormatter { get; private set; }

        /// <summary>
        /// Gets the standard date formatter.
        /// </summary>
        /// <value>
        /// Standard date formatter.
        /// </value>
        public DateTimeFormatter StandardDateFormatter { get; private set; }

        /// <summary>
        /// Gets the standard time formatter.
        /// </summary>
        /// <value>
        /// Standard time formatter.
        /// </value>
        public DateTimeFormatter StandardTimeFormatter { get; private set; }

        /// <summary>
        /// Dates from client data mm date string time string.
        /// </summary>
        /// <param name="mmDate">
        /// The mm date.
        /// </param>
        /// <param name="mmTime">
        /// The mm time.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime? DateFromClientDataMMDateStringTimeString(string mmDate, string mmTime)
        {
            string dateTimeBuf = $"{mmDate} {mmTime}";
            if (this.ClientDataDateTimeFormatter != null)
            {
                return this.ClientDataDateTimeFormatter.DateFromString(dateTimeBuf);
            }

            return dateTimeBuf.DateTimeFromCrmValue();
        }

        /// <summary>
        /// Dates from client data mm date time.
        /// </summary>
        /// <param name="mmDate">
        /// The mm date.
        /// </param>
        /// <param name="mmTime">
        /// The mm time.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime? DateFromClientDataMMDateTime(string mmDate, string mmTime)
        {
            var dateTimeBuf = $"{mmDate} {mmTime}";
            if (this.ClientDataDateTimeFormatter != null)
            {
                return this.ClientDataDateTimeFormatter.DateFromString(dateTimeBuf);
            }

            return dateTimeBuf.DateTimeFromCrmValue();
        }

        /// <summary>
        /// Dates from current data mm date string time string.
        /// </summary>
        /// <param name="mmDate">
        /// The mm date.
        /// </param>
        /// <param name="mmTime">
        /// The mm time.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime? DateFromCurrentDataMMDateStringTimeString(string mmDate, string mmTime)
        {
            var dateTimeBuf = $"{mmDate} {mmTime}";
            if (this.CurrentDateTimeFormatter != null)
            {
                return this.CurrentDateTimeFormatter.DateFromString(dateTimeBuf);
            }

            return dateTimeBuf.DateTimeFromCrmValue();
        }

        /// <summary>
        /// Gets the adjusted client data mm date.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAdjustedClientDataMMDate(DateTime date)
        {
            if (this.ClientDataDateFormatter != null)
            {
                return this.ClientDataDateFormatter.StringFromDate(date);
            }

            return date.CrmValueFromDate();
        }

        /// <summary>
        /// Gets the adjusted client data mm time.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAdjustedClientDataMMTime(DateTime? date)
        {
            if (this.ClientDataTimeFormatter != null)
            {
                return this.ClientDataTimeFormatter.StringFromDate(date);
            }

            return date.CrmValueFromTime();
        }

        /// <summary>
        /// Gets the adjusted current mm date.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAdjustedCurrentMMDate(DateTime date)
        {
            if (this.CurrentDateFormatter != null)
            {
                return this.CurrentDateFormatter.StringFromDate(date);
            }

            return date.CrmValueFromDate();
        }

        /// <summary>
        /// Gets the adjusted current mm date from c date c time.
        /// </summary>
        /// <param name="mmDate">
        /// The mm date.
        /// </param>
        /// <param name="mmTime">
        /// The mm time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAdjustedCurrentMMDateFromCDateCTime(string mmDate, string mmTime)
        {
            if (!this.NeedsTimeZoneAdjustment)
            {
                return mmDate;
            }

            var date = this.DateFromClientDataMMDateTime(mmDate, mmTime);
            return date != null ? this.GetAdjustedCurrentMMDate(date.Value) : mmDate;
        }

        /// <summary>
        /// Gets the adjusted current mm date from date time.
        /// </summary>
        /// <param name="mmDate">
        /// The mm date.
        /// </param>
        /// <param name="mmTime">
        /// The mm time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAdjustedCurrentMMDateFromDateTime(string mmDate, string mmTime)
        {
            if (!this.NeedsTimeZoneAdjustment)
            {
                return mmDate;
            }

            DateTime? date = this.DateFromClientDataMMDateStringTimeString(mmDate, mmTime);
            return date != null ? this.GetAdjustedCurrentMMDate(date.Value) : mmDate;
        }

        /// <summary>
        /// Gets the adjusted current mm time.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAdjustedCurrentMMTime(DateTime? date)
        {
            if (this.CurrentTimeFormatter != null)
            {
                return this.CurrentTimeFormatter.StringFromDate(date);
            }

            return date.CrmValueFromTime();
        }

        /// <summary>
        /// Gets the adjusted current mm time from c date c time.
        /// </summary>
        /// <param name="mmDate">
        /// The mm date.
        /// </param>
        /// <param name="mmTime">
        /// The mm time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAdjustedCurrentMMTimeFromCDateCTime(string mmDate, string mmTime)
        {
            if (!this.NeedsTimeZoneAdjustment)
            {
                return mmTime;
            }

            DateTime? date = this.DateFromClientDataMMDateTime(mmDate, mmTime);
            return date != null ? this.GetAdjustedCurrentMMTime(date.Value) : mmTime;
        }

        /// <summary>
        /// Gets the adjusted current mm time from date time.
        /// </summary>
        /// <param name="mmDate">
        /// The mm date.
        /// </param>
        /// <param name="mmTime">
        /// The mm time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAdjustedCurrentMMTimeFromDateTime(string mmDate, string mmTime)
        {
            if (!this.NeedsTimeZoneAdjustment)
            {
                return mmTime;
            }

            DateTime? date = this.DateFromClientDataMMDateStringTimeString(mmDate, mmTime);
            return date != null ? this.GetAdjustedCurrentMMTime(date.Value) : mmTime;
        }

        /// <summary>
        /// Updates the local time.
        /// </summary>
        public void UpdateLocalTime()
        {
            this.CurrentTimeZone = TimeZoneInfo.Local;
            this.NeedsTimeZoneAdjustment = false;
        }

        private static TimeZoneInfo FindTimeZoneByName(string timeZoneName)
        {
            return TimeZoneInfo.Local;
        }
    }
}
