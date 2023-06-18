using MxA.Icons.Material;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace MxA.Converters {
   class BatterLevelToGlyphConverter : IValueConverter {
      private static string[] Glyphs = {

         IconFont.Battery_0_bar,
         IconFont.Battery_1_bar,
         IconFont.Battery_2_bar,
         IconFont.Battery_3_bar,
         IconFont.Battery_4_bar,
         IconFont.Battery_5_bar,
         IconFont.Battery_6_bar,
         IconFont.Battery_full
      };
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is uint percent) {

            int index = (int) (percent / 12.5);
            if (index > 6) {
               index = 7;
            }

            return Glyphs[index];
         }

         return value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
