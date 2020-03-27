// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtension.cs" company="Aurea Software Gmbh">
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
//   Exception extensions
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Extensions
{
    using System;

    /// <summary>
    /// Exception extensions
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// Determines whether [is connection offline error].
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>
        ///   <c>true</c> if [is connection offline error] [the specified error]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsConnectionOfflineError(this Exception error)
        {
            return error?.Message?.Contains("ConnectionOfflineError") ?? false;
        }

        /// <summary>
        /// Determines whether [connection is dropped].
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>
        ///   <c>true</c> if [is connection is dropped] [the specified error]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsConnectionError(this Exception error)
        {
            return error?.Message?.Contains("establish") ?? false;
        }

        /// <summary>
        /// Determines whether [is licensing error].
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>
        ///   <c>true</c> if [is licensing error] [the specified error]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLicensingError(this Exception error)
        {
            return error?.Message?.Contains("MODULE_RIGHT") ?? false;
        }

        /// <summary>
        /// Determines whether [is invalid login error].
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>
        ///   <c>true</c> if [is invalid login error] [the specified error]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInvalidLoginError(this Exception error)
        {
            return error?.Message?.Contains("Status Code: Forbidden") ?? false;
        }

        /// <summary>
        /// Determines whether [is not authenticated error].
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>
        ///   <c>true</c> if [is not authenticated error] [the specified error]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotAuthenticatedError(this Exception error)
        {
            if (error?.Message == null)
            {
                return false;
            }

            return error.Message.Contains("user not authenticated") || error.Message.Contains("Status Code: Forbidden");
        }

        /// <summary>
        /// Determines whether [is application starting error].
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>
        ///   <c>true</c> if [is application starting error] [the specified error]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsApplicationStartingError(this Exception error)
        {
            return error?.Message?.Contains("with the application being in state 'Starting'") ?? false;
        }
    }
}
