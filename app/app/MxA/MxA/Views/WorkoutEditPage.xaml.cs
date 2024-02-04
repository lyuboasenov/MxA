using MxA.ViewModels;

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