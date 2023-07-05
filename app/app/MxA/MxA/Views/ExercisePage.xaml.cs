using MxA.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MxA.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ExercisePage : ContentPage {
      ExerciseViewModel _viewModel;

      public ExercisePage() {
         InitializeComponent();
         BindingContext = _viewModel = new ExerciseViewModel();
      }
   }
}