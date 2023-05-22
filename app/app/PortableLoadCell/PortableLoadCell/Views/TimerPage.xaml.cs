using PortableLoadCell.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PortableLoadCell.Views
{
    public partial class TimerPage : ContentPage
    {
        public TimerPage()
        {
            InitializeComponent();
            BindingContext = new TimerViewModel();
        }
    }
}