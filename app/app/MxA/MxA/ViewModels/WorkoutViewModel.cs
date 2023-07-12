using MxA.Models;
using MxA.Views;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
      public ICommand EditCommand { get; private set; }
      public ICommand DeleteCommand { get; private set; }
      public ICommand SelectActivityCommand { get; private set; }

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
         EditCommand = new Command(OnEditCommand);
         DeleteCommand = new Command(OnDeleteCommand);
         SelectActivityCommand = new Command<ActivityExercise>(OnSelectActivityCommand);
      }
      #endregion

      private async void OnSelectActivityCommand(ActivityExercise item) {
         await Shell.Current.GoToAsync($"//{nameof(TimerPage)}?{nameof(TimerViewModel.ActivityId)}={item.Activity.Id}");
      }

      private async void OnExitCommand(object obj) {
         await Shell.Current.Navigation.PopToRootAsync();
      }

      private async void OnEditCommand(object obj) {
         await Shell.Current.GoToAsync($"{nameof(WorkoutEditPage)}?{nameof(WorkoutEditViewModel.WorkoutId)}={Workout.Id}");
      }

      private async void OnDeleteCommand(object obj) {
         if (await DisplayAlertAsync("Delete", "Are you sure you want to remote this workout?", "OK", "Cancel")) {
            Workout.Active = false;
            Workout.Updated = DateTime.Now;

            await DataStore.Workouts.UpdateItemAsync(Workout);

            await Shell.Current.Navigation.PopToRootAsync();
         }
      }

      private async void OnProgressionSelected(WorkoutRef obj) {
         await Shell.Current.GoToAsync($"{nameof(WorkoutPage)}?{nameof(WorkoutViewModel.WorkoutId)}={obj.WorkoutId}");
      }

      public async void OnWorkoutIdChanged() {
         await LoadWorkout();
      }

      private async Task LoadWorkout() {
         try {
            Workout = await DataStore.Workouts.GetItemAsync(WorkoutId);
            Title = Workout.Name;

            if (!string.IsNullOrEmpty(Workout.ProgressionId)) {
               _progression = await DataStore.Progression.GetItemAsync(Workout.ProgressionId);

               _workoutRefs = await DataStore.WorkoutRefs.GetItemsAsync();
               foreach(var w in _workoutRefs.Where(w => w.ProgressionId == _progression.Id)) {
                  WorkoutRefs.Add(w);
               }
            }

            _activities = await DataStore.Activities.GetItemsAsync(a => a.WorkoutId == Workout.Id);

            foreach (var a in _activities) {
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