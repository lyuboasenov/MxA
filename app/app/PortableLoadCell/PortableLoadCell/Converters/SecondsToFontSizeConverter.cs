using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PortableLoadCell.Converters {
   public class SecondsToFontSizeConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is uint seconds) {
            if (seconds < 60) {
               return 200;
            } else {
               return 130;
            }
         }

         return value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
