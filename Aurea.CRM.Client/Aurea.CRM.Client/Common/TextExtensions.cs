// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextExtensions.cs" company="Aurea Software Gmbh">
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
//   Text extensions
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Client.UI.Common
{
    using System.Linq;

    /// <summary>
    /// Text extension methods class
    /// </summary>
    public static class TextExtensions
    {
        /// <summary>
        /// Return shortened text when length exceeds provided as parameter
        /// </summary>
        /// <param name="textToShorten">text to shorten</param>
        /// <param name="maxLength">max text length</param>
        /// <returns>shortened text</returns>
        public static string ShortenText(this string textToShorten, int maxLength)
        {
            if (maxLength < 3)
            {
                maxLength = textToShorten.Length;
            }

            if (textToShorten?.Length > maxLength)
            {
                return $"{textToShorten.Substring(0, maxLength - 3)}...";
            }

            return textToShorten;
        }

        /// <summary>
        /// Checks if the input string can be found in the specified values strings array.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="valuesStrings">The values strings.</param>
        /// <returns>True if the input string can be found in the specified values strings array</returns>
        public static bool In(this string text, params string[] valuesStrings)
        {
            return valuesStrings.Contains(text);
        }
    }
}
