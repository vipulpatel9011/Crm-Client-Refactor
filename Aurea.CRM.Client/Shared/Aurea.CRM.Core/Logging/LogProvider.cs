// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogProvider.cs" company="Aurea Software Gmbh">
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
//   Log provider implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Logging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.Platform;

    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// Log provider implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Logging.ILogger" />
    public class LogProvider : ILogger
    {
        /// <summary>
        /// Max log file size before purging
        /// </summary>
        private const ulong MaxFileSizeBytes = 10485760;

        /// <summary>
        /// % of log file max size to retain
        /// </summary>
        private const double RetainPercentageOfMaxSize = 0.5;

        /// <summary>
        /// The messenger.
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// The platform.
        /// </summary>
        private readonly IStorageProvider storageProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogProvider"/> class.
        /// </summary>
        /// <param name="storageProvider">Platform service for file handling</param>
        /// <param name="logSettings">log settings service</param>
        /// <param name="messengerInstance">messenger service</param>
        public LogProvider(IStorageProvider storageProvider, ILogSettings logSettings, IMessenger messengerInstance)
        {
            this.messenger = messengerInstance;
            this.storageProvider = storageProvider;
            this.LogSettings = logSettings;
            this.messenger.Register<LoginEventMessage>(this, this.CheckLogFileSize);
        }

        /// <summary>
        /// Gets the file data.
        /// </summary>
        /// <value>
        /// The file data.
        /// </value>
        public byte[] FileData => this.storageProvider.FileContents(this.FileName).GetAwaiter().GetResult();

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName => Path.Combine(this.storageProvider.DocumentsFolderPath, "log.txt");

        /// <summary>
        /// Gets number of days to keep logs during truncate operation
        /// </summary>
        public int KeepLogDays => 3;

        /// <summary>
        /// Gets the log settings.
        /// </summary>
        public ILogSettings LogSettings { get; }

        /// <summary>
        /// Gets the Content of LogFile
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<string> GetLogDataAsync()
        {
            var content = await this.storageProvider.FileContents(this.FileName);
            return System.Text.Encoding.UTF8.GetString(content, 0, content.Length);
        }

        /// <summary>
        /// Logs debug statements
        /// </summary>
        /// <param name="message">message to log</param>
        /// <param name="debugFlag">message type</param>
        /// <param name="callerFilePath">caller class name</param>
        /// <param name="callerName">caller method name</param>
        public void LogDebug(string message, LogFlag debugFlag, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerName = "")
        {
            var logMessage = new LogMessage(message, LogLevel.Debug)
            {
                MethodName = callerName,
                FileName = Path.GetFileName(callerFilePath),
                LogFlag = debugFlag,
            };
            if ((this.LogSettings.LogFlags & debugFlag) != 0)
            {
                this.LogMessage(logMessage);
            }
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="callerFilePath">The file path of the caller</param>
        /// <param name="callerName">The name of the method calling the log statement</param>
        public void LogError(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerName = "")
        {
            var logMessage = new LogMessage(message, LogLevel.Error)
            {
                MethodName = callerName,
                FileName = Path.GetFileName(callerFilePath)
            };
            this.LogMessage(logMessage);
        }

        /// <inheritdoc />
        public void LogError(Exception errorException, [CallerMemberName] string callerPath = "", [CallerMemberName] string callerName = "")
        {
            if (errorException != null)
            {
                this.LogError(errorException.ToString(), callerPath, callerName);
            }
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="callerPath">The file path of the caller</param>
        /// <param name="callerName">The name of the method calling the log statement</param>
        public void LogInfo(string message, [CallerMemberName] string callerPath = "", [CallerMemberName] string callerName = "")
        {
            var logMessage = new LogMessage(message, LogLevel.Info)
            {
                MethodName = callerName,
                FileName = Path.GetFileName(callerPath)
            };
            this.LogMessage(logMessage);
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        public void LogMessage(ILogMessage logMessage)
        {
            try
            {
                var configClass = this.LogSettings.LogClass;
                if (!string.IsNullOrEmpty(configClass) && !configClass.Equals(logMessage.FileName))
                {
                    return;
                }

                var configMethod = this.LogSettings.LogMethod;
                if (!string.IsNullOrWhiteSpace(configMethod) && !configMethod.Equals(logMessage.MethodName))
                {
                    return;
                }

                if (this.LogSettings.LogLevel < logMessage.Level && logMessage.LogFlag == LogFlag.LogNone)
                {
                    return;
                }

                var message = DebugLogFormatter.FormatLogMessage(logMessage);
                this.storageProvider?.AppendTextAsync(this.FileName, message, 500, 200);
            }
            catch(Exception ex)
            {
                this.LogError(ex);
            }
        }

        /// <summary>
        /// Logs the warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="callerPath">The file path of the caller</param>
        /// <param name="callerName">The name of the method calling the log statement</param>
        public void LogWarn(string message, [CallerMemberName] string callerPath = "", [CallerMemberName] string callerName = "")
        {
            var logMessage = new LogMessage(message, LogLevel.Warn)
            {
                MethodName = callerName,
                FileName = callerPath
            };
            this.LogMessage(logMessage);
        }

        /// <summary>
        /// Resets the file.
        /// </summary>
        public void ResetFile()
        {
            this.storageProvider.CreateFile(this.FileName);
        }

        /// <summary>
        /// The check log file size.
        /// </summary>
        /// <param name="m">
        /// The login event message
        /// </param>
        private void CheckLogFileSize(LoginEventMessage m)
        {
            try
            {
                if (m.IsLogedIn && this.storageProvider.FileExists(this.FileName))
                {
                    Task.Run(
                        async () =>
                        {
                            try
                            {
                                var currentFileSize = await this.storageProvider.GetFileSize(this.FileName);
                                if (currentFileSize > MaxFileSizeBytes)
                                {
                                    var logData = await this.GetLogDataAsync();
                                    var allLines = logData
                                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                        .ToList();
                                    var lineSize = (long)currentFileSize / allLines.Count;
                                    var maxNumberOfLines = (int)MaxFileSizeBytes * RetainPercentageOfMaxSize / lineSize;

                                    var linesToSkip = allLines.Count - maxNumberOfLines;

                                    var logLines = allLines.Skip(linesToSkip > 0 ? (int)linesToSkip : 0);

                                    logData = string.Join(Environment.NewLine, logLines);
                                    var logBytes = Encoding.UTF8.GetBytes(logData);
                                    await this.storageProvider.CreateFile(this.FileName, logBytes, true);
                                    this.messenger?.Send(new ToastrMessage
                                    {
                                        MessageText = string.Format(LocalizedString.TextErrorLogPurge),
                                        DetailedMessage = string.Format(
                                            LocalizedString.TextErrorLogPurgeDetail,
                                            MaxFileSizeBytes / (1024 * 1024))
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                this.LogError(ex);
                            }
                        }).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }
    }
}
