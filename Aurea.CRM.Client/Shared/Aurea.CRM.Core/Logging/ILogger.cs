// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Aurea Software Gmbh">
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
//   Differnt levels of log messages
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Logging
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Different levels of log messages
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// The off
        /// </summary>
        Off,

        /// <summary>
        /// The error.
        /// </summary>
        Error,

        /// <summary>
        /// The warn.
        /// </summary>
        Warn,

        /// <summary>
        /// The info.
        /// </summary>
        Info,

        /// <summary>
        /// The debug.
        /// </summary>
        Debug
    }

    /// <summary>
    /// Different levels of log messages
    /// </summary>
    [Flags]
    public enum LogFlag
    {
         /// <summary>
        ///  Log None
        /// </summary>
        LogNone = 0,

        /// <summary>
        ///  Log config
        /// </summary>
        LogConfig = 1,

        /// <summary>
        /// Log network
        /// </summary>
        LogNetwork = 2,

        /// <summary>
        /// Log questionnaire
        /// </summary>
        LogQuestionnaire = 4,

        /// <summary>
        /// Log requests
        /// </summary>
        LogRequests = 8,

        /// <summary>
        /// Log results
        /// </summary>
        LogResults = 16,

        /// <summary>
        /// Log statements
        /// </summary>
        LogStatements = 32,

        /// <summary>
        /// Log upsync
        /// </summary>
        LogUpSync = 64,

        /// <summary>
        /// Log Quick Actions
        /// </summary>
        LogSerialEntry = 128,

        /// <summary>
        /// All requests are logged
        /// </summary>
        LogAll = 255,
    }

    /// <summary>
    /// Interface for a logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the settings
        /// </summary>
        ILogSettings LogSettings { get; }

        /// <summary>
        /// Gets the file data.
        /// </summary>
        /// <value>
        /// The file data.
        /// </value>
        byte[] FileData { get; }

        /// <summary>
        /// Gets the Content of LogFile
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetLogDataAsync();

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        string FileName { get; }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="callerPath">The file path of the caller</param>
        /// <param name="callerName">The name of the method calling the log statement</param>
        void LogError(string error, [CallerMemberName]string callerPath = "", [CallerMemberName] string callerName = "");

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        void LogMessage(ILogMessage logMessage);

        /// <summary>
        /// Logs the warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="callerPath">The file path of the caller</param>
        /// <param name="callerName">The name of the method calling the log statement</param>
        void LogWarn(string message, [CallerMemberName] string callerPath = "", [CallerMemberName] string callerName = "");

        /// <summary>
        /// Logs the info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// /// <param name="callerPath">The file path of the caller</param>
        /// <param name="callerName">The name of the method calling the log statement</param>
        void LogInfo(string message, [CallerMemberName]string callerPath = "", [CallerMemberName]string callerName = "");

        /// <summary>
        /// Resets the file.
        /// </summary>
        void ResetFile();

        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="error">Exception to log</param>
        /// <param name="callerFilePath">caller class name</param>
        /// <param name="callerName">caller method name</param>
        void LogError(Exception error, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerName = "");

        /// <summary>
        /// Logs debug statements
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="debugFlag">message type</param>
        /// <param name="callerFilePath">caller class name</param>
        /// <param name="callerName">caller method name</param>
        void LogDebug(string message, LogFlag debugFlag, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerName = "");
    }

    /// <summary>
    /// Interface for a log message
    /// </summary>
    public interface ILogMessage
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        string FileName { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the flag.
        /// </summary>
        /// <value>
        /// The flag.
        /// </value>
        LogFlag LogFlag { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Interface for log settings
    /// </summary>
    public interface ILogSettings
    {
        /// <summary>
        /// Gets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        LogLevel LogLevel { get; }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        LogFlag LogFlags { get; }

        /// <summary>
        /// Gets the log class.
        /// </summary>
        /// <value>
        /// The log class.
        /// </value>
        string LogClass { get; }

        /// <summary>
        /// Gets a value indicating whether [log configuration].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log configuration]; otherwise, <c>false</c>.
        /// </value>
        bool LogConfig { get; }

        /// <summary>
        /// Gets the log method.
        /// </summary>
        /// <value>
        /// The log method.
        /// </value>
        string LogMethod { get; }

        /// <summary>
        /// Gets a value indicating whether [log network].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log network]; otherwise, <c>false</c>.
        /// </value>
        bool LogNetwork { get; }

        /// <summary>
        /// Gets a value indicating whether [log questionnaire].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log questionnaire]; otherwise, <c>false</c>.
        /// </value>
        bool LogQuestionnaire { get; }

        /// <summary>
        /// Gets a value indicating whether [log requests].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log requests]; otherwise, <c>false</c>.
        /// </value>
        bool LogRequests { get; }

        /// <summary>
        /// Gets a value indicating whether [log results].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log results]; otherwise, <c>false</c>.
        /// </value>
        bool LogResults { get; }

        /// <summary>
        /// Gets a value indicating whether [log serial entry].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log serial entry]; otherwise, <c>false</c>.
        /// </value>
        bool LogSerialEntry { get; }

        /// <summary>
        /// Gets a value indicating whether [log statements].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log statements]; otherwise, <c>false</c>.
        /// </value>
        bool LogStatements { get; }

        /// <summary>
        /// Gets a value indicating whether [log up synchronize].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [log up synchronize]; otherwise, <c>false</c>.
        /// </value>
        bool LogUpSync { get; }

        /// <summary>
        /// Gets a value indicating whether the local log settings are active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if override is active otherwise, <c>false</c>.
        /// </value>
        bool OverrideServerSettings { get; }

        /// <summary>
        /// Gets the email address where the log report will be delivered
        /// </summary>
        string LogReportEmailAddress { get; }

        /// <summary>
        /// Reloads log settings from the config store and saves them to local storage
        /// </summary>
        /// <param name="configStore">The config store from which to get log settings</param>
        void UpdateSettingsFromSession(IConfigurationUnitStore configStore);
    }
}
