using PortableLoadCell.Models;
using PortableLoadCell.ViewModels;
using PortableLoadCell.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PortableLoadCell.Views {
   public partial class BleDevicesPage : ContentPage {
      BleDevicesViewModel _viewModel;

      public BleDevicesPage() {
         InitializeComponent();

         BindingContext = _viewModel = new BleDevicesViewModel();
      }

      protected override void OnAppearing() {
         base.OnAppearing();
         _viewModel.OnAppearing();
      }
   }
}