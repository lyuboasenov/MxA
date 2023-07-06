using MxA.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MxA.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class TrainingsPage : ContentPage {
      TrainingsViewModel _viewModel;
      public TrainingsPage() {
         InitializeComponent();
         BindingContext = _viewModel = new TrainingsViewModel();
      }

      void OnFabTabTapped(object sender, TabTappedEventArgs e) => DisplayAlert("FabTabGallery", "Tab Tapped.", "Ok");

      protected override void OnAppearing() {
         base.OnAppearing();
         _viewModel.OnAppearing();
      }
   }
}