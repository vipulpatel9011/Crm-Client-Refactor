// <copyright file="EmailExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

using System;

namespace Aurea.CRM.Services.Extensions
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Email extensions
    /// </summary>
    public static class EmailExtensions
    {
        /// <summary>
        /// Validates email address using regex
        /// </summary>
        /// <param name="email">
        /// The email string
        /// </param>
        /// <returns>
        /// <see cref="bool"/>.
        /// </returns>
        public static bool IsValidEmail(this string email)
        {
            var isValid = true;
            if (string.IsNullOrEmpty(email))
            {
                return isValid;
            }

            // Return true if strIn is in valid email format.
            try
            {
                return Regex.IsMatch(
                    email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase,
                    TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
