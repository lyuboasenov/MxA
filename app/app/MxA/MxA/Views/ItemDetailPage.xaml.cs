using MxA.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MxA.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}