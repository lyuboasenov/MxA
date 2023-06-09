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
         return !String.IsNullOrWhiteSpace(Name)
             && (PrepMinutes * 60 + PrepSeconds) > 0
             && (WorkMinutes * 60 + WorkSeconds) > 0
             && (RestMinutes * 60 + RestSeconds) > 0
             && (SetRestMinutes * 60 + SetRestSeconds) > 0
             && (CoolDownMinutes * 60 + CoolDownSeconds) > 0
             && Reps > 0
             && Sets > 0;
      }

      public string Name { get; set; }
      public uint PrepMinutes { get; set; }
      public uint PrepSeconds { get; set; }
      public uint WorkMinutes { get; set; }
      public uint WorkSeconds { get; set; }
      public uint RestMinutes { get; set; }
      public uint RestSeconds { get; set; }
      public uint SetRestMinutes { get; set; }
      public uint SetRestSeconds { get; set; }
      public uint CoolDownMinutes { get; set; }
      public uint CoolDownSeconds { get; set; }
      public uint Reps { get; set; }
      public uint Sets { get; set; }

      public Command SaveCommand { get; }
      public Command CancelCommand { get; }

      private async void OnCancel() {
         // This will pop the current page off the navigation stack
         await Shell.Current.GoToAsync("..");
      }

      private async void OnSave() {
         SimpleTraining newTraining = new SimpleTraining() {
            Id = Name.
               Replace(" ", "_").
               Replace("\\", "_").
               Replace("/", "_").
               ToLower(),
            Name = Name,
            PrepTime = PrepMinutes * 60 + PrepSeconds,
            WorkTime = WorkMinutes * 60 + WorkSeconds,
            RestTime = RestMinutes * 60 + RestSeconds,
            RestBwSetsTime = SetRestMinutes * 60 + SetRestSeconds,
            CoolDownTime = CoolDownMinutes * 60 + CoolDownSeconds,
            Reps = Reps,
            Sets = Sets
         };

         await DataStore.AddItemAsync(newTraining);

         // This will pop the current page off the navigation stack
         await Shell.Current.GoToAsync("..");
      }
   }
}
