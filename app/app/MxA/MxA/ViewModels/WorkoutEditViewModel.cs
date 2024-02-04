using MxA.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MxA.Database.Models;

namespace MxA.ViewModels {

   [QueryProperty(nameof(WorkoutId), nameof(WorkoutId))]
   public class WorkoutEditViewModel : BaseViewModel {

      #region commands
      public ICommand DeleteCommand { get; private set; }
      public ICommand ExitCommand { get; private set; }
      public ICommand SaveCommand { get; private set; }
      public ICommand CanelCommand { get; private set; }
      #endregion

      #region properties
      public string WorkoutId { get; set; }
      public Workout Workout { get; set; }
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
      public uint Sets { get; set; }
      public uint Reps { get; set; }

      #endregion

      #region constructors

      public WorkoutEditViewModel() {
         Title = "Workout";
         DeleteCommand = new Command(OnDeleteCommand);
         ExitCommand = new Command(OnExitCommand);
         SaveCommand = new Command(OnSaveCommand);
         CanelCommand = new Command(OnCanelCommand);

         Workout = new Workout() {
            // Id = Guid.NewGuid().ToString("N")
         };
      }
      #endregion

      private async void OnExitCommand(object obj) {
         await Shell.Current.GoToAsync($"//{nameof(TrainingsPage)}/{nameof(WorkoutPage)}?{nameof(WorkoutViewModel.WorkoutId)}={Workout.Id}");
      }

      private void OnCanelCommand(object obj) {
         ExitCommand?.Execute(obj);
      }

      private void OnDeleteCommand(object obj) {
         throw new NotImplementedException();
      }

      private async void OnSaveCommand(object obj) {
         //TODO: save
         Workout.Prep = PrepMinutes * 60 + PrepSeconds;
         Workout.Work = WorkMinutes * 60 + WorkSeconds;
         Workout.RepRest = RestMinutes * 60 + RestSeconds;
         Workout.SetRest = SetRestMinutes * 60 + SetRestSeconds;
         Workout.CoolDown = CoolDownMinutes * 60 + CoolDownSeconds;
         Workout.Sets = Sets;
         Workout.Reps = Reps;

         await DataStore.Workouts.AddOrUpdateItemAsync(Workout);

         ExitCommand?.Execute(obj);
      }

      public async void OnWorkoutIdChanged() {
         await LoadWorkout();
      }

      private async Task LoadWorkout() {
         try {
            Workout = await DataStore.Workouts.GetItemAsync(WorkoutId);
            Title = Workout.Name;
            PrepMinutes = Workout.Prep / 60;
            PrepSeconds = Workout.Prep * 60;
            WorkMinutes = Workout.Work / 60;
            WorkSeconds = Workout.Work * 60;
            RestMinutes = Workout.RepRest / 60;
            RestSeconds = Workout.RepRest * 60;
            SetRestMinutes = Workout.SetRest / 60;
            SetRestSeconds = Workout.SetRest * 60;
            CoolDownMinutes = Workout.CoolDown / 60;
            CoolDownSeconds = Workout.CoolDown * 60;
            Reps = Workout.Reps;
            Sets = Workout.Sets;

         } catch (Exception) {
            Debug.WriteLine("Failed to Load Item");
         }
      }
   }
}