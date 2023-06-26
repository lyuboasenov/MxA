using MxA.Models;
using MxA.ViewModels;
using MxA.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MxA.Views {
   public partial class ExercisesPage : ContentPage {
      WorkoutsViewModel _viewModel;

      public ExercisesPage() {
         InitializeComponent();

         BindingContext = _viewModel = new WorkoutsViewModel();
      }

      protected override void OnAppearing() {
         base.OnAppearing();
         _viewModel.OnAppearing();
      }
   }
}