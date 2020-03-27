// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToExpandIconConverter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Stefan Stanca
// </author>
// <summary>
//   Value converter for xamarin.forms
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Client.UI.Common
{
    using System;
    using System.Globalization;

    using Aurea.CRM.Client.UI.Themes.Icons;

    using Xamarin.Forms;

    /// <summary>
    /// Used to convert a boolean value to it's negated value
    /// </summary>
    /// <seealso cref="Xamarin.Forms.IValueConverter" />
    public class BooleanToExpandIconConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified bool value.
        /// </summary>
        /// <param name="boolValue">The bool value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The nagated input value</returns>
        public object Convert(object boolValue, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)boolValue;
            if (b)
            {
                return HalflingsIcons.ChevronUp;
            }
            else
            {
                return HalflingsIcons.ChevronDown;
            }
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="stringValue">The bool value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The negated input value</returns>
        public object ConvertBack(object stringValue, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (string)stringValue;
            return s == HalflingsIcons.ChevronUp ? true : false;
        }
    }
}
