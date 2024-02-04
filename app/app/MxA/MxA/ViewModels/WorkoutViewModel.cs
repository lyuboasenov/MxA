using MxA.Models;
using MxA.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MxA.Database.Models;

namespace MxA.ViewModels {

   [QueryProperty(nameof(WorkoutId), nameof(WorkoutId))]
   public class WorkoutViewModel : BaseViewModel {

      #region commands
      public ICommand StartWorkoutCommand { get; private set; }
      public ICommand ExitCommand { get; private set; }
      public ICommand EditCommand { get; private set; }
      public ICommand DeleteCommand { get; private set; }
      #endregion

      #region properties
      public string WorkoutId { get; set; }
      public Workout Workout { get; set; }
      #endregion

      #region constructors

      public WorkoutViewModel() {
         Title = "Workout";
         ExitCommand = new Command(OnExitCommand);
         EditCommand = new Command(OnEditCommand);
         DeleteCommand = new Command(OnDeleteCommand);
         StartWorkoutCommand = new Command(OnStartWorkoutCommand);
      }
      #endregion

      private async void OnStartWorkoutCommand() {
         await Shell.Current.GoToAsync($"//{nameof(TimerPage)}?{nameof(TimerViewModel.WorkoutId)}={WorkoutId}");
      }

      private async void OnExitCommand(object obj) {
         await Shell.Current.GoToAsync($"//{nameof(TrainingsPage)}");
      }

      private async void OnEditCommand(object obj) {
         await Shell.Current.GoToAsync($"//{nameof(TrainingsPage)}/{nameof(WorkoutEditPage)}?{nameof(WorkoutEditViewModel.WorkoutId)}={Workout.Id}");
      }

      private async void OnDeleteCommand(object obj) {
         if (await DisplayAlertAsync("Delete", "Are you sure you want to remote this workout?", "OK", "Cancel")) {

            await DataStore.Workouts.DeleteItemAsync(Workout.Id);

            await Shell.Current.Navigation.PopToRootAsync();
         }
      }

      public async void OnWorkoutIdChanged() {
         await LoadWorkout();
      }

      private async Task LoadWorkout() {
         try {
            Workout = await DataStore.Workouts.GetItemAsync(WorkoutId);
            Title = Workout.Name;
         } catch (Exception) {
            Debug.WriteLine("Failed to Load Item");
         }
      }
   }
}