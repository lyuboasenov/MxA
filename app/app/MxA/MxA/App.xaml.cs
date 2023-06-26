using MxA.Database.Services;
using MxA.Models;
using MxA.Services;
using MxA.Themes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MxA {
   public partial class App : Application {

      public App() {
         InitializeComponent();
         DependencyService.Register<IDataStoreEntity<Training>, LocalFileTrainingDataStore>();
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
