// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka, Max Menezes
// </author>
// <summary>
//   Constants related to string manipulation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Services;
    using Aurea.CRM.Core.Session;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Org.BouncyCastle.Crypto.Digests;

    /// <summary>
    /// Constants related to string manipulation
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The up field format_ group separator.
        /// </summary>
        public const int UPFieldFormatGroupSeparator = 1;

        /// <summary>
        /// The up field format_ show 0.
        /// </summary>
        public const int UPFieldFormatShow0 = 32;
    }

    /// <summary>
    /// Implements some extension functionality against string values
    /// </summary>
    public static class StringExtensions
    {
        private const int WildcardLength = 8;

        /// <summary>
        /// Strips the HTML.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string StripHtml(this string source)
        {
            if (source == null)
            {
                return null;
            }

            var cleanHtml = Regex.Replace(source, @"<[^>]+>|&nbsp;", string.Empty).Trim();
            return Regex.Replace(cleanHtml, @"\s{2,}", " ");
        }

        /// <summary>
        /// Transform a Json object to a string dictionary.
        /// </summary>
        /// <param name="source">
        /// The source json.
        /// </param>
        /// <returns>
        /// deserialized dictionary
        /// </returns>
        public static Dictionary<string, string> JsonAsDictionary(this string source)
        {
            return JsonDictionaryFromString(source)?.ToDictionary(k => k.Key, k => (string)k.Value);
        }

        /// <summary>
        /// Transform a complex Json object string into a dictionary\array format.
        /// </summary>
        /// <param name="source">
        /// The source json.
        /// </param>
        /// <param name="deepParsing">
        /// if set to <c>true</c> [deep parsing].
        /// </param>
        /// <returns>
        /// the dictionary typed
        /// </returns>
        public static Dictionary<string, object> JsonDictionaryFromString(this string source, bool deepParsing = true)
        {
            if (source == null)
            {
                return null;
            }

            JObject jObject = null;

            try
            {
                jObject = JsonConvert.DeserializeObject<JObject>(source);
            }
            catch (Exception ex)
            {
                ServerSession.Logger.LogError(ex.Message);
            }

            return deepParsing
                ? jObject?.ParseObject<Dictionary<string, object>>()
                : jObject?.ToObject<Dictionary<string, object>>();
        }

        /// <summary>
        /// Transform a complex Json object string into a dictionary\array format.
        /// </summary>
        /// <param name="source">
        /// The source json.
        /// </param>        
        /// <returns>
        /// the dictionary typed
        /// </returns>
        public static DataModelSyncDeserializer DataSyncObjectFromString(this string source)
        {
            if (source == null)
            {
                return null;
            }

            DataModelSyncDeserializer jObject = null;

            try
            {
                jObject = JsonConvert.DeserializeObject<DataModelSyncDeserializer>(source);
            }
            catch (Exception ex)
            {
                ServerSession.Logger.LogError(ex.Message);
            }

            return jObject;
        }

        /// <summary>
        /// Parses as json.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ParseAsJson(this string source)
        {
            if (source == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<object>(source, new NativeValueConverter());
            }
            catch (Exception ex)
            {
                ServerSession.Logger.LogError(string.Concat("Illegal json ignored: ", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// Extract the InfoAreaId from a record identifier.
        /// </summary>
        /// <param name="source">
        /// The record identifier.
        /// </param>
        /// <returns>
        /// the InfoAreaId
        /// </returns>
        public static string InfoAreaId(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            var parts = source.Split('.');
            return parts[0];
        }

        /// <summary>
        /// Extracts the Record Id from a unique identifier.
        /// </summary>
        /// <param name="source">
        /// The source string.
        /// </param>
        /// <returns>
        /// the record id
        /// </returns>
        public static string RecordId(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            var parts = source.Split('.');
            return parts.Length > 1 ? parts[1] : source;
        }

        /// <summary>
        /// Check for "true" or "false" value of a given string and converts to a boolean.
        /// If for any other text, it returns the default value.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="defaultValue">
        /// if set to <c>true</c> [default value].
        /// </param>
        /// <returns>
        /// coresponding boolean value for "true" and "false" (case insensitive);
        /// default value for any other string
        /// </returns>
        public static bool ToBoolWithDefaultValue(this string source, bool defaultValue)
        {
            if (string.IsNullOrEmpty(source))
            {
                return defaultValue;
            }

            if (source.ToLower() == "true")
            {
                return true;
            }

            return source.ToLower() != "false" && defaultValue;
        }

        /// <summary>
        /// Determines whether a given string has the recordId format.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// true if satisfying the record identification pattern
        /// </returns>
        public static bool IsRecordIdentification(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            return source.Split('.').Length == 2;
        }

        /// <summary>
        /// Construct a record identification from given infoarea id and record id.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <returns>
        /// unique identifier string
        /// </returns>
        public static string InfoAreaIdRecordId(this string infoAreaId, string recordId)
        {
            if (string.IsNullOrEmpty(infoAreaId))
            {
                return string.Empty;
            }

            return string.IsNullOrWhiteSpace(recordId) ? $"{infoAreaId}." : $"{infoAreaId}.{recordId}";
        }

        /// <summary>
        /// Construct a record identification from infoarea id with linkid(optional) and record id
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// the record identifier
        /// </returns>
        public static string InfoAreaIdLinkIdFieldId(this string infoAreaId, int linkId, int fieldId)
        {
            if (string.IsNullOrEmpty(infoAreaId))
            {
                return string.Empty;
            }

            return linkId > 0 ? $"{infoAreaId}:{linkId}.{fieldId}" : $"{infoAreaId}.{fieldId}";
        }

        /// <summary>
        /// Construct a record identifier with infoAreaId and hex formated record id.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <returns>
        /// the record identifier
        /// </returns>
        public static string InfoAreaIdNumberRecordId(this string infoAreaId, long recordId)
        {
            // x%016llx
            return InfoAreaIdRecordId(infoAreaId, $"x{recordId:x16}");
        }

        /// <summary>
        /// Construct infoareaId with field id identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// the identifier
        /// </returns>
        public static string InfoAreaIdFieldId(this string infoAreaId, int fieldId)
        {
            return $"{infoAreaId}.{fieldId}";
        }

        /// <summary>
        /// Construct infoAreaId with LinkId identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// the identifier
        /// </returns>
        public static string InfoAreaIdLinkId(this string infoAreaId, int linkId)
        {
            return linkId > 0 ? $"{infoAreaId}:{linkId}" : $"{infoAreaId}";
        }

        /// <summary>
        /// Extract the FieldId from a field identifier.
        /// </summary>
        /// <param name="source">
        /// The source string.
        /// </param>
        /// <returns>
        /// the field id
        /// </returns>
        public static int FieldIdFromFieldIdentification(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return -1;
            }

            var parts = source.Split('.');
            if (parts.Length > 1)
            {
                return int.Parse(parts[1]);
            }

            return -1;
        }

        /// <summary>
        /// Extracts FieldId from a identifier provided the infoAreaId is matching.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// the field id
        /// </returns>
        public static int FieldIdFromStringWithInfoAreaId(this string source, string infoAreaId)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(infoAreaId))
            {
                return -1;
            }

            var parts = source.Split('.');
            if (parts.Length <= 1)
            {
                return -1;
            }

            return parts[0].Equals(infoAreaId) ? int.Parse(parts[1]) : -1;
        }

        /// <summary>
        /// Floats the display text from float.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FloatDisplayTextFromFloat(double value)
        {
            return value.ToString("N", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Floats the display text from float format.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FloatDisplayTextFromFloatFormat(double value, int format)
        {
            return format == Constants.UPFieldFormatGroupSeparator
                ? FloatDisplayTextFromFloat(value)
                : value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Floats the display text from float format decimal digit.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="format">The format.</param>
        /// <param name="decimalDigits">The decimal digits.</param>
        /// <returns></returns>
        public static string FloatDisplayTextFromFloatFormatDecimalDigit(double value, int format, int decimalDigits)
        {
            return format == Constants.UPFieldFormatGroupSeparator
                ? value.ToString($"N{decimalDigits}", CultureInfo.InvariantCulture)
                : value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the CRM value from date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="useFormating">To use the current formater in parsing</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string CrmValueFromDate(this DateTime date, bool useFormating = false)
        {
            var formatter = UPCRMTimeZone.Current?.CurrentDateFormatter;
            return useFormating && formatter != null ? formatter.StringFromDate(date) : date.ToString("yyyyMMdd");
        }

        /// <summary>
        /// CRMs the value from date.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="useFormating">To use the current formater in parsing</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string CrmValueFromDate(this DateTime? date, bool useFormating = false)
        {
            if (date == null)
            {
                return null;
            }

            return CrmValueFromDate(date.Value, useFormating);
        }

        /// <summary>
        /// CRMs the value from time.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="useFormating">To use the current formater in parsing</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        ///
        public static string CrmValueFromTime(this DateTime date, bool useFormating = false)
        {
            return CrmValueFromTime((DateTime?)date, useFormating);
        }

        /// <summary>
        /// CRMs the value from time.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="useFormating">To use the current formater in parsing</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        ///
        public static string CrmValueFromTime(this DateTime? date, bool useFormating = false)
        {
            if (date == null)
            {
                return null;
            }

            var formatter = UPCRMTimeZone.Current?.CurrentTimeFormatter;
            return useFormating && formatter != null ? formatter.StringFromDate(date.Value) : date.Value.ToString("HHmm");
        }

        /// <summary>
        /// Returns the value with seconds from time in CRM format.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="useFormating">To use the current formater in parsing</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        ///
        public static string CrmValueWithSecondsFromTime(this DateTime date, bool useFormating = false)
        {
            var formatter = UPCRMTimeZone.Current?.CurrentTimeSecFormatter;
            return useFormating && formatter != null ? formatter.StringFromDate(date) : date.ToString("HHmmss");
        }

        /// <summary>
        /// Returns the value from date time in CRM format.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="useFormating">To use the current formater in parsing</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        ///
        public static string CrmValueFromDateTime(this DateTime date, bool useFormating = false)
        {
            var formatter = UPCRMTimeZone.Current?.CurrentDateTimeFormatter;
            return useFormating && formatter != null ? formatter.StringFromDate(date) : date.ToString("yyyyMMdd HHmm");
        }

        /// <summary>
        /// Dates from CRM value.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime? DateFromCrmValue(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            if (source.Length == 4)
            {
                if (source.Contains(":"))
                {
                    source = "0000";
                }

                var baseDate = DateExtensions.BaseDate;
                return new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, Convert.ToInt32(source.Substring(0, 2)), Convert.ToInt32(source.Substring(2, 2)), 0);
            }

            DateTime formattedDate;

            if (source.Length == 8)
            {
                if (source.Contains("/"))
                {
                    source = DateTime.Today.ToString("yyyyMMdd");
                }

                DateTime.TryParseExact(source, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out formattedDate);
                return formattedDate;
            }

            if (source.Length == 13)
            {
                DateTime.TryParseExact(source, "yyyyMMdd HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out formattedDate);
                return formattedDate;
            }

            if (source.Length == 14)
            {
                DateTime.TryParseExact(source, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out formattedDate);
                return formattedDate;
            }

            if (source.Length == 16)
            {
                DateTime.TryParseExact(source, "yyyyMMdd h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out formattedDate);
                return formattedDate;
            }

            if (source.Length == 17)
            {
                DateTime.TryParseExact(source, "yyyyMMdd hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out formattedDate);
                return formattedDate;
            }

            DateTime parsedDate;

            var retDate = DateTime.TryParse(source, out parsedDate) ? parsedDate : (DateTime?)null;
#if PORTING
            retDate = DateByString(source).ObjectForKey(this);
            if (!retDate)
            {
                retDate = UPCRMTimeZone.CurrentDateFormatter().DateFromString(this);
                NSString.DateByString().SetValueForKey(retDate, this);
            }
#endif
            return retDate;
        }

        /// <summary>
        /// CRMs the value from bool.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns></returns>
        public static string CrmValueFromBool(bool value)
        {
            return value ? "true" : "false";
        }

#if PORTING

        public static NSMutableDictionary DateByString(this string source)
        {
            var dateByString = null;
            if (dateByString == null || dateByString.len >= 1000)
            {
                dateByString = new NSMutableDictionary();
            }

            return dateByString;
        }
#endif

        /// <summary>
        /// Integers the display text from integer.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string IntegerDisplayTextFromInteger(int value)
        {
            return $"{value:D}";
        }

#if PORTING
        NSNumber NumberFromDisplayValue()
        {
            return NSString.FloatFormatter().NumberFromString(this);
        }

        float DisplayFloatValue()
        {
            return NumberFromDisplayValue().FloatValue;
        }

        NSNumberFormatter GpsFractionFormatter()
        {
            static
                dispatch_once_t once;
            static
            NSNumberFormatter gpsFractionFormatter;
            dispatch_once(once, delegate ()
            {
                gpsFractionFormatter = NSNumberFormatter.TheNew();
                gpsFractionFormatter.SetNumberStyle(NSNumberFormatterDecimalStyle);
                gpsFractionFormatter.SetFormatterBehavior(NSNumberFormatterBehaviorDefault);
                gpsFractionFormatter.SetDecimalSeparator(".");
                gpsFractionFormatter.SetUsesGroupingSeparator(false);
            });
            return gpsFractionFormatter;
        }
        NSNumber GpsLongtitudeNumberValue(this string str)
        {
            NSNumber myNumber = GpsFractionFormatter().NumberFromString(str);
            if (myNumber.DoubleValue <= 180 && myNumber.DoubleValue > -180)
            {
                return myNumber;
            }
            else
            {
                return null;
            }

        }

        NSNumber GpsLatitudeNumberValue()
        {
            NSNumber myNumber = GpsFractionFormatter().NumberFromString(this);
            if (myNumber.DoubleValue <= 90 && myNumber.DoubleValue > -90)
            {
                return myNumber;
            }
            else
            {
                return null;
            }

        }

        
#endif

        /// <summary>
        /// Dates from string.
        /// </summary>
        /// <param name="dateString">
        /// The date string Ex: 20160624.
        /// </param>
        /// <param name="timeString">
        /// The date string Ex: 1930.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime DateFromStrings(string dateString, string timeString)
        {
            if (string.IsNullOrEmpty(dateString) || dateString.Length < 8)
            {
                return DateTime.MinValue;
            }

            if (dateString.Contains("/"))
            {
                return DateTime.Today;
            }

            int year = Convert.ToInt32(dateString.Substring(0, 4));
            int month = Convert.ToInt32(dateString.Substring(4, 2));
            int day = Convert.ToInt32(dateString.Substring(6, 2));

            if (!string.IsNullOrEmpty(timeString) && timeString.Length == 4 && !timeString.Contains("/"))
            {
                return new DateTime(
                    year,
                    month,
                    day,
                    Convert.ToInt32(timeString.Substring(0, 2)),
                    Convert.ToInt32(timeString.Substring(2, 2)),
                    0);
            }

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Times from CRM value.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime TimeFromCrmValue(this string source)
        {
            DateTime result;
            var success = DateTime.TryParseExact(source, "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

            if (!success)
            {
                DateTime.TryParseExact(source, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }

            return result;
        }

        /// <summary>
        /// Dates the time from CRM value.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime? DateTimeFromCrmValue(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            DateTime parsedDateTime;
            DateTime.TryParseExact(source, "yyyyMMdd HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);
            if (parsedDateTime == DateTime.MinValue)
            {
                parsedDateTime = UPCRMTimeZone.Current?.CurrentDateTimeFormatter != null
                    ? (DateTime)UPCRMTimeZone.Current.CurrentDateTimeFormatter.DateFromString(source)
                    : DateTime.MinValue;
            }

            return parsedDateTime;
        }

        /// <summary>
        /// Determines whether [is not localized].
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsNotLocalized(this string source)
        {
            return source == "<Not localized>";
        }

        /// <summary>
        /// Dates from string.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime DateFromString(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return DateTime.UtcNow;
            }

            if (source.Length == 10)
            {
                return DateTime.Parse($"{source.Substring(0, 4)}-{source.Substring(4, 2)}-{source.Substring(6, 2)}");
            }

            return DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the Year from CRM date.
        /// </summary>
        /// <param name="crmDate">The CRM date.</param>
        /// <returns></returns>
        public static int YearFromCrmDate(this string crmDate)
        {
            if (crmDate != null && crmDate.Length == 10)
            {
                return Convert.ToInt32(crmDate.Substring(0, 4));
            }

            return 0;
        }

        /// <summary>
        /// Gets the Month from CRM date.
        /// </summary>
        /// <param name="crmDate">The CRM date.</param>
        /// <returns></returns>
        public static int MonthFromCrmDate(this string crmDate)
        {
            if (crmDate != null && crmDate.Length == 10)
            {
                return Convert.ToInt32(crmDate.Substring(4, 2));
            }

            return 0;
        }

        /// <summary>
        /// Gets the Day from CRM date.
        /// </summary>
        /// <param name="crmDate">The CRM date.</param>
        /// <returns></returns>
        public static int DayFromCrmDate(this string crmDate)
        {
            if (crmDate != null && crmDate.Length == 10)
            {
                return Convert.ToInt32(crmDate.Substring(6, 2));
            }

            return 0;
        }

        /// <summary>
        /// The document key to record identification.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DocumentKeyToRecordIdentification(this string str)
        {
            if (str.StartsWith("new:"))
            {
                return str.Substring(4);
            }

            if (str.StartsWith("A"))
            {
                string d1RecordId = str.DocumentKeyToD1RecordId();
                if (!string.IsNullOrEmpty(d1RecordId))
                {
                    return $"D1.{d1RecordId}";
                }
            }
            else if (str.StartsWith("K"))
            {
                string d2RecordId = str.DocumentKeyToD2RecordId();
                if (!string.IsNullOrEmpty(d2RecordId))
                {
                    return $"D2.{d2RecordId}";
                }
            }

            return null;
        }

        /// <summary>
        /// The document key to d 1 record id.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DocumentKeyToD1RecordId(this string str)
        {
            return !str.StartsWith("A") ? null : str.DocumentKeyToRecordId();
        }

        /// <summary>
        /// The document key to d 2 record id.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DocumentKeyToD2RecordId(this string str)
        {
            return !str.StartsWith("K") ? null : str.DocumentKeyToRecordId();
        }

        /// <summary>
        /// The document key to record id. (Ported from NSString+UPCRMRecord.m)
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DocumentKeyToRecordId(this string str)
        {
            int index = str.IndexOf("-", StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                return null;
            }

            var firstPart = str.Substring(1, index - 1).ToInt();
            var secondPart = str.Substring(index + 1).ToInt();

            return $"x{firstPart.ToString("X8")}{secondPart.ToString("X8")}";
        }

        /// <summary>
        /// Parses hour from crm time string
        /// </summary>
        /// <param name="str">Time string</param>
        /// <returns>Hour as <see cref="int"/></returns>
        public static int HourFromCrmTime(this string str)
        {
            if (str?.Length == 5 || str?.Length == 4)
            {
                return str.Substring(0, 2).ToInt();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Parses minute from crm time string
        /// </summary>
        /// <param name="str">Time string</param>
        /// <returns>Minute as <see cref="int"/></returns>
        public static int MinuteFromCrmTime(this string str)
        {
            if (str?.Length == 5)
            {
                return str.Substring(3, 2).ToInt();
            }
            else if (str?.Length == 4)
            {
                return str.Substring(2, 2).ToInt();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Used to replace CRM localization string tokens with dotnet tokens
        /// </summary>
        /// <param name="localizationString">String to convert</param>
        /// <returns>Localization <see cref="string"/> string</returns>
        public static string ToDotnetStringFormatTemplate(this string localizationString)
        {
            if (string.IsNullOrWhiteSpace(localizationString))
            {
                return localizationString;
            }

            var stringTemplate = new StringBuilder();

            var tokens = new Dictionary<string, string>
            {
                { "@", "{index}" }
            };

            var templateTokenIndex = 0;

            for (var currentIndex = 0; currentIndex < localizationString.Length; currentIndex++)
            {
                var tokenCharacter = localizationString[currentIndex];

                if (tokenCharacter == '%')
                {
                    var token = localizationString.Substring(currentIndex + 1, 1);
                    if (localizationString.Length > currentIndex + 1 && tokens.ContainsKey(token))
                    {
                        stringTemplate.Append(tokens[token].Replace("index", templateTokenIndex.ToString()));

                        currentIndex++;
                        templateTokenIndex++;
                    }
                }
                else
                {
                    stringTemplate.Append(tokenCharacter);
                }
            }

            return stringTemplate.ToString();
        }

#if PORTING

        NSDate DateFromCrmValueWithCrmTime(string crmTime)
        {
            if (crmTime.Length)
            {
                return NSString.StringWithFormat("%@ %@", this, crmTime).DateTimeFromCrmValue();
            }
            else
            {
                return DateFromCrmValue();
            }

        }

        object JsonFromString()
        {
            if (Length())
            {
                NSError error;
                object idOption =
                    NSJSONSerialization.JSONObjectWithDataOptionsError(DataUsingEncoding(NSUTF8StringEncoding), 0, 
                        error);
                if (error == null && idOption != null)
                {
                    return idOption;
                }

                DDLogWarn("illegal json ignored: %@", this);
                return null;
            }
            else
            {
                return null;
            }

        }

        NSDictionary JsonDictionaryFromString()
        {
            object idOption = JsonFromString();
            if (idOption != null && idOption.IsKindOfClass(typeof (NSDictionary)))
            {
                return (NSDictionary) idOption;
            }
            else
            {
                return NSDictionary.Dictionary();
            }

        }

        List<object> JsonArrayFromString()
        {
            object idOption = JsonFromString();
            if (idOption != null && idOption.IsKindOfClass(typeof (List<object>)))
            {
                return (List<object>) idOption;
            }
            else
            {
                DDLogWarn("illegal json ignored: %@", this);
                return new List<object>();
            }

        }

        static NSNumberFormatter FloatFormatterNoGrouping()
        {
        static
            dispatch_once_t once;
            static
            NSNumberFormatter floatFormatterNoGrouping;
            dispatch_once(once, delegate()
            {
            });
            if (floatFormatterNoGrouping == null)
            {
                floatFormatterNoGrouping = NSNumberFormatter.TheNew();
                floatFormatterNoGrouping.SetUsesGroupingSeparator(false);
                floatFormatterNoGrouping.SetMinimumIntegerDigits(1);
                floatFormatterNoGrouping.SetMinimumFractionDigits(2);
                floatFormatterNoGrouping.SetMaximumFractionDigits(2);
            }

            return floatFormatterNoGrouping;
        }

        static NSNumberFormatter FloatFormatter(int decimaDigits)
        {
            if (decimaDigits == 0)
            {
            static
                dispatch_once_t once01;
                static
                NSNumberFormatter noDecimalFloatFormatter;
                dispatch_once(once01, delegate()
                {
                    noDecimalFloatFormatter = NSNumberFormatter.TheNew();
                    noDecimalFloatFormatter.SetUsesGroupingSeparator(true);
                    noDecimalFloatFormatter.SetGroupingSize(3);
                    noDecimalFloatFormatter.SetMinimumIntegerDigits(1);
                    noDecimalFloatFormatter.SetMinimumFractionDigits(decimaDigits);
                    noDecimalFloatFormatter.SetMaximumFractionDigits(decimaDigits);
                });
                return noDecimalFloatFormatter;
            }
            else if (decimaDigits == 1)
            {
            static
                dispatch_once_t once02;
                static
                NSNumberFormatter oneDecimalFloatFormatter;
                dispatch_once(once02, delegate()
                {
                    oneDecimalFloatFormatter = NSNumberFormatter.TheNew();
                    oneDecimalFloatFormatter.SetUsesGroupingSeparator(true);
                    oneDecimalFloatFormatter.SetGroupingSize(3);
                    oneDecimalFloatFormatter.SetMinimumIntegerDigits(1);
                    oneDecimalFloatFormatter.SetMinimumFractionDigits(decimaDigits);
                    oneDecimalFloatFormatter.SetMaximumFractionDigits(decimaDigits);
                });
                return oneDecimalFloatFormatter;
            }
            else if (decimaDigits == 3)
            {
            static
                dispatch_once_t once03;
                static
                NSNumberFormatter threeDecimalFloatFormatter;
                dispatch_once(once03, delegate()
                {
                    threeDecimalFloatFormatter = NSNumberFormatter.TheNew();
                    threeDecimalFloatFormatter.SetUsesGroupingSeparator(true);
                    threeDecimalFloatFormatter.SetGroupingSize(3);
                    threeDecimalFloatFormatter.SetMinimumIntegerDigits(1);
                    threeDecimalFloatFormatter.SetMinimumFractionDigits(decimaDigits);
                    threeDecimalFloatFormatter.SetMaximumFractionDigits(decimaDigits);
                });
                return threeDecimalFloatFormatter;
            }
            else if (decimaDigits == 4)
            {
            static
                dispatch_once_t once04;
                static
                NSNumberFormatter fourDecimalFloatFormatter;
                dispatch_once(once04, delegate()
                {
                    fourDecimalFloatFormatter = NSNumberFormatter.TheNew();
                    fourDecimalFloatFormatter.SetUsesGroupingSeparator(true);
                    fourDecimalFloatFormatter.SetGroupingSize(3);
                    fourDecimalFloatFormatter.SetMinimumIntegerDigits(1);
                    fourDecimalFloatFormatter.SetMinimumFractionDigits(decimaDigits);
                    fourDecimalFloatFormatter.SetMaximumFractionDigits(decimaDigits);
                });
                return fourDecimalFloatFormatter;
            }
            else
            {
            static
                dispatch_once_t once02;
                static
                NSNumberFormatter twoDecimalFloatFormatter;
                dispatch_once(once02, delegate()
                {
                    twoDecimalFloatFormatter = NSNumberFormatter.TheNew();
                    twoDecimalFloatFormatter.SetUsesGroupingSeparator(true);
                    twoDecimalFloatFormatter.SetGroupingSize(3);
                    twoDecimalFloatFormatter.SetMinimumIntegerDigits(1);
                    twoDecimalFloatFormatter.SetMinimumFractionDigits(2);
                    twoDecimalFloatFormatter.SetMaximumFractionDigits(2);
                });
                return twoDecimalFloatFormatter;
            }

        }

        static NSNumberFormatter FloatFormatterNoGrouping(int decimaDigits)
        {
            if (decimaDigits == 0)
            {
            static
                dispatch_once_t once11;
                static
                NSNumberFormatter noDecimalNoGroupFloatFormatter;
                dispatch_once(once11, delegate()
                {
                    noDecimalNoGroupFloatFormatter = NSNumberFormatter.TheNew();
                    noDecimalNoGroupFloatFormatter.SetUsesGroupingSeparator(false);
                    noDecimalNoGroupFloatFormatter.SetMinimumIntegerDigits(1);
                    noDecimalNoGroupFloatFormatter.SetMinimumFractionDigits(decimaDigits);
                    noDecimalNoGroupFloatFormatter.SetMaximumFractionDigits(decimaDigits);
                });
                return noDecimalNoGroupFloatFormatter;
            }
            else if (decimaDigits == 1)
            {
            static
                dispatch_once_t once12;
                static
                NSNumberFormatter oneDecimalNoGroupFloatFormatter;
                dispatch_once(once12, delegate()
                {
                    oneDecimalNoGroupFloatFormatter = NSNumberFormatter.TheNew();
                    oneDecimalNoGroupFloatFormatter.SetUsesGroupingSeparator(false);
                    oneDecimalNoGroupFloatFormatter.SetMinimumIntegerDigits(1);
                    oneDecimalNoGroupFloatFormatter.SetMinimumFractionDigits(decimaDigits);
                    oneDecimalNoGroupFloatFormatter.SetMaximumFractionDigits(decimaDigits);
                });
                return oneDecimalNoGroupFloatFormatter;
            }
            else if (decimaDigits == 3)
            {
            static
                dispatch_once_t once13;
                static
                NSNumberFormatter threeDecimalNoGroupFloatFormatter;
                dispatch_once(once13, delegate()
                {
                    threeDecimalNoGroupFloatFormatter = NSNumberFormatter.TheNew();
                    threeDecimalNoGroupFloatFormatter.SetUsesGroupingSeparator(false);
                    threeDecimalNoGroupFloatFormatter.SetMinimumIntegerDigits(1);
                    threeDecimalNoGroupFloatFormatter.SetMinimumFractionDigits(decimaDigits);
                    threeDecimalNoGroupFloatFormatter.SetMaximumFractionDigits(decimaDigits);
                });
                return threeDecimalNoGroupFloatFormatter;
            }
            else if (decimaDigits == 4)
            {
            static
                dispatch_once_t once14;
                static
                NSNumberFormatter fourDecimalNoGroupFloatFormatter;
                dispatch_once(once14, delegate()
                {
                    fourDecimalNoGroupFloatFormatter = NSNumberFormatter.TheNew();
                    fourDecimalNoGroupFloatFormatter.SetUsesGroupingSeparator(false);
                    fourDecimalNoGroupFloatFormatter.SetMinimumIntegerDigits(1);
                    fourDecimalNoGroupFloatFormatter.SetMinimumFractionDigits(decimaDigits);
                    fourDecimalNoGroupFloatFormatter.SetMaximumFractionDigits(decimaDigits);
                });
                return fourDecimalNoGroupFloatFormatter;
            }
            else
            {
            static
                dispatch_once_t once12;
                static
                NSNumberFormatter twoDecimalNoGroupFloatFormatter;
                dispatch_once(once12, delegate()
                {
                    twoDecimalNoGroupFloatFormatter = NSNumberFormatter.TheNew();
                    twoDecimalNoGroupFloatFormatter.SetUsesGroupingSeparator(false);
                    twoDecimalNoGroupFloatFormatter.SetMinimumIntegerDigits(1);
                    twoDecimalNoGroupFloatFormatter.SetMinimumFractionDigits(2);
                    twoDecimalNoGroupFloatFormatter.SetMaximumFractionDigits(2);
                });
                return twoDecimalNoGroupFloatFormatter;
            }

        }

        static NSNumberFormatter NumberFormatter()
        {
        static
            dispatch_once_t once;
            static
            NSNumberFormatter numberFormatter;
            dispatch_once(once, delegate()
            {
                numberFormatter = NSNumberFormatter.TheNew();
                numberFormatter.SetUsesGroupingSeparator(true);
                numberFormatter.SetGroupingSize(3);
                numberFormatter.SetMinimumIntegerDigits(1);
                numberFormatter.SetMinimumFractionDigits(0);
                numberFormatter.SetMaximumFractionDigits(0);
            });
            return numberFormatter;
        }

        static NSNumberFormatter NumberFormatterNoGrouping()
        {
        static
            dispatch_once_t once;
            static
            NSNumberFormatter numberFormatterNoGrouping;
            dispatch_once(once, delegate()
            {
                numberFormatterNoGrouping = NSNumberFormatter.TheNew();
                numberFormatterNoGrouping.SetUsesGroupingSeparator(false);
                numberFormatterNoGrouping.SetMinimumIntegerDigits(1);
                numberFormatterNoGrouping.SetMinimumFractionDigits(0);
                numberFormatterNoGrouping.SetMaximumFractionDigits(0);
            });
            return numberFormatterNoGrouping;
        }

        string FloatDisplayTextWithFormat(int format)
        {
            return NSString.FloatDisplayTextFromFloatFormat(DoubleValue(), format);
        }

        string FloatDisplayText()
        {
            return NSString.FloatDisplayTextFromFloat(DoubleValue());
        }

        static string IntegerDisplayTextFromIntegerFormat(int value, int format)
        {
            if (format & Constants.UPFieldFormat_GroupSeparator)
            {
                return NumberFormatter().StringFromNumber(NSNumber.NumberWithInteger(value));
            }
            else
            {
                return NumberFormatterNoGrouping().StringFromNumber(NSNumber.NumberWithInteger(value));
            }

        }

        // - (NSString *) documentD1RecordIdToKey {
        // if (![self StartsWith:@"x"] || self.length != 17) {
        // return nil;
        // }
        // NSRange firstPart = NSMakeRange(1, 8);
        // NSRange secondPart = NSMakeRange(9, 8);
        // return [NSString stringWithFormat:@"A%ld-%ld", (long)[[self substringWithRange:firstPart] intFromHex], (long)[[self substringWithRange:secondPart] intFromHex]];
        // }
        // - (NSString *) documentD2RecordIdToKey {
        // if (![self StartsWith:@"x"] || self.length != 17) {
        // return nil;
        // }
        // NSRange firstPart = NSMakeRange(1, 8);
        // NSRange secondPart = NSMakeRange(9, 8);
        // return [NSString stringWithFormat:@"K%ld-%ld", (long)[[self substringWithRange:firstPart] intFromHex], (long)[[self substringWithRange:secondPart] intFromHex]];
        // }
        string DocumentKeyToRecordIdentification()
        {
            if (StartsWith("new:"))
            {
                return SubstringFromIndex(4);
            }
            else if (StartsWith("A"))
            {
                string d1RecordId = DocumentKeyToD1RecordId();
                if (d1RecordId.Length > 0)
                {
                    return NSString.StringWithFormat("D1.%@", d1RecordId);
                }

            }
            else if (StartsWith("K"))
            {
                string d2RecordId = DocumentKeyToD2RecordId();
                if (d2RecordId.Length > 0)
                {
                    return NSString.StringWithFormat("D2.%@", d2RecordId);
                }

            }

            return null;
        }

        // - (NSString *) documentRecordIdentificationToKey {
        // if ([self StartsWith:@"D1."]) {
        // return [[self substringFromIndex:3] documentD1RecordIdToKey];
        // }
        // if ([self StartsWith:@"D2."]) {
        // return [[self substringFromIndex:3] documentD2RecordIdToKey];
        // }
        // return nil;
        // }

#endif

        /// <summary>
        /// Stat No from record identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int StatNoFromRecordId(this string value)
        {
            if (!value.StartsWith("x") || value.Length != 17)
            {
                return -1;
            }

            return Convert.ToInt32(value.Substring(1, 8), 16);
        }

        /// <summary>
        /// Record No from record identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int RecordNoFromRecordId(this string value)
        {
            if (value == null || !value.StartsWith("x") || value.Length != 17)
            {
                return -1;
            }

            return Convert.ToInt32(value.Substring(9, 8), 16);
        }

        /// <summary>
        /// Stat No from record identifier string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string StatNoFromRecordIdString(this string value)
        {
            int no = value?.StatNoFromRecordId() ?? 0;
            return no >= 0 ? no.ToString() : string.Empty;
        }

        /// <summary>
        /// Record No from record identifier string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string RecordNoFromRecordIdString(this string value)
        {
            int no = value.RecordNoFromRecordId();
            return no >= 0 ? no.ToString() : string.Empty;
        }

        /// <summary>
        /// String from object.
        /// </summary>
        /// <param name="theObject">The object.</param>
        /// <returns></returns>
        public static string StringFromObject(object theObject)
        {
            string json = JsonConvert.SerializeObject(theObject);
            return json;
        }

        /// <summary>
        /// Reps the identifier.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int RepId(this string source)
        {
            int intValue;
            if (!string.IsNullOrEmpty(source) && source.Length == 9 && source.StartsWith("U"))
            {
                var repString = $"10{source.Substring(1)}";
                return int.TryParse(repString, out intValue) ? intValue : 0;
            }

            return int.TryParse(source, out intValue) ? intValue : 0;
        }

        /// <summary>
        /// Reps the identifier string.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RepIdString(this string source)
        {
            if (!string.IsNullOrEmpty(source) && source.Length < 9)
            {
                return NineDigitStringFromRep(source.RepId());
            }

            return source;
        }

        /// <summary>
        /// Nines the digit string from rep.
        /// </summary>
        /// <param name="rep">
        /// The rep.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string NineDigitStringFromRep(int rep)
        {
            if (rep == 0)
            {
                return string.Empty;
            }

            return rep >= 1000000000 ? $"U{rep - 1000000000:D8}" : $"{rep:D9}";
        }

        /// <summary>
        /// Determines whether [is wildcard date].
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsWildcardDate(this string source)
        {
            if (source.Length < 9 || !source.StartsWith("$"))
            {
                return false;
            }

            for (var i = 1; i < 9; i++)
            {
                if ("?#0123456789".IndexOf(source.Substring(i, 1)) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Replaces the date variables.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReplaceDateVariables(this string source)
        {
            if (source.StartsWith("$cur"))
            {
                return source.StartsWith("$curTime")
                           ? source.ReplaceTimeVariables()
                           : source.ReplaceDatePostfixWithDateReplacePrefixLength(DateTime.UtcNow, 4);
            }

            return source.IsWildcardDate()
                       ? source.ReplaceDatePostfixWithDateReplacePrefixLength(DateTime.UtcNow, 0)
                       : source;
        }

        /// <summary>
        /// Replaces the length of the date postfix with date replace prefix.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="prefixLength">
        /// Length of the prefix.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReplaceDatePostfixWithDateReplacePrefixLength(this string source, DateTime date, int prefixLength)
        {
            string wildcardDate = null;
            var dateComponents = date;

            var resultLength = 0;
            var nextPosition = 0;
            var varPart = source.Substring(prefixLength);
            if (!ExtractDateComponents(source, prefixLength, ref wildcardDate, ref dateComponents, ref resultLength, ref nextPosition, varPart))
            {
                return source;
            }

            date = dateComponents;
            date = PostfixExtractAndAddDateComponents(source, date, ref nextPosition, true);

            var resultDateString = UPCRMTimeZone.Current.GetAdjustedClientDataMMDate(date);
            if (string.IsNullOrWhiteSpace(wildcardDate))
            {
                return resultLength > 0 ? resultDateString.Substring(0, resultLength) : resultDateString;
            }

            var resultDateBuilder = new StringBuilder();
            var wildcardCopyRangeLength = 0;
            var resultCopyRangeLength = 0;
            var resultCopyRangeLocation = 0;
            var wildcardCopyRangeLocation = 0;
            ExtractDateWildCardParameters(wildcardDate, resultDateString, resultDateBuilder, ref wildcardCopyRangeLength, ref resultCopyRangeLength, ref resultCopyRangeLocation, ref wildcardCopyRangeLocation);

            if (resultCopyRangeLength > 0)
            {
                resultDateBuilder.Append(resultDateString.Substring(resultCopyRangeLocation, resultCopyRangeLength));
            }
            else if (wildcardCopyRangeLength > 0)
            {
                resultDateBuilder.Append(wildcardDate.Substring(wildcardCopyRangeLocation, wildcardCopyRangeLength));
            }

            return resultDateBuilder.ToString();
        }

        /// <summary>
        /// Replaces the time variables.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReplaceTimeVariables(this string source)
        {
            return source.StartsWith("$cur")
                       ? source.ReplaceTimePostfixWithDateReplacePrefixLength(DateTime.UtcNow, 4)
                       : source;
        }

        /// <summary>
        /// Replaces the length of the time postfix with date replace prefix.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="prefixLength">
        /// Length of the prefix.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReplaceTimePostfixWithDateReplacePrefixLength(this string source, DateTime date, int prefixLength)
        {
            var seconds = false;
            var varPart = source.Substring(prefixLength);
            int nextPosition;
            if (varPart.StartsWith("Time"))
            {
                if (varPart.StartsWith("TimeSec"))
                {
                    seconds = true;
                    nextPosition = prefixLength + 7;
                }
                else
                {
                    nextPosition = prefixLength + 4;
                }
            }
            else if (varPart.StartsWith("Hour"))
            {
                nextPosition = prefixLength + 4;
                date = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
            }
            else
            {
                return source;
            }

            date = PostfixExtractAndAddDateComponents(source, date, ref nextPosition, false);

            if (seconds)
            {
                return UPCRMTimeZone.Current.ClientDataTimeSecFormatter.StringFromDate(date);
            }

            return UPCRMTimeZone.Current.GetAdjustedClientDataMMTime(date);
        }

        /// <summary>
        /// Strings the by replacing occurrences of parameter with index with string.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="nr">
        /// The nr.
        /// </param>
        /// <param name="replaceString">
        /// The replace string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string StringByReplacingOccurrencesOfParameterWithIndexWithString(this string source, int nr, string replaceString)
        {
            if (source == null)
            {
                return null;
            }

            var pattern = $"{{{nr + 1}}}";
            if (source.Contains(pattern))
            {
                return source.Replace(pattern, replaceString);
            }

            pattern = $"%%{nr + 1}";
            return source.Replace(pattern, replaceString);
        }

        /// <summary>
        /// Strings the by replacing occurrences of parameter with raw index with string.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="nr">
        /// The nr.
        /// </param>
        /// <param name="replaceString">
        /// The replace string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string StringByReplacingOccurrencesOfParameterWithRawIndexWithString(this string source, int nr, string replaceString)
        {
            var pattern = "{" + $"r{nr + 1}" + "}";
            if (source.IndexOf(pattern) > 0)
            {
                return source.Replace(pattern, replaceString);
            }

            pattern = $"%%r{nr + 1}";
            return source.Replace(pattern, replaceString);
        }

        /// <summary>
        /// Gets the multi line characters.
        /// </summary>
        /// <value>
        /// The multi line characters.
        /// </value>
        public static string MultiLineCharacters => "\r\n";

        /// <summary>
        /// Singles the line string.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string SingleLineString(this string source)
        {
            if (source.IndexOf(MultiLineCharacters) < 0)
            {
                return source;
            }

            var s = source.Replace(MultiLineCharacters, " ");
            s = s.Replace("\n", " ");
            return s;
        }

        /// <summary>
        /// Determines whether this instance is int.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsInt(this string source)
        {
            int test;
            return int.TryParse(source, out test);
        }

        /// <summary>
        /// Referenceds the string with default.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="defaultString">
        /// The default string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReferencedStringWithDefault(this string source, string defaultString)
        {
            if (string.IsNullOrEmpty(source))
            {
                return defaultString;
            }

            var parts = source.Split(':');
#if PORTING
            if (parts.Length == 2)
            {
                string ls = UPLocalizedString(parts[0], parts[1].IntegerValue());
                if (ls.IsNotLocalized())
                {
                    return defaultString != null ? defaultString : ls;
                }

                return ls;
            }
#endif

            return defaultString;
        }

        /// <summary>
        /// Referenceds the string.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReferencedString(this string source)
        {
            return source.ReferencedStringWithDefault(null);
        }

        /// <summary>
        /// Strings from reference.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string StringFromReference(string reference)
        {
            return reference.ReferencedString();
        }

        /// <summary>
        /// Valids the name of the file.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ValidFileName(this string source)
        {
            var invalidFileNameCharacters = "/\\?%*|\"<>".ToCharArray();

            var fileNameParts = source.Split(invalidFileNameCharacters);
            return string.Join("_", fileNameParts);
        }

        /// <summary>
        /// Records the identifier from stat no record no.
        /// </summary>
        /// <param name="statNo">
        /// The stat no.
        /// </param>
        /// <param name="recordNo">
        /// The record no.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RecordIdFromStatNoRecordNo(long statNo, long recordNo)
        {
            // return NSString.StringWithFormat("x%08lx%08lx", (long) statNo, (long) recordNo);
            return $"x{statNo:D8}{recordNo:D8}";
        }

        /// <summary>
        /// Records the identifier from stat no string record no string.
        /// </summary>
        /// <param name="statNoString">
        /// The stat no string.
        /// </param>
        /// <param name="recordNoString">
        /// The record no string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RecordIdFromStatNoStringRecordNoString(string statNoString, string recordNoString)
        {
            return RecordIdFromStatNoRecordNo(int.Parse(statNoString), int.Parse(recordNoString));
        }

        /// <summary>
        /// Compares two strings and returns if str1 contains str2
        /// </summary>
        /// <param name="str1">
        /// First String
        /// </param>
        /// <param name="str2">
        /// Second String
        /// </param>
        /// <param name="diacriticSensitive">
        /// Forces diacritic search (a.k.a. Accent search)
        /// </param>
        /// <param name="caseSensitive">
        /// Forces case sensitive search
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool CompareWithOptions(
            this string str1,
            string str2,
            bool diacriticSensitive = false,
            bool caseSensitive = false)
        {
            CompareInfo ci = new CultureInfo("en-US").CompareInfo;
            CompareOptions co = (caseSensitive ? CompareOptions.IgnoreCase : CompareOptions.None)
                                | (diacriticSensitive ? CompareOptions.IgnoreNonSpace : CompareOptions.None);
            return ci.IndexOf(str1, str2, co) != -1;
        }

        /// <summary>
        /// The up classic layout enabled.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool UpClassicLayoutEnabled(this string str)
        {
            string classicLayoutConfig = ConfigurationUnitStore.DefaultStore.ConfigValue("List.ClassicLayout");

            if (!string.IsNullOrEmpty(classicLayoutConfig))
            {
                if (classicLayoutConfig == "1" || classicLayoutConfig.ToLower() == "true")
                {
                    return true;
                }

                if (classicLayoutConfig == "0" || classicLayoutConfig.ToLower() == "false")
                {
                    return false;
                }

                Dictionary<string, object> classicLayoutDictionary = classicLayoutConfig.JsonDictionaryFromString();
                if (classicLayoutDictionary != null && classicLayoutDictionary.Count > 0)
                {
                    bool defaultValue = false;
                    string defaultString = classicLayoutDictionary["default"] as string;
                    if (!string.IsNullOrEmpty(defaultString)
                        && (defaultString == "1" || defaultString.CompareWithOptions("true")))
                    {
                        defaultValue = true;
                    }

                    List<string> excludes = classicLayoutDictionary["excludes"] as List<string>;
                    if (excludes != null)
                    {
                        if (excludes.Contains(str))
                        {
                            return !defaultValue;
                        }
                    }

                    bool includesExists = false;
                    List<string> includes = classicLayoutDictionary["includes"] as List<string>;
                    if (includes != null)
                    {
                        includesExists = true;
                        if (includes.Contains(str))
                        {
                            return defaultValue;
                        }
                    }

                    return includesExists ? !defaultValue : defaultValue;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates hashed Md5 string by given string.
        /// </summary>
        /// <param name="str">String to be hashed</param>
        /// <returns><see cref="string"/></returns>
        public static string Md5String(this string str)
        {
            var dig = new MD5Digest();

            var msgBytes = Encoding.UTF8.GetBytes(str);
            dig.BlockUpdate(msgBytes, 0, msgBytes.Length);
            var result = new byte[dig.GetDigestSize()];
            dig.DoFinal(result, 0);

            return System.BitConverter.ToString(result).Replace("-", string.Empty);
        }

        /// <summary>
        /// To parse ; seperetd string to list
        /// </summary>
        /// <param name="str">String to be parsed</param>
        /// <returns>The list ouput</returns>
        public static List<string> ParseToList(this string str)
        {
            return str.Split(';')?.ToList();
        }

        /// <summary>
        /// Check if the given string is a font icon file name.
        /// A font icon name contains a '\' followed by a hexa number,
        /// Checking the same logic here
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsFontIcon(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            return int.TryParse(str.Remove(0, 1), System.Globalization.NumberStyles.HexNumber, null, out int iconFileName);
        }

        /// <summary>
        /// Check if the given string contains international characters.
        /// </summary>
        /// <param name="input">The input</param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ContainsInternationalCharacter(this string input)
        {
            string[] foreignCharacters =
            {
                "äæǽ", "öœ", "ü", "Ä", "Ü", "Ö", "ÀÁÂÃÄÅǺĀĂĄǍΑΆẢẠẦẪẨẬẰẮẴẲẶА", "àáâãåǻāăąǎªαάảạầấẫẩậằắẵẳặа", "Б", "б",
                "ÇĆĈĊČ", "çćĉċč", "Д", "д", "ÐĎĐΔ", "ðďđδ", "ÈÉÊËĒĔĖĘĚΕΈẼẺẸỀẾỄỂỆЕЭ", "èéêëēĕėęěέεẽẻẹềếễểệеэ", "Ф", "ф",
                "ĜĞĠĢΓГҐ", "ĝğġģγгґ", "ĤĦ", "ĥħ", "ÌÍÎÏĨĪĬǏĮİΗΉΊΙΪỈỊИЫ", "ìíîïĩīĭǐįıηήίιϊỉịиыї", "Ĵ", "ĵ", "ĶΚК", "ķκк",
                "ĹĻĽĿŁΛЛ", "ĺļľŀłλл", "М", "м", "ÑŃŅŇΝН", "ñńņňŉνн", "ÒÓÔÕŌŎǑŐƠØǾΟΌΩΏỎỌỒỐỖỔỘỜỚỠỞỢО",
                "òóôõōŏǒőơøǿºοόωώỏọồốỗổộờớỡởợо", "П", "п", "ŔŖŘΡР", "ŕŗřρр", "ŚŜŞȘŠΣС", "śŝşșšſσςс", "ȚŢŤŦτТ", "țţťŧт",
                "ÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛŨỦỤỪỨỮỬỰУ", "ùúûũūŭůűųưǔǖǘǚǜυύϋủụừứữửựу", "ÝŸŶΥΎΫỲỸỶỴЙ", "ýÿŷỳỹỷỵй", "В", "в", "Ŵ", "ŵ",
                "ŹŻŽΖЗ", "źżžζз", "ÆǼ", "ß", "Ĳ", "ĳ", "Œ", "ƒ", "ξ", "π", "β", "μ", "ψ", "Ё", "ё", "Є", "є", "Ї", "Ж",
                "ж", "Х", "х", "Ц", "ц", "Ч", "ч", "Ш", "ш", "Щ", "щ", "ЪъЬь", "Ю", "ю", "Я", "я"
            };

            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return input.ToCharArray().Any(c => foreignCharacters.Any(f => f.IndexOf(c) != -1));
        }

        private static void ExtractDateWildCardParameters(string wildcardDate, string resultDateString, StringBuilder resultDateBuilder, ref int wildcardCopyRangeLength, ref int resultCopyRangeLength, ref int resultCopyRangeLocation, ref int wildcardCopyRangeLocation)
        {
            for (var wildcardIndex = 0; wildcardIndex < WildcardLength; wildcardIndex++)
            {
                if (wildcardDate[wildcardIndex] == '#')
                {
                    if (resultCopyRangeLength > 0)
                    {
                        resultCopyRangeLength++;
                    }
                    else
                    {
                        if (wildcardCopyRangeLength > 0)
                        {
                            resultDateBuilder.Append(wildcardDate.Substring(wildcardCopyRangeLocation, wildcardCopyRangeLength));
                            wildcardCopyRangeLength = 0;
                        }

                        resultCopyRangeLength = 1;
                        resultCopyRangeLocation = wildcardIndex;
                    }
                }
                else
                {
                    if (wildcardCopyRangeLength > 0)
                    {
                        wildcardCopyRangeLength++;
                    }
                    else
                    {
                        if (resultCopyRangeLength > 0)
                        {
                            resultDateBuilder.Append(resultDateString.Substring(resultCopyRangeLocation, resultCopyRangeLength));
                            resultCopyRangeLength = 0;
                        }

                        wildcardCopyRangeLength = 1;
                        wildcardCopyRangeLocation = wildcardIndex;
                    }
                }
            }
        }

        private static bool ExtractDateComponents(string source, int prefixLength, ref string wildcardDate, ref DateTime dateComponents, ref int resultLength, ref int nextPosition, string varPart)
        {
            if (prefixLength == 0 && source.IsWildcardDate())
            {
                wildcardDate = source.Substring(1, 8);
                nextPosition = 9;
            }
            else if (varPart.StartsWith("Day"))
            {
                nextPosition = prefixLength + 3;
            }
            else if (varPart.StartsWith("Date"))
            {
                nextPosition = prefixLength + 4;
            }
            else if (varPart.StartsWith("fdMonth"))
            {
                dateComponents = new DateTime(dateComponents.Year, dateComponents.Month, 1);
                nextPosition = prefixLength + 7;
            }
            else if (varPart.StartsWith("fdQuarter"))
            {
                dateComponents = new DateTime(dateComponents.Year, dateComponents.Month, 1);
                dateComponents = SetQuarterDate(dateComponents);

                nextPosition = prefixLength + 9;
            }
            else if (varPart.StartsWith("fdWeek"))
            {
                var dayOfWeekIndex = (int)dateComponents.DayOfWeek;
                if (dayOfWeekIndex == 1)
                {
                    dayOfWeekIndex = 8;
                }

                dateComponents = new DateTime(dateComponents.Year, dateComponents.Month, dateComponents.Day);
                nextPosition = prefixLength + 6;
            }
            else if (varPart.StartsWith("fdYear"))
            {
                dateComponents = new DateTime(dateComponents.Year, 1, 1);
                nextPosition = prefixLength + 6;
            }
            else if (varPart.StartsWith("YYYY"))
            {
                dateComponents = new DateTime(dateComponents.Year, 1, 1);
                nextPosition = prefixLength + 4;
                resultLength = 4;
            }
            else
            {
                return false;
            }

            return true;
        }

        private static DateTime SetQuarterDate(DateTime dateComponents)
        {
            switch (dateComponents.Month)
            {
                case 2:
                case 3:
                    dateComponents = new DateTime(dateComponents.Year, 1, dateComponents.Day);
                    break;
                case 5:
                case 6:
                    dateComponents = new DateTime(dateComponents.Year, 4, dateComponents.Day);
                    break;
                case 8:
                case 9:
                    dateComponents = new DateTime(dateComponents.Year, 7, dateComponents.Day);
                    break;
                case 11:
                case 12:
                    dateComponents = new DateTime(dateComponents.Year, 10, dateComponents.Day);
                    break;
                default:
                    break;
            }

            return dateComponents;
        }

        private static DateTime PostfixExtractAndAddDateComponents(string source, DateTime date, ref int nextPosition, bool isDate)
        {
            var valueLength = source.Length;
            while (nextPosition < valueLength)
            {
                var addValue = false;
                int endPosition;
                int addVal;
                switch (source[nextPosition])
                {
                    case '+':
                        addValue = true;
                        ++nextPosition;
                        break;
                    case '-':
                        addValue = false;
                        ++nextPosition;
                        break;
                    case ' ':
                        ++nextPosition;
                        continue;
                    default:
                        nextPosition = valueLength;
                        break;
                }

                while (nextPosition < valueLength && source[nextPosition] == ' ')
                {
                    ++nextPosition;
                }

                endPosition = nextPosition;
                while (endPosition < valueLength && source[endPosition] >= '0' && source[endPosition] <= '9')
                {
                    ++endPosition;
                }

                addVal = int.Parse(source.Substring(nextPosition, endPosition - nextPosition));
                nextPosition = endPosition;
                while (nextPosition < valueLength && source[nextPosition] == ' ')
                {
                    ++nextPosition;
                }

                if (!addValue)
                {
                    addVal = -addVal;
                }

                if (nextPosition >= valueLength)
                {
                    break;
                }

                if (isDate)
                {
                    date = AddDateComponentsToDatePostfix(source, date, ref nextPosition, addVal);
                }
                else
                {
                    date = AddDateComponentsToTimePostfix(source, date, ref nextPosition, addVal);
                }
            }

            return date;
        }

        private static DateTime AddDateComponentsToDatePostfix(string source, DateTime date, ref int nextPosition, int addVal)
        {
            var diffDateComponents = TimeSpan.Zero;

            switch (source[nextPosition])
            {
                case 'd':
                case 'D':
                    diffDateComponents = TimeSpan.FromDays(addVal);
                    ++nextPosition;
                    break;
                case 'w':
                case 'W':
                    diffDateComponents = TimeSpan.FromDays(addVal * 7);
                    ++nextPosition;
                    break;
                case 'm':
                case 'M':
                    diffDateComponents = TimeSpan.FromDays(addVal * 30);
                    ++nextPosition;
                    break;
                case 'q':
                case 'Q':
                    diffDateComponents = TimeSpan.FromDays(addVal * 91);
                    ++nextPosition;
                    break;
                case 'y':
                case 'Y':
                    diffDateComponents = TimeSpan.FromDays(365);
                    ++nextPosition;
                    break;
                default:
                    diffDateComponents = TimeSpan.FromDays(addVal);
                    break;
            }

            date = date.Add(diffDateComponents);

            return date;
        }

        private static DateTime AddDateComponentsToTimePostfix(string source, DateTime date, ref int nextPosition, int addVal)
        {
            var diffDateComponents = TimeSpan.Zero;
            switch (source[nextPosition])
            {
                case 'h':
                case 'H':
                    diffDateComponents = TimeSpan.FromHours(addVal);
                    ++nextPosition;
                    break;
                case 'm':
                case 'M':
                    diffDateComponents = TimeSpan.FromMinutes(addVal);
                    ++nextPosition;
                    break;
                default:
                    diffDateComponents = TimeSpan.FromMinutes(addVal);
                    break;
            }

            date = date.Add(diffDateComponents);

            return date;
        }
    }
}
