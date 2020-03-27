// <copyright file="BooleanToColorConverter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Client.UI.Common
{
    using System;
    using System.Globalization;
    using GalaSoft.MvvmLight.Ioc;
    using Services.Theme;
    using Xamarin.Forms;

    /// <summary>
    /// Converter for search bar buttons
    /// </summary>
    public class BooleanToColorConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            if (val)
            {
                return SimpleIoc.Default.GetInstance<IThemeService>().GetStyle<Color>("XfSearchFilterIconColor");
            }
            else
            {
                return Color.LightGray;
            }
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
