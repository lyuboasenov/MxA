﻿using MxA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MxA.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class WorkoutEditPage : ContentPage {
      private WorkoutEditViewModel _viewModel;

      public WorkoutEditPage() {
         InitializeComponent();
         BindingContext = _viewModel = new WorkoutEditViewModel();
      }
   }
}