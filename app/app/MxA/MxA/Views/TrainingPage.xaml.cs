using MxA.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MxA.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class TrainingPage : TabbedPage {
      TrainingViewModel _viewModel;
      public TrainingPage() {
         InitializeComponent();
         BindingContext = _viewModel = new TrainingViewModel();
      }

      protected override void OnAppearing() {
         base.OnAppearing();
         _viewModel.OnAppearing();
      }
   }
}