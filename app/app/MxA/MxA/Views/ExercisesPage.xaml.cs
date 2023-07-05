using MxA.ViewModels;
using Xamarin.Forms;

namespace MxA.Views {
   public partial class ExercisesPage : ContentPage {
      ExercisesViewModel _viewModel;

      public ExercisesPage() {
         InitializeComponent();

         BindingContext = _viewModel = new ExercisesViewModel();
      }

      protected override void OnAppearing() {
         base.OnAppearing();
         _viewModel.OnAppearing();
      }
   }
}