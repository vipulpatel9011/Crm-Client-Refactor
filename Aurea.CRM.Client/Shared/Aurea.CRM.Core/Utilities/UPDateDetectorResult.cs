// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPDateDetectorResult.cs" company="Aurea Software Gmbh">
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
//   Date Detector Result
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Date Detector Options
    /// </summary>
    [Flags]
    public enum UPDateDetectorOptions
    {
        /// <summary>
        /// No special options
        /// </summary>
        NoSpecialOptions = 0,

        /// <summary>
        /// Detector disabled
        /// </summary>
        DetectorDisabled = 1 << 0,

        /// <summary>
        /// Detector filter non localized dates
        /// </summary>
        DetectorFilterNonLocalizedDates = 1 << 1,

        /// <summary>
        /// Range disabled
        /// </summary>
        RangeDisabled = 1 << 2,

        /// <summary>
        /// Constants disabled
        /// </summary>
        ConstantsDisabled = 1 << 3
    }

    /// <summary>
    /// Date Detector Result
    /// </summary>
    public class UPDateDetectorResult
    {
        /// <summary>
        /// Gets or sets all dates.
        /// </summary>
        /// <value>
        /// All dates.
        /// </value>
        public List<DateTime> AllDates { get; set; }

        //public ArrayList AllDatesRanges { get; set; }

        /// <summary>
        /// Gets or sets the search string.
        /// </summary>
        /// <value>
        /// The search string.
        /// </value>
        public string SearchString { get; set; }

        /// <summary>
        /// Gets or sets the search string without results.
        /// </summary>
        /// <value>
        /// The search string without results.
        /// </value>
        public string SearchStringWithoutResults { get; set; }

        /// <summary>
        /// Gets or sets the full search string without results.
        /// </summary>
        /// <value>
        /// The full search string without results.
        /// </value>
        public string FullSearchStringWithoutResults { get; set; }

        /// <summary>
        /// Gets the maximum date.
        /// </summary>
        /// <value>
        /// The maximum date.
        /// </value>
        public DateTime? MaxDate
        {
            get
            {
                if (this.AllDates != null && this.AllDates.Count > 0)
                {
                    return this.AllDates.Max();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the minimum date.
        /// </summary>
        /// <value>
        /// The minimum date.
        /// </value>
        public DateTime? MinDate
        {
            get
            {
                if (this.AllDates != null && this.AllDates.Count > 0)
                {
                    return this.AllDates.Min();
                }

                return null;
            }
        }
    }

    /// <summary>
    /// Date Detector - Extension class
    /// </summary>
    public static class UPDateDetector
    {
        /// <summary>
        /// Detects all dates.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static UPDateDetectorResult DetectAllDates(this string source, UPDateDetectorOptions options = UPDateDetectorOptions.DetectorFilterNonLocalizedDates)
        {
            return null;
        }

        private static DateTime CalcLastDayOfMonth(DateTime date)
        {
            var newDate = new DateTime(date.Year, date.Month + 1, 1);
            return newDate.AddDays(-1);
        }

        private static List<string> CalcMonthNames()
        {
            return DateTimeFormatInfo.CurrentInfo.MonthNames.ToList();
        }

#if PORTING
        private static bool CheckDateInStringDateStyle(DateTime date, string theString, NSDateFormatterStyle dateStyle)
        {
            string dateString = NSDateFormatter.LocalizedStringFromDateDateStyleTimeStyle(date, dateStyle, NSDateFormatterNoStyle);
            return dateString == theString;
        }

        private static bool CheckDateInString(DateTime date, string theString)
        {
            return this.CheckDateInStringDateStyle(date, theString, NSDateFormatterShortStyle) ||
                   this.CheckDateInStringDateStyle(date, theString, NSDateFormatterMediumStyle) ||
                   this.CheckDateInStringDateStyle(date, theString, NSDateFormatterLongStyle);
        }
#endif
    }
}
