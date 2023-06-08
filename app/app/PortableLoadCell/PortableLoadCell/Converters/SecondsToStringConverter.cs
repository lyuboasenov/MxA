using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PortableLoadCell.Converters {
   public class SecondsToStringConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is uint seconds) {
            if (seconds < 60) {
               return $"{seconds:00}";
            } else {
               var mins = seconds / 60;
               var secs = seconds % 60;

               return $"{mins:00}:{secs:00}";
            }
         }

         return value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
