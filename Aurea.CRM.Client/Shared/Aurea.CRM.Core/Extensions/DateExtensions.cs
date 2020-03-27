// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateExtensions.cs" company="Aurea Software Gmbh">
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
//   Date Extensions
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Extensions
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Date Extension functions
    /// </summary>
    public static class DateExtensions
    {
        /// <summary>
        /// Gets the base date.
        /// </summary>
        /// <value>
        /// The base date.
        /// </value>
        public static DateTime BaseDate => new DateTime(2001, 1, 1);

        /// <summary>
        /// Gets the interval since1970.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Timespan</returns>
        public static TimeSpan TimeIntervalSince1970(this DateTime source)
        {
            return source - new DateTime(1970, 1, 1);
        }

        /// <summary>
        /// Times the interval since reference date.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Timespan</returns>
        public static TimeSpan TimeIntervalSinceReferenceDate(this DateTime source)
        {
            return source - new DateTime(2001, 1, 1);
        }

        /// <summary>
        /// Returns the interval for the date since now.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Timespan for the date since now.</returns>
        public static TimeSpan TimeIntervalSinceNow(this DateTime source)
        {
            return DateTime.UtcNow - source;
        }

        /// <summary>
        /// Gets the with time interval since now.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>DateTime</returns>
        public static DateTime AddTimeSpanToUtcNow(TimeSpan source)
        {
            return DateTime.UtcNow + source;
        }

        /// <summary>
        /// Date by adding one day.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// DateTime
        /// </returns>
        public static DateTime EndDateForDate(this DateTime source)
        {
            var oneday = new TimeSpan(0, 23, 59, 59);
            return source + oneday;
        }

        /// <summary>
        /// Returns the Localized and formatted date.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string ReportFormattedDate(this DateTime source)
        {
            var formatter = UPCRMTimeZone.Current?.StandardDateFormatter;
            return formatter != null ? formatter.StringFromDate(source) : source.ToString("MMM d, yyyy");
        }

        /// <summary>
        /// Returns the Localized and formatted date.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string LocalizedFormattedDate(this DateTime? source)
        {
            if (source == null)
            {
                return null;
            }

            if (source.Value.Date == DateTime.Today)
            {
                return LocalizedString.TextToday;
            }

            if (source.Value.Date == DateTime.Today.AddDays(-1))
            {
                return LocalizedString.TextYesterday;
            }

            if (source.Value.Date == DateTime.Today.AddDays(1))
            {
                return LocalizedString.TextTomorrow;
            }

            var formatter = UPCRMTimeZone.Current?.StandardDateFormatter;
            return formatter != null ? formatter.StringFromDate(source.Value) : source.Value.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Returns the Localized and formatted date.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string LongLocalizedFormattedDate(this DateTime? source)
        {
            if (source == null)
            {
                return null;
            }

            if (source.Value.Date == DateTime.Today)
            {
                return LocalizedString.TextToday;
            }

            if (source.Value.Date == DateTime.Today.AddDays(-1))
            {
                return LocalizedString.TextYesterday;
            }

            if (source.Value.Date == DateTime.Today.AddDays(1))
            {
                return LocalizedString.TextTomorrow;
            }

            var formatter = UPCRMTimeZone.Current?.EditDateFormatter;
            return formatter != null ? formatter.StringFromDate(source.Value) : source.Value.ToString("dddd, MMMM d, yyyy");
        }

        /// <summary>
        /// Returns the Localized and formatted date.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string ShortLocalizedFormattedDate(this DateTime? source)
        {
            if (source == null)
            {
                return null;
            }

            var formatter = UPCRMTimeZone.Current?.ShortDateFormatter;
            return formatter != null ? formatter.StringFromDate(source.Value) : source.Value.ToString("M/d/yy");
        }

        /// <summary>
        /// Returns the Localized and formatted date.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string LocalizedFormattedTime(this DateTime? source)
        {
            if (source == null)
            {
                return null;
            }

            var formatter = UPCRMTimeZone.Current?.StandardTimeFormatter;
            return formatter != null ? formatter.StringFromDate(source.Value) : source.Value.ToString("HH:mm");
        }

        /// <summary>
        /// Returns the Localized and formatted date and time.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string LocalizedFormattedDateTime(this DateTime? source)
        {
            return source == null ? null : $"{source.LocalizedFormattedDate()} {source.LocalizedFormattedTime()}";
        }
    }
}
