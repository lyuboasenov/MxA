using MxA.ViewModels;
using MxA.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MxA {
   public partial class AppShell : Xamarin.Forms.Shell {
      public AppShell() {
         InitializeComponent();
         Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
         Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
         Routing.RegisterRoute(nameof(TimerPage), typeof(TimerPage));
         Routing.RegisterRoute(nameof(BleDevicesPage), typeof(BleDevicesPage));
         Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
         Routing.RegisterRoute(nameof(TrainingPage), typeof(TrainingPage));
         Routing.RegisterRoute(nameof(WorkoutPage), typeof(WorkoutPage));
      }

      private async void OnMenuItemClicked(object sender, EventArgs e) {
         // await Shell.Current.GoToAsync("//LoginPage");
      }
   }
}
