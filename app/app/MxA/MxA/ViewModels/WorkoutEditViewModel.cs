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
   public class WorkoutEditViewModel : BaseViewModel {

      private Progression _progression;
      private IEnumerable<WorkoutRef> _workoutRefs;
      private IEnumerable<Activity> _activities;

      #region commands
      public ICommand SelectProgressionCommand { get; private set; }
      public ICommand ExitCommand { get; private set; }
      public ICommand AddActivityCommand { get; private set; }
      public ICommand SaveCommand { get; private set; }
      public ICommand CanelCommand { get; private set; }
      public ICommand EditActivityCommand { get; private set; }
      public ICommand DeleteActivityCommand { get; private set; }

      #endregion

      #region properties
      public string WorkoutId { get; set; }
      public Workout Workout { get; set; }
      public ObservableCollection<WorkoutRef> WorkoutRefs { get; } = new ObservableCollection<WorkoutRef>();
      public ObservableCollection<ActivityExercise> Activities { get; } = new ObservableCollection<ActivityExercise>();
      #endregion

      #region constructors

      public WorkoutEditViewModel() {
         Title = "Workout";
         SelectProgressionCommand = new Command<WorkoutRef>(OnProgressionSelected);
         ExitCommand = new Command(OnExitCommand);
         AddActivityCommand = new Command(OnAddActivityCommand);
         SaveCommand = new Command(OnSaveCommand);
         CanelCommand = new Command(OnCanelCommand);
         EditActivityCommand = new Command<ActivityExercise>(OnEditActivityCommand);
         DeleteActivityCommand = new Command<ActivityExercise>(OnDeleteActivityCommand);

         Workout = new Workout() {
            Id = Guid.NewGuid().ToString("N")
         };
      }
      #endregion

      private async void OnAddActivityCommand() {
         var activity = await DataStore.
            Activities.
            AddOrUpdateItemAsync(new Activity() {
               WorkoutId = Workout.Id
            });

         await Shell.Current.GoToAsync($"{nameof(ActivityEditPage)}?{nameof(ActivityEditViewModel.ActivityId)}={activity.Id}");
      }

      private async void OnEditActivityCommand(ActivityExercise item) {
         await Shell.Current.GoToAsync($"{nameof(ActivityEditPage)}?{nameof(ActivityEditViewModel.ActivityId)}={item.Activity.Id}");
      }
      private async void OnDeleteActivityCommand(ActivityExercise item) {
         if (await DisplayAlertAsync("Delete", "Are you sure you want to remote this activity?", "OK", "Cancel")) {
            item.Activity.Active = false;

            await DataStore.Activities.DeleteItemAsync(item.Activity.Id);

            await LoadWorkout();
         }
      }

      private async void OnExitCommand(object obj) {
         await Shell.Current.Navigation.PopToRootAsync();
      }

      private async void OnCanelCommand(object obj) {
         await Shell.Current.Navigation.PopToRootAsync();
      }

      private async void OnSaveCommand(object obj) {
         //TODO: save
         await DataStore.Workouts.AddOrUpdateItemAsync(Workout);

         await Shell.Current.Navigation.PopToRootAsync();
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
               WorkoutRefs.Clear();
               foreach (var w in _workoutRefs.Where(w => w.ProgressionId == _progression.Id)) {
                  WorkoutRefs.Add(w);
               }
            }

            _activities = await DataStore.Activities.GetItemsAsync(a => a.WorkoutId == Workout.Id);
            Activities.Clear();
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