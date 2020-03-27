// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextConverter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Felipe Botero
// </author>
// <summary>
//   Helper for text conversion
// </summary>
// ------------------------------------------------------------------------
namespace Aurea.CRM.Core.Utilities
{
    /// <summary>
    /// Helper for text conversion
    /// </summary>
    public class TextConverter
    {
        /// <summary>
        /// Remove line breaks of string
        /// </summary>
        /// <param name="text">The string to format</param>
        /// <param name="replaceWithBlankSpace"> True if the line break should be replaced with a blank space</param>
        /// <returns>The formatted string</returns>
        public static string RemoveLineBreaks(string text, bool replaceWithBlankSpace = false)
        {
            return replaceWithBlankSpace ? text?.Replace("\n", " ") : text?.Replace("\n", string.Empty);
        }

        /// <summary>
        /// Remove line breaks of string
        /// </summary>
        /// <param name="text">The string to format</param>
        /// <param name="replaceTabSpace"> True if the line break should be replaced with a blank space</param>
        /// <returns>The formatted string</returns>
        public static string AddLineBreaks(string text, bool replaceTabSpace = true)
        {
            return replaceTabSpace ? text?.Replace("\n", System.Environment.NewLine).Replace("\r", System.Environment.NewLine)
                : text?.Replace("\n", System.Environment.NewLine);
        }
    }
}
