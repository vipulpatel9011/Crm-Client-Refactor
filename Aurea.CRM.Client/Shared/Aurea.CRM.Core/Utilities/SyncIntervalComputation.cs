// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncIntervalComputation.cs" company="Aurea Software Gmbh">
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
//   Sync interval calculation implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Sync interval calculation implementation
    /// </summary>
    public class UPSyncIntervalComputation
    {
        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Creates the specified configuration string.
        /// </summary>
        /// <param name="configurationString">The configuration string.</param>
        /// <returns></returns>
        public static UPSyncIntervalComputation Create(string configurationString)
        {
            if (string.IsNullOrEmpty(configurationString))
            {
                return null;
            }

            var configDict = configurationString.JsonDictionaryFromString();
            if (configDict == null || configDict.Count == 0)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogWarn($"SyncIntervalComputation: invalid configuration string '{configurationString}' -> disabled");
                return null;
            }

            return new UPSyncIntervalComputation(configDict);
        }

        private UPSyncIntervalComputation(Dictionary<string, object> configDictionary)
        {
            this.Configuration = configDictionary;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public Dictionary<string, object> Configuration { get; private set; }

        /// <summary>
        /// Nexts the incremental synchronize for.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        public TimeSpan? NextIncrementalSyncFor(DateTime date)
        {
            var timeInterval = this.NextIncrementalSyncForWithConfigurationTimezone(date, this.Configuration, null);
            if (timeInterval > TimeSpan.Zero)
            {
                var nextDate = date.Add(timeInterval);
                if (nextDate < DateTime.UtcNow)
                {
                    // if (UPLogSettings.LogUpSync())
                    // {
                    // DDLogInfo("UPSync: next incremental sync: immediately (after %@)", nextDate);
                    // }
                    this.Logger.LogDebug($"UpSync: next incremental sync: immediately after {nextDate}", LogFlag.LogUpSync);
                    return TimeSpan.Zero;
                }

                // if (UPLogSettings.LogUpSync())
                // {
                // DDLogInfo("UPSync: next incremental sync in %5.2f minutes (%@)", timeInterval / 60, nextDate);
                // }
                this.Logger.LogDebug($"UpSync: next incremental sync in {timeInterval.TotalMinutes} ({nextDate})", LogFlag.LogUpSync);
                return timeInterval;
            }

            return null;
        }

        /// <summary>
        /// Nexts the incremental synchronize for with configuration timezone.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        public TimeSpan NextIncrementalSyncForWithConfigurationTimezone(
            DateTime date,
            Dictionary<string, object> configuration,
            TimeZoneInfo timeZone)
        {
            TimeSpan myTimeInterval;
            var timeZoneString = configuration.ValueOrDefault("timezone") as string;
            if (!string.IsNullOrEmpty(timeZoneString))
            {
                // NSTimeZone.TimeZoneWithName(timeZoneString)
                // Until we have TimeZone support on the device
                var curTimeZone = TimeZoneInfo.Local;
                if (curTimeZone != null)
                {
                    timeZone = curTimeZone;
                }
            }

            List<string> hours;
            var hour = configuration.ValueOrDefault("hour") as string;
            if (hour != null)
            {
                hours = new List<string> { hour };
            }
            else
            {
                var hourList = configuration.ValueOrDefault("hours");

                // TODO: Need to parse "hours" configuration properly
                // hours = (List<string>) hourList;
                return TimeSpan.Zero;
            }

            var minDiff = 24d;
            var currentHour = ((double)(date.Hour * 60 + date.Minute)) / 60;
            var minHour = 24d;

            foreach (var h in hours)
            {
                var hd = double.Parse(h);
                var diff = hd - currentHour;
                if (diff > 0 && diff < minDiff)
                {
                    minDiff = diff;
                }

                if (hd < minHour)
                {
                    minHour = hd;
                }
            }

            // Sunday = 0, Saturday = 6
            var currentWeekDay = (int)date.DayOfWeek;
            var sameDaySync = true;
            if (Equals(minDiff, 24d))
            {
                minDiff = minHour - currentHour + 24;
                sameDaySync = false;
            }

            List<string> weekdays = null;
            var weekday = configuration.ValueOrDefault("weekday") as string;
            if (!string.IsNullOrEmpty(weekday))
            {
                weekdays = new List<string> { weekday };
            }
            else
            {
                // TODO: Need to parse "weekdays" configuration properly
                // weekdays = configuration.ValueOrDefault("weekdays");
            }

            if (weekdays?.Count > 0)
            {
                double foundDiff = 24 * 7;
                foreach (var day in weekdays)
                {
                    var curday = int.Parse(day);
                    double d;
                    if (curday == currentWeekDay)
                    {
                        if (sameDaySync)
                        {
                            foundDiff = minDiff;
                            break;
                        }

                        d = minHour + (24 * 7) - currentHour;
                    }
                    else if (curday < currentWeekDay)
                    {
                        d = minHour + (7 - currentWeekDay + curday) * 24 - currentHour;
                    }
                    else
                    {
                        d = minHour + (curday - currentWeekDay) * 24 - currentHour;
                    }

                    if (d < foundDiff)
                    {
                        foundDiff = d;
                    }
                }

                minDiff = foundDiff;
            }

            myTimeInterval = TimeSpan.FromSeconds(minDiff * 3600);

            var alternateConfigs = configuration.ValueOrDefault("alternates") as List<object>;

            if (alternateConfigs?.Count > 0)
            {
                foreach (var config in alternateConfigs)
                {
                    var alternateConfig = config as Dictionary<string, object>;
                    if (alternateConfig == null)
                    {
                        continue;
                    }

                    var ti = this.NextIncrementalSyncForWithConfigurationTimezone(date, alternateConfig, timeZone);
                    if (ti > TimeSpan.Zero && ti < myTimeInterval)
                    {
                        myTimeInterval = ti;
                    }
                }
            }

            return myTimeInterval;
        }
    }
}
