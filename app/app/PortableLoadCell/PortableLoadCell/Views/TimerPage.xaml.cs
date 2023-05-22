using PortableLoadCell.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PortableLoadCell.Views {
   public partial class TimerPage : ContentPage {
      TimerViewModel _viewModel;
      public TimerPage() {
         InitializeComponent();
         BindingContext = _viewModel = new TimerViewModel();
      }

      protected override void OnAppearing() {
         base.OnAppearing();
         _viewModel.OnAppearing();
      }

      protected override void OnDisappearing() {
           base.OnDisappearing();
         _viewModel.OnDisappearing();
      }
   }
}