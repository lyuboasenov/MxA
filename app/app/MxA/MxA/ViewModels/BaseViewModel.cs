using MxA.Models;
using MxA.Services;
using PropertyChanged;
using System.ComponentModel;
using Xamarin.Forms;

namespace MxA.ViewModels {

   [AddINotifyPropertyChangedInterface]
   public class BaseViewModel : INotifyPropertyChanged {

      public IDataStore<Training> DataStore => DependencyService.Get<IDataStore<Training>>();

      public bool IsBusy { get; set; }

      public string Title { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
