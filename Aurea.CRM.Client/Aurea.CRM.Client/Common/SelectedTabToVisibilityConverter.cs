// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedTabToVisibilityConverter.cs" company="Aurea Software Gmbh">
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
//   Value converter for setting the selected tab visibility
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Client.UI.Common
{
    using System;
    using System.Globalization;
    using Xamarin.Forms;

    /// <summary>
    /// Used to convert a boolean value (is focused) to it's coresponding background color
    /// </summary>
    /// <seealso cref="Xamarin.Forms.IValueConverter" />
    public class SelectedTabToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified bool value.
        /// </summary>
        /// <param name="selectedTabIndex">Index of the selected tab.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// background color for focused item
        /// </returns>
        public object Convert(object selectedTabIndex, Type targetType, object parameter, CultureInfo culture)
        {
            var isParameterInt = int.TryParse(parameter.ToString(), out var intParam);
            return selectedTabIndex is int && isParameterInt && (int)selectedTabIndex == intParam;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="selectedTabIndex">Index of the selected tab.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>background color for focused item</returns>
        public object ConvertBack(object selectedTabIndex, Type targetType, object parameter, CultureInfo culture)
        {
            var isParameterInt = int.TryParse(parameter.ToString(), out var intParam);
            return selectedTabIndex is int && isParameterInt && (int)selectedTabIndex == intParam;
        }
    }
}
