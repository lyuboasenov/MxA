﻿using MxA.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MxA.Views {
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

      protected override void OnSizeAllocated(double width, double height) {
         base.OnSizeAllocated(width, height);
         if (width > height) {
            leftRightContainer.Orientation = StackOrientation.Horizontal;
         } else {
            leftRightContainer.Orientation = StackOrientation.Vertical;
         }
      }
   }
}