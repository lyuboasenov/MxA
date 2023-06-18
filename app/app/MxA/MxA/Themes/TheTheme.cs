using MxA.Helpers;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MxA.Themes {
   public static class TheTheme {
      public static void SetTheme() {
         switch (Settings.Theme) {
            //default
            case 0:
               App.Current.UserAppTheme = OSAppTheme.Unspecified;
               break;
            //light
            case 1:
               App.Current.UserAppTheme = OSAppTheme.Light;
               break;
            //dark
            case 2:
               App.Current.UserAppTheme = OSAppTheme.Dark;
               break;
         }

         var nav = App.Current.MainPage as NavigationPage;

         var e = DependencyService.Get<IEnvironment>();
         if (App.Current.RequestedTheme == OSAppTheme.Dark) {
            object primaryColor = null;
            try {
               primaryColor = Application.Current.Resources["Primary"];
            } catch { }
            Color color = Color.Black;
            if (null != primaryColor) {
               color = (Color) primaryColor;
            }
            e?.SetStatusBarColor(color, false);
            if (nav != null) {
               nav.BarBackgroundColor = Color.Black;
               nav.BarTextColor = Color.White;
            }
         } else {
            object primaryColor = null;
            try {
               primaryColor = Application.Current.Resources["Primary"];
            } catch { }
            Color color = Color.White;
            if (null != primaryColor) {
               color = (Color) primaryColor;
            }
            e?.SetStatusBarColor(color, true);
            if (nav != null) {
               nav.BarBackgroundColor = Color.White;
               nav.BarTextColor = Color.Black;
            }
         }

         ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
         if (mergedDictionaries != null) {
            mergedDictionaries.Clear();

            switch (Settings.Theme) {
               case 2:
                  mergedDictionaries.Add(new Dark());
                  break;
               case 1:
               default:
                  mergedDictionaries.Add(new Light());
                  break;
            }
         }
      }
   }
}
