using MxA.Helpers;
using MxA.Themes;

namespace MxA.ViewModels {
   public class SettingsViewModel : BaseViewModel {
      public SettingsViewModel() {
         Title = "Settings";
         Theme = Settings.Theme == 2 ? "Dark" : "Light";
      }

      public string Theme { get; set; }

      public void OnThemeChanged() {
         if (Theme == "Dark") {
            Settings.Theme = 2;
         } else if (Theme == "Light") {
            Settings.Theme = 1;
         } else {
            Settings.Theme = 0;
         }
         TheTheme.SetTheme();
      }
   }
}