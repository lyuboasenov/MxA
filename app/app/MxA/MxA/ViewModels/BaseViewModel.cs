using MxA.Database.Services;
using MxA.Models;
using MxA.Services;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MxA.ViewModels {

   [AddINotifyPropertyChangedInterface]
   public class BaseViewModel : INotifyPropertyChanged {
      public IDataStore DataStore => DependencyService.Get<IDataStore>();
      public bool IsBusy { get; set; }
      public string Title { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;

      public static Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "") {
         return App.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
      }
      public static async Task DisplayAlertAsync(string title, string message, string cancel) {
         await App.Current.MainPage.DisplayAlert(title, message, cancel);
      }
      public static Task HandleExceptionAsync(Exception ex) {
         return HandleExceptionAsync(string.Empty, ex);
      }
      public static async Task HandleExceptionAsync(string message, Exception ex) {
         var title = string.IsNullOrEmpty(message) ? "Error" : $"Error: {message}";
         await DisplayAlertAsync(title, FormatException(ex), "Fine");
      }

      public static string FormatException(Exception ex) {
         return ex.Message;
      }
   }
}
