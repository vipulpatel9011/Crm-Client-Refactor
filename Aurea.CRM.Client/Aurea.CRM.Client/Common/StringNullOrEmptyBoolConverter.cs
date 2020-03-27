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
namespace Aurea.CRM.Client.UI.Common
{
    using System;
    using System.Globalization;

    using Aurea.CRM.Client.UI.Themes.Icons;

    using Xamarin.Forms;

    /// <summary>
    /// Helper for text conversion
    /// </summary>
    public class StringNullOrEmptyBoolConverter : IValueConverter
    {
        /// <summary>Returns false if string is null or empty
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;

            if (!String.IsNullOrEmpty(s))
            {
                return true;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
