using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aurea.CRM.Client.UI.Common
{    
    public static class ListExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }
    }
}
