using PortableLoadCell.Models;
using PortableLoadCell.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace PortableLoadCell.ViewModels {

   [QueryProperty(nameof(TrainingId), nameof(TrainingId))]
   public class TimerViewModel : BaseViewModel {
      private string _trainingId;
      private string _trainingName;
      private bool _isRunning;
      private uint _rep;
      private uint _totalReps;
      private uint _set;
      private uint _totalSets;
      private uint _counter;
      private uint _load;
      private float _repsProgress;
      private float _setsProgress;
      private Color _color;
      private Color _nextColor;
      private string _nextPeriod;
      private uint _nextPeriodTime;
      private List<Period> _periods = new List<Period>();

      private int _currentPeriod = -1;
      private uint _currentTime;

      private Timer Timer { get; set; }

      public TimerViewModel() {
         Title = "Timer";
         Timer = new Timer(1000);
         Timer.Elapsed += Timer_Elapsed;
         Timer.Start();

         PlayPauseCommand = new Command(OnPlayPauseCommand);
         ExitCommand = new Command(OnExitCommand);

         MinusSecondCommand = new Command(OnMinusSecondCommand);
         PlusSecondCommand = new Command(OnPlusSecondCommand);
         PrevSetCommand = new Command(OnPrevExerciseCommand);
         NextSetCommand = new Command(OnNextExerciseCommand);

      }

      private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
         if (IsRunning) {
            if (_currentPeriod < 0) {
               MoveNextPeriod();
            }
            _currentTime++;
            if (_periods[_currentPeriod].To < _currentTime) {
               MoveNextPeriod();
            }
            Counter = _periods[_currentPeriod].To - _currentTime;
         }
      }

      private void MoveNextPeriod() {
         _currentPeriod++;
         Rep = _periods[_currentPeriod].Rep;
         Set = _periods[_currentPeriod].Set;
         Counter = _periods[_currentPeriod].To - _currentTime;
         Color = _periods[_currentPeriod].Color;

         RepsProgress = (float) _periods[_currentPeriod].Rep / TotalReps;
         SetsProgress = (float) _periods[_currentPeriod].Rep / TotalSets;

         if (_periods.Count > _currentPeriod + 1) {
            NextColor = _periods[_currentPeriod + 1].Color;
            NextPeriod = _periods[_currentPeriod + 1].Name;
            NextPeriodTime = _periods[_currentPeriod + 1].Time;
         } else {
            NextColor = Color.AliceBlue;
            NextPeriod = "";
            NextPeriodTime = 0;
         }
      }

      public ICommand PlayPauseCommand { get; }
      public ICommand ExitCommand { get; }
      public ICommand MinusSecondCommand { get; }
      public ICommand PlusSecondCommand { get; }
      public ICommand PrevSetCommand { get; }
      public ICommand NextSetCommand { get; }

      public bool IsRunning {
         get => _isRunning;
         set => SetProperty(ref _isRunning, value);
      }

      public uint Rep { get => _rep; set => SetProperty(ref _rep, value); }
      public uint TotalReps { get => _totalReps; set => SetProperty(ref _totalReps, value); }
      public uint Set { get => _set; set => SetProperty(ref _set, value); }
      public uint TotalSets { get => _totalSets; set => SetProperty(ref _totalSets, value); }
      public uint Counter { get => _counter; set => SetProperty(ref _counter, value); }
      public uint Load { get => _load; set => SetProperty(ref _load, value); }
      public float RepsProgress { get => _repsProgress; set => SetProperty(ref _repsProgress, value); }
      public float SetsProgress { get => _setsProgress; set => SetProperty(ref _setsProgress, value); }
      public Color Color { get => _color; set => SetProperty(ref _color, value); }
      public Color NextColor { get => _nextColor; set => SetProperty(ref _nextColor, value); }
      public string TrainingId { get => _trainingId; set { _trainingId = value; LoadTraining(value); } }
      public string TrainingName { get => _trainingName; set => SetProperty(ref _trainingName, value); }
      public string NextPeriod { get => _nextPeriod; set => SetProperty(ref _nextPeriod, value); }
      public uint NextPeriodTime { get => _nextPeriodTime; set => SetProperty(ref _nextPeriodTime, value); }

      public async void LoadTraining(string itemId) {
         try {
            var item = await DataStore.GetItemAsync(itemId);
            TotalReps = item.Reps;
            TotalSets = item.Sets;
            TrainingName = item.Name;

            ExpandTraining(item);
         } catch (Exception) {
            Debug.WriteLine("Failed to Load Item");
         }
      }

      private void ExpandTraining(Training item) {
         _periods.Clear();
         uint totalTime = 0;
         _periods.Add(new Period {
            Color = Color.Orange,
            Name = "Prepare",
            Time = item.PrepTime,
            From = totalTime,
            To = item.PrepTime,
            Rep = 0,
            Set = 0,
         });
         totalTime += item.PrepTime;

         for (uint set = 0; set < item.Sets; set++) {
            for (uint rep = 0; rep < item.Reps; rep++) {
               _periods.Add(new Period {
                  Color = Color.Green,
                  Name = "Work",
                  Time = item.WorkTime,
                  From = totalTime,
                  To = item.WorkTime + totalTime,
                  Rep = rep + 1,
                  Set = set + 1,
               });
               totalTime += item.WorkTime;

               if (rep < item.Reps - 1) {
                  _periods.Add(new Period {
                     Color = Color.Red,
                     Name = "Rest",
                     Time = item.RestTime,
                     From = totalTime,
                     To = item.RestTime + totalTime,
                     Rep = rep + 1,
                     Set = set + 1,
                  });
                  totalTime += item.RestTime;
               }
            }

            _periods.Add(new Period {
               Color = Color.DarkRed,
               Name = "Long Rest",
               Time = item.RestBwSetsTime,
               From = totalTime,
               To = item.RestBwSetsTime + totalTime,
               Rep = item.Reps,
               Set = set + 1,
            });
            totalTime += item.RestBwSetsTime;
         }

         _periods.Add(new Period {
            Color = Color.Red,
            Name = "Cooldown",
            Time = item.CooldownTime,
            From = totalTime,
            To = item.CooldownTime + totalTime,
            Rep = item.Reps,
            Set = item.Sets,
         });
         totalTime += item.CooldownTime;

         if (_periods.Count > _currentPeriod + 1) {
            NextColor = _periods[_currentPeriod + 1].Color;
            NextPeriod = _periods[_currentPeriod + 1].Name;
            NextPeriodTime = _periods[_currentPeriod + 1].Time;
         }
      }

      private void OnPlayPauseCommand(object obj) {
         IsRunning = !IsRunning;
      }

      private async void OnExitCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"//");
      }

      private async void OnMinusSecondCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
      }

      private async void OnPlusSecondCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
      }

      private async void OnPrevExerciseCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
      }

      private async void OnLoginClicked(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
      }

      private async void OnNextExerciseCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
      }
   }
}