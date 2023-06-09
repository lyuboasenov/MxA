using PortableLoadCell.Models;
using PortableLoadCell.Services;
using PortableLoadCell.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PortableLoadCell {
   public partial class App : Application {

      public App() {
         InitializeComponent();

         DependencyService.Register<IDataStore<Training>, LocalFileTrainingDataStore>();
         MainPage = new AppShell();
      }

      protected override void OnStart() {
      }

      protected override void OnSleep() {
      }

      protected override void OnResume() {
      }
   }
}
