using System;
using System.Globalization;
using Xamarin.Forms;

namespace MxA.Converters {
   class AddValueConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         return (int)value + (int)parameter;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
