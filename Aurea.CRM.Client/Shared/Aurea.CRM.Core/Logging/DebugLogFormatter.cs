// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogFormatter.cs" company="Aurea Software Gmbh">
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
//   Formats log messages
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Logging
{
    using System.Text;

    /// <summary>
    /// Formats log messages
    /// </summary>
    public class DebugLogFormatter
    {
        // : DDLogFormatter
        /// <summary>
        /// The date format
        /// </summary>
        public const string DateFormat = "yyyy/MM/dd HH:mm:ss:SSS";

        /// <summary>
        /// Formats the log message.
        /// </summary>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormatLogMessage(ILogMessage logMessage)
        {
            var dateAndTime = logMessage.Timestamp.ToString(DateFormat);
            var fileName = logMessage.FileName;
            var methodName = logMessage.MethodName;
            var strLogLevel = logMessage.Level.ToString().ToUpper();
            var strLogFlag = logMessage.Level == LogLevel.Debug ? $"[{logMessage.LogFlag}]" : string.Empty;
            var sessionInfo = new StringBuilder();
            var addSeparator = false;
            if (!string.IsNullOrEmpty(fileName))
            {
                sessionInfo.AppendFormat($" {fileName}");
                addSeparator = true;
            }

            if (!string.IsNullOrEmpty(methodName))
            {
                if (addSeparator)
                {
                    sessionInfo.Append(".");
                }

                sessionInfo.AppendFormat($"{methodName} ");
            }

            return $"[{strLogLevel}] {strLogFlag};{dateAndTime};({sessionInfo});{logMessage.Message}";
        }
    }
}
