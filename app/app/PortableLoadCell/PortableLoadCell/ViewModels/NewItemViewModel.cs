using PortableLoadCell.Models;
using System;
using Xamarin.Forms;

namespace PortableLoadCell.ViewModels {
   public class NewItemViewModel : BaseViewModel {

      public NewItemViewModel() {
         SaveCommand = new Command(OnSave, ValidateSave);
         CancelCommand = new Command(OnCancel);
         this.PropertyChanged +=
             (_, __) => SaveCommand.ChangeCanExecute();
      }

      private bool ValidateSave() {
         return !String.IsNullOrWhiteSpace(Text)
             && !String.IsNullOrWhiteSpace(Description);
      }

      public string Text { get; set; }

      public string Description { get; set; }

      public Command SaveCommand { get; }
      public Command CancelCommand { get; }

      private async void OnCancel() {
         // This will pop the current page off the navigation stack
         await Shell.Current.GoToAsync("..");
      }

      private async void OnSave() {
         Item newItem = new Item() {
            Id = Guid.NewGuid().ToString(),
            Text = Text,
            Description = Description
         };

         // await DataStore.AddItemAsync(newItem);

         // This will pop the current page off the navigation stack
         await Shell.Current.GoToAsync("..");
      }
   }
}
