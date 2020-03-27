// <copyright file="ServerException.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Azat Jalilov
// </author>
// <summary>
//   Exception with stack trace from server, StackTrace of ordinar exception can not be modified
// </summary>

namespace Aurea.CRM.Core.Common
{
    using System;

    /// <summary>
    /// ServerException
    /// </summary>
    public class ServerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerException"/> class.
        /// </summary>
        /// <param name="message">Exception message</param>
        public ServerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Gets or sets servers stacktrace
        /// </summary>
        public string ServerStackTrace { get; set; }
    }
}
