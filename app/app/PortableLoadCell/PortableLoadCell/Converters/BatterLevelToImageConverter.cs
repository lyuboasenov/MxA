using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace PortableLoadCell.Converters {
   class BatterLevelToImageConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is uint percent) {

            int index = (int) (percent / 12.5);
            string indexStr = $"{index}_bar";
            if (index > 6) {
               indexStr = "full";
            }

            return ImageSource.FromResource($"PortableLoadCell.Resources.icons.battery_{indexStr}_FILL0_wght400_GRAD0_opsz48.png", typeof(BatterLevelToImageConverter).GetTypeInfo().Assembly);
         }

         return value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
