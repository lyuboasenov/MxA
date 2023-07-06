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

namespace MxA.Views
{
    public partial class ActivityLogsPage : ContentPage
    {
        ActivityLogsViewModel _viewModel;

        public ActivityLogsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ActivityLogsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}