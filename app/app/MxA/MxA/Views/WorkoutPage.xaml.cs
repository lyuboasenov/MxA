using MxA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MxA.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class WorkoutPage : ContentPage {
      private WorkoutViewModel _viewModel;

      public WorkoutPage() {
         InitializeComponent();
         BindingContext = _viewModel = new WorkoutViewModel();
      }
   }
}