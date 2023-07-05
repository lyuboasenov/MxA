using MxA.Database.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MxA.ViewModels {
   [QueryProperty(nameof(ExerciseId), nameof(ExerciseId))]
   public class ExerciseViewModel : BaseViewModel {

      public string ExerciseId { get; set; }

      public Exercise Exercise { get; set; }

      public ObservableCollection<ExerciseFocusPoint> FocusPoints { get; private set; }

      public ICommand LoadExerciseCommand { get; }
      public ICommand ExitCommand { get; private set; }

      public ExerciseViewModel() {
         LoadExerciseCommand = new Command(async () => await LoadExercise() );
         ExitCommand = new Command(OnExitCommand);
      }
      public void OnExerciseIdChanged() {
         LoadExerciseCommand?.Execute(null);

      }

      private async Task LoadExercise() {
         if (string.IsNullOrEmpty(ExerciseId))
            return;

         Exercise = await DataStore.Exercises.GetItemAsync(ExerciseId);
         var focusPoints = await DataStore.ExerciseFocusPoints.GetItemsAsync();
         focusPoints = focusPoints.Where(f => f.ExerciseId == ExerciseId);

         if (focusPoints.Any()) {
            FocusPoints = new ObservableCollection<ExerciseFocusPoint>();
            foreach (var item in focusPoints) {
               FocusPoints.Add(item);
            }
         }
      }

      private async void OnExitCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.Navigation.PopToRootAsync();
      }
   }
}
