// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogMessage.cs" company="Aurea Software Gmbh">
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
//   Simple log message implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Logging
{
    using System;

    /// <summary>
    /// Simple log message implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Logging.ILogMessage" />
    public class LogMessage : ILogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        public LogMessage(string message, LogLevel level)
        {
            this.Timestamp = DateTime.Now;
            this.Message = message;
            this.Level = level;
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the flag.
        /// </summary>
        /// <value>
        /// The flag.
        /// </value>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the flag.
        /// </summary>
        /// <value>
        /// The flag.
        /// </value>
        public LogFlag LogFlag { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
