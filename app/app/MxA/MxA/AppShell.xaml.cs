using MxA.Views;
using System;
using Xamarin.Forms;

namespace MxA {
   public partial class AppShell : Xamarin.Forms.Shell {
      public AppShell() {
         InitializeComponent();
         Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
         Routing.RegisterRoute(nameof(TimerPage), typeof(TimerPage));
         Routing.RegisterRoute(nameof(BleDevicesPage), typeof(BleDevicesPage));
         Routing.RegisterRoute(nameof(TrainingsPage), typeof(TrainingsPage));
         Routing.RegisterRoute(nameof(WorkoutPage), typeof(WorkoutPage));
         Routing.RegisterRoute(nameof(WorkoutEditPage), typeof(WorkoutEditPage));
         Routing.RegisterRoute(nameof(ExercisePage), typeof(ExercisePage));
         Routing.RegisterRoute(nameof(ActivityEditPage), typeof(ActivityEditPage));
         Routing.RegisterRoute(nameof(ActivityLogsPage), typeof(ActivityLogsPage));
         Routing.RegisterRoute(nameof(ActivityLoadReportPage), typeof(ActivityLoadReportPage));
      }

      private async void OnMenuItemClicked(object sender, EventArgs e) {
         // await Shell.Current.GoToAsync("//LoginPage");
      }
   }
}
