using PortableLoadCell.Models;
using PortableLoadCell.Services;
using PropertyChanged;
using System.ComponentModel;
using Xamarin.Forms;

namespace PortableLoadCell.ViewModels {

   [AddINotifyPropertyChangedInterface]
   public class BaseViewModel : INotifyPropertyChanged {

      public IDataStore<Training> DataStore => DependencyService.Get<IDataStore<Training>>();

      public bool IsBusy { get; set; }

      public string Title { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
