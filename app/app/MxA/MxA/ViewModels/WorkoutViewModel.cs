using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.SimpleAudioPlayer;
using MxA.Models;
using MxA.Services;
using MxA.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using MxA.Database.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MxA.ViewModels {

   [QueryProperty(nameof(WorkoutId), nameof(WorkoutId))]
   public class WorkoutViewModel : BaseViewModel {

      private Progression _progression;
      private IEnumerable<WorkoutRef> _workoutRefs;
      private IEnumerable<Activity> _activities;
      #region commands
      public ICommand SelectProgressionCommand { get; private set; }
      public ICommand ExitCommand { get; private set; }

      #endregion
      #region properties
      public string WorkoutId { get; set; }
      public Workout Workout { get; set; }
      public ObservableCollection<WorkoutRef> WorkoutRefs { get; } = new ObservableCollection<WorkoutRef>();
      public ObservableCollection<ActivityExercise> Activities { get; } = new ObservableCollection<ActivityExercise>();
      #endregion

      #region constructors

      public WorkoutViewModel() {
         Title = "Workout";
         SelectProgressionCommand = new Command<WorkoutRef>(OnProgressionSelected);
         ExitCommand = new Command(OnExitCommand);
      }

      private async void OnExitCommand(object obj) {
         await Shell.Current.GoToAsync($"//{nameof(TrainingPage)}");
      }

      private async void OnProgressionSelected(WorkoutRef obj) {
         await Shell.Current.GoToAsync($"{nameof(WorkoutPage)}?{nameof(WorkoutViewModel.WorkoutId)}={obj.WorkoutId}");
      }
      #endregion

      public async void OnWorkoutIdChanged() {
         await LoadWorkout();
      }

      private async Task LoadWorkout() {
         try {
            Workout = await DataStore.Workouts.GetItemAsync(WorkoutId);
            Title = Workout.Name;

            if (!string.IsNullOrEmpty(Workout.ProgressionId)) {
               _progression = await DataStore.Progression.GetItemAsync(Workout.ProgressionId);
               var progressionWorkoutRefs = await DataStore.ProgressionWorkoutRefs.GetItemsAsync();
               var workoutRefTasks = progressionWorkoutRefs.
                  Where(ww => ww.ProgressionId == Workout.ProgressionId).
                  OrderBy(wr => wr.Order).
                  Select(async (w) => await DataStore.WorkoutRefs.GetItemAsync(w.WorkoutRefId));


               _workoutRefs = await Task.WhenAll(workoutRefTasks);
               foreach(var w in _workoutRefs) {
                  WorkoutRefs.Add(w);
               }
            }

            var activityRefs = await DataStore.
               WorkoutActivities.
               GetItemsAsync();

            var activityTasks = activityRefs.
               Where(ar => ar.WorkoutId == Workout.Id).
               OrderBy(o => o.Order).
               Select(a => DataStore.Activities.GetItemAsync(a.ActivityId));

            _activities = await Task.WhenAll(activityTasks);

            foreach(var a in _activities) {
               var e = await DataStore.Exercises.GetItemAsync(a.ExerciseId);
               Activities.Add(new ActivityExercise() {
                  Activity = a,
                  Exercise = e
               });
            }
         } catch (Exception) {
            Debug.WriteLine("Failed to Load Item");
         }
      }
   }
}