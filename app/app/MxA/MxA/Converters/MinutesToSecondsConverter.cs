using System;
using System.Globalization;
using Xamarin.Forms;

namespace MxA.Converters {
   public class MinutesToSecondsConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         bool invert = parameter != null && parameter is bool b && b;
         if (value is uint minutes) {
            if (invert) {
               return minutes / 60;
            } else {
               return minutes * 60;
            }

         }

         return value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         bool invert = parameter != null && parameter is bool b && b;
         if (value is uint minutes) {
            if (invert) {
               return minutes * 60;
            } else {
               return minutes / 60;
            }

         }

         return value;
      }
   }
}
