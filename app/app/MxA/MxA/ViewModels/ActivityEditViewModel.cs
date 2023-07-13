using MxA.Database.Models;
using MxA.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MxA.ViewModels {
   [QueryProperty(nameof(ActivityId), nameof(ActivityId))]
   public class ActivityEditViewModel : BaseViewModel {
      private bool _exercisesLoaded = false;
      public string ActivityId { get; set; }
      public ObservableCollection<Exercise> Exercises { get; set; } = new ObservableCollection<Exercise>();
      public Exercise Exercise { get; set; }
      public Activity Activity { get; set; }

      public uint PrepMinutes { get; set; }
      public uint PrepSeconds { get; set; }
      public uint WorkMinutes { get; set; }
      public uint WorkSeconds { get; set; }
      public uint RestMinutes { get; set; }
      public uint RestSeconds { get; set; }
      public uint SetRestMinutes { get; set; }
      public uint SetRestSeconds { get; set; }
      public uint Reps { get; set; }
      public uint Sets { get; set; }

      public ICommand LoadExercisesCommand { get; }
      public ICommand ExitCommand { get; private set; }
      public ICommand SaveCommand { get; private set; }
      public ICommand CanelCommand { get; private set; }

      public ActivityEditViewModel() {
         LoadExercisesCommand = new Command(async () => await OnLoadExercisesCommand());
         ExitCommand = new Command(OnExitCommand);
         SaveCommand = new Command(OnSaveCommand);
         CanelCommand = new Command(OnCanelCommand);

         LoadExercisesCommand?.Execute(null);
      }

      public void OnExerciseChanged() {
         Activity.ExerciseId = Exercise?.Id;
      }
      public void OnRepsChanged() {
         Activity.Reps = Reps;
      }
      public void OnSetsChanged() {
         Activity.Sets = Sets;
      }
      public void OnPrepMinutesChanged() {
         Activity.Prep = GetSeconds(PrepMinutes, PrepSeconds);
      }
      public void OnPrepSecondsChanged() {
         Activity.Prep = GetSeconds(PrepMinutes, PrepSeconds);
      }
      public void OnWorkMinutesChanged() {
         Activity.Work = GetSeconds(WorkMinutes, WorkSeconds);
      }
      public void OnWorkSecondsChanged() {
         Activity.Work = GetSeconds(WorkMinutes, WorkSeconds);
      }

      public void OnRestMinutesChanged() {
         Activity.RestBWReps = GetSeconds(RestMinutes, RestSeconds);
      }
      public void OnRestSecondsChanged() {
         Activity.RestBWReps = GetSeconds(RestMinutes, RestSeconds);
      }
      public void OnSetRestMinutesChanged() {
         Activity.RestBWSets = GetSeconds(SetRestMinutes, SetRestSeconds);
      }
      public void OnSetRestSecondsChanged() {
         Activity.RestBWSets = GetSeconds(SetRestMinutes, SetRestSeconds);
      }

      private uint GetSeconds(uint minutes, uint seconds) {
         return minutes * 60 + seconds;
      }

      private async void OnExitCommand(object obj) {
         await Shell.Current.GoToAsync($"//{nameof(TrainingsPage)}/{nameof(WorkoutEditPage)}?{nameof(WorkoutViewModel.WorkoutId)}={Activity.WorkoutId}");
      }

      private void OnCanelCommand(object obj) {
         ExitCommand?.Execute(null);
      }

      private async void OnSaveCommand(object obj) {
         await DataStore.Activities.AddOrUpdateItemAsync(Activity);

         ExitCommand?.Execute(null);
      }

      private async Task OnLoadExercisesCommand() {
         _exercisesLoaded = false;
         var exercises = await DataStore.Exercises.GetItemsAsync();
         Exercises.Clear();
         foreach(var ex in exercises) {
            Exercises.Add(ex);
         }
         _exercisesLoaded = true;
      }

      public async void OnActivityIdChanged() {
         await LoadActivity();
      }

      private async Task LoadActivity() {
         try {
            while (!_exercisesLoaded) { await Task.Delay(100); }
            Activity = await DataStore.Activities.GetItemAsync(ActivityId);
            Exercise = Exercises.FirstOrDefault(e => e.Id == Activity.ExerciseId);

            Title = Exercise?.Name;

            PrepMinutes = Activity.Prep / 60;
            PrepSeconds = Activity.Prep % 60;

            WorkMinutes = Activity.Work / 60;
            WorkSeconds = Activity.Work % 60;

            RestMinutes = Activity.RestBWReps / 60;
            RestSeconds = Activity.RestBWReps % 60;

            SetRestMinutes = Activity.RestBWSets / 60;
            SetRestSeconds = Activity.RestBWSets % 60;

            Reps = Activity.Reps;
            Sets = Activity.Sets;

         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
      }


   }
}
