// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueFormatterDateTime.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Jakub Malczak
// </author>
// <summary>
//   Field value formatter date
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel.FieldValueFormatters
{
    using System;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Field Value formatter
    /// </summary>
    public class FieldValueFormatterDateTime
    {
        /// <summary>
        /// Convert date to string
        /// </summary>
        /// <param name="rawValue">value</param>
        /// <param name="options">options</param>
        /// <returns>string</returns>
        public static string ConvertDate(string rawValue, UPFormatOption options)
        {
            var date = rawValue.DateFromCrmValue();
            return options == UPFormatOption.Report ? date?.ReportFormattedDate() : date.LocalizedFormattedDate();
        }

        /// <summary>
        /// Conver time to string
        /// </summary>
        /// <param name="rawValue">value</param>
        /// <returns>string</returns>
        public static string ConvertTime(string rawValue)
        {
            DateTime? date = rawValue.TimeFromCrmValue();
            return date.LocalizedFormattedTime();
        }
    }
}
