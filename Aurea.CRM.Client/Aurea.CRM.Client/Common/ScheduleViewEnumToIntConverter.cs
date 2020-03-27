﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanNegationConverter.cs" company="Aurea Software Gmbh">
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
//   Value converter for xamarin.forms
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Client.UI.Common
{
    using Syncfusion.SfSchedule.XForms;
    using System;
    using System.Globalization;
    using Xamarin.Forms;

    /// <summary>
    /// Used to convert a boolean value to it's negated value
    /// </summary>
    /// <seealso cref="Xamarin.Forms.IValueConverter" />
    public class ScheduleViewEnumToIntConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified bool value.
        /// </summary>
        /// <param name="boolValue">The bool value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The nagated input value</returns>
        public object Convert(object enumValue, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)enumValue)
            {
                case 1:
                    return ScheduleView.WorkWeekView;
                case 2:
                    return ScheduleView.WeekView;
                default:
                    return Enum.Parse(typeof(ScheduleView), enumValue?.ToString());
            }
            //return Enum.Parse(enumValue.GetType(), enumValue?.ToString());
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="boolValue">The bool value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The negated input value</returns>
        public object ConvertBack(object intValue, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)intValue)
            {
                case 1:
                    return ScheduleView.WorkWeekView;
                case 2:
                    return ScheduleView.WeekView;
                default:
                    return (ScheduleView)intValue;
            }

            //return (ScheduleView)intValue;

        }
    }
}
