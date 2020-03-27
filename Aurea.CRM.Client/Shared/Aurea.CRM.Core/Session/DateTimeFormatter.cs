// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeFormatter.cs" company="Aurea Software Gmbh">
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
//   Date and time formatter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;

    /// <summary>
    /// Date and time formatter
    /// </summary>
    public class DateTimeFormatter
    {
        private string format;
        private TimeZoneInfo timeZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeFormatter" /> class.
        /// </summary>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="dateFormat">The format string.</param>
        public DateTimeFormatter(TimeZoneInfo timeZone, string dateFormat)
        {
            this.timeZone = timeZone;
            this.format = dateFormat;
        }

        /// <summary>
        /// Dates from string.
        /// </summary>
        /// <param name="dateString">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{T}"/>with <c>T</c> being <see cref="DateTime"/>
        /// </returns>
        public DateTime? DateFromString(string dateString)
        {
            DateTime parsedDateTime;
            DateTime.TryParse(dateString, out parsedDateTime);
            return parsedDateTime;
        }

        /// <summary>
        /// Strings from date.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringFromDate(DateTime? date)
        {
            if (date != null)
            {
                return date.Value.ToString(this.format);
            }

            return null;
        }
    }
}
