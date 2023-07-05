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
   public partial class ActivityLoadReportPage : ContentPage {
      ActivityLoadReportViewModel _viewModel;
      public ActivityLoadReportPage() {
         InitializeComponent();

         BindingContext = _viewModel = new ActivityLoadReportViewModel();
      }
   }
}