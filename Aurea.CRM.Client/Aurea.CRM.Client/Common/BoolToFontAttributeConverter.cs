using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Aurea.CRM.Client.UI.Common
{
    public class BoolToFontAttributeConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((Boolean)value)
                    return FontAttributes.Bold;
                else
                    return FontAttributes.None;
            }
            return FontAttributes.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
