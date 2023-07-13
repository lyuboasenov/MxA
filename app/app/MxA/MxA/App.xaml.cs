using MxA.Database.Services;
using MxA.Themes;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Svg;

namespace MxA {
   public partial class App : Application {

      public App() {
         InitializeComponent();
         SvgImageSource.RegisterAssembly();
         DependencyService.Register<IDataStore, DataStore>();
         MainPage = new AppShell();
      }

      protected override void OnStart() {
         OnResume();
      }

      protected override void OnSleep() {
         TheTheme.SetTheme();
         RequestedThemeChanged -= App_RequestedThemeChanged;
      }

      protected override void OnResume() {
         TheTheme.SetTheme();
         RequestedThemeChanged += App_RequestedThemeChanged;
      }

      private void App_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e) {
         MainThread.BeginInvokeOnMainThread(() =>
         {
            TheTheme.SetTheme();
         });
      }
   }
}
