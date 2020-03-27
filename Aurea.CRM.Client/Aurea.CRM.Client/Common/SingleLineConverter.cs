// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleLineConverter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Felipe Botero
// </author>
// <summary>
//   Used to convert a multple line string to a single line string
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Client.UI.Common
{
    using System;
    using System.Globalization;
    using Xamarin.Forms;

    /// <summary>
    /// Used to convert a multple line string to a single line string
    /// </summary>
    /// <seealso cref="Xamarin.Forms.IValueConverter" />
    public class SingleLineConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified string value.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// The single line value
        /// </returns>
        public object Convert(object inputString, Type targetType, object parameter, CultureInfo culture)
        {
            return inputString is string ? ((string)inputString).Replace('\n', ' ') : string.Empty;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// The single line value
        /// </returns>
        public object ConvertBack(object inputString, Type targetType, object parameter, CultureInfo culture)
        {
            return inputString is string ? ((string)inputString) : string.Empty;
        }
    }
}
