using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.SimpleAudioPlayer;
using MxA.Services;
using MxA.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using MxA.Database.Models;
using System.Collections.Generic;
using MxA.Models;
using System.Collections.Concurrent;
using System.Linq;
using Newtonsoft.Json;

namespace MxA.ViewModels {

   [QueryProperty(nameof(ActivityId), nameof(ActivityId))]
   public class TimerViewModel : BaseViewModel {

      #region members
      private static ISimpleAudioPlayer _tone1;
      private static ISimpleAudioPlayer _tone2;
      private static ISimpleAudioPlayer _tone3;

      private static ImageSource _btImage;
      private static ImageSource _btConnectedImage;

      private IDevice _ble;
      private TimerStateMachine _timerSM;
      private bool _playSound;
      private Activity _activity;
      private Exercise _exercise;
      private ConcurrentBag<TimerEvent> _timerEvents = new ConcurrentBag<TimerEvent>();
      private bool _timerDoneExecuted = false;
      private uint _timerEventCounter;
      private TimerStateMachine.TimerState _currentState = TimerStateMachine.TimerState.Preparation;
      private TimerStateMachine.TimerState _previousState = TimerStateMachine.TimerState.Preparation;
      private uint _currentStateMaxCounter = 9;
      #endregion

      #region commands
      public ICommand PlayPauseCommand { get; private set; }
      public ICommand ExitCommand { get; private set; }
      public ICommand PrevRepCommand { get; private set; }
      public ICommand NextRepCommand { get; private set; }
      public ICommand PrevSetCommand { get; private set; }
      public ICommand NextSetCommand { get; private set; }
      public ICommand ConnectHangboardCommand { get; private set; }
      public ICommand CompleteTimerCommand { get; private set; }
      public ICommand CancelTimerCommand { get; private set; }
      #endregion

      #region properties
      public bool IsRunning { get; set; }
      public uint Rep { get; set; }
      public uint TotalReps { get; set; }
      public uint Set { get; set; }
      public uint TotalSets { get; set; }
      public uint Counter { get; set; }
      public double Load { get; set; }
      public uint BatteryLevel { get; set; }
      public bool BleConnected { get; set; } = false;
      public float RepsProgress { get; set; }
      public float SetsProgress { get; set; }
      public Color Color { get; set; }
      public Color NextColor { get; set; }
      public bool ISNextVisible { get; set; }
      public string ActivityId { get; set; }
      public string TrainingName { get; set; }
      public string NextPeriod { get; set; }
      public uint NextPeriodTime { get; set; }
      public string PlayPauseGlyph { get; set; }
      public string ConnectHangboardGlyph { get; set; }
      #endregion

      #region constructors
      static TimerViewModel() {
         LoadImagesAndSounds();
      }

      public TimerViewModel() {
         InitializeCommands();

         Title = "Timer";
         PlayPauseGlyph = Icons.Material.IconFont.Play_arrow;
         ConnectHangboardGlyph = Icons.Material.IconFont.Bluetooth;
      }
      #endregion

      private void PlayTones() {
         if (Counter == 0) {
            _tone3.Play();
         } else if (Counter <= 3) {
            _tone1.Play();
         } else if (Counter <= 6) {
            _tone2.Play();
         }
      }

      public async void OnActivityIdChanged() {
         await LoadActivity();
      }

      public void OnIsRunningChanged() {
         if (IsRunning) {
            PlayPauseGlyph = Icons.Material.IconFont.Pause;
         } else {
            PlayPauseGlyph = Icons.Material.IconFont.Play_arrow;
         }
      }

      public async void ConnectBleDevice() {
         try {
            if (Preferences.ContainsKey("BLE_ADDRESS")) {
               var bleAddress = Preferences.Get("BLE_ADDRESS", "");
               if (!string.IsNullOrEmpty(bleAddress)) {
                  _ble = await CrossBluetoothLE.Current.Adapter.
                     ConnectToKnownDeviceAsync(Guid.Parse(bleAddress));

                  var loadService = await _ble.GetServiceAsync(Guid.Parse("0000181d-0000-1000-8000-00805f9b34fb"));
                  if (loadService != null) {
                     var characteristic = await loadService.GetCharacteristicAsync(Guid.Parse("00002a98-0000-1000-8000-00805f9b34fb"));
                     if (characteristic != null) {
                        characteristic.ValueUpdated += Load_ValueUpdated;
                        await characteristic.StartUpdatesAsync();
                     }
                  }

                  var batteryService = await _ble.GetServiceAsync(Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"));
                  if (batteryService != null) {
                     var characteristic = await batteryService.GetCharacteristicAsync(Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"));
                     if (characteristic != null) {
                        var batteryLevel = await characteristic.ReadAsync();

                        var uIntValue = BitConverter.ToUInt16(batteryLevel, 0);
                        Debug.Write($"Value: {uIntValue}");
                        BatteryLevel = (uint) uIntValue;
                        BleConnected = true;
                     }
                  }


                  ConnectHangboardGlyph = Icons.Material.IconFont.Bluetooth_connected;
               }
            }
         } catch (Exception ex) {
            await HandleExceptionAsync("Connect ble device", ex);
         }
      }

      private void Load_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e) {
         var doubleValue = BitConverter.ToDouble(e.Characteristic.Value, 0);
         Debug.Write($"Value: {doubleValue}");
         Load = doubleValue >= 0 ? doubleValue : 0;
      }

      public void OnAppearing() {
         DeviceDisplay.KeepScreenOn = true;
         ConnectBleDevice();
      }

      public void OnDisappearing() {
         DeviceDisplay.KeepScreenOn = false;
      }

      private static void LoadImagesAndSounds() {
         var assembly = typeof(App).GetTypeInfo().Assembly;

         _btImage = ImageSource.FromResource("MxA.Resources.icons.bluetooth_FILL0_wght400_GRAD0_opsz48.png", typeof(TimerViewModel).GetTypeInfo().Assembly);
         _btConnectedImage = ImageSource.FromResource("MxA.Resources.icons.bluetooth_connected_FILL0_wght400_GRAD0_opsz48.png", typeof(TimerViewModel).GetTypeInfo().Assembly);

         _tone1 = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
         _tone2 = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
         _tone3 = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();

         Stream tone1Stream = assembly.GetManifestResourceStream("MxA.Resources.sounds.countdown.wav");
         Stream tone3Stream = assembly.GetManifestResourceStream("MxA.Resources.sounds.end_rep.mp3");
         Stream tone2Stream = assembly.GetManifestResourceStream("MxA.Resources.sounds.Tones.ogg");

         _tone1.Load(tone1Stream);
         _tone2.Load(tone2Stream);
         _tone3.Load(tone3Stream);
      }

      private void InitializeCommands() {
         PlayPauseCommand = new Command(OnPlayPauseCommand, CanPlayPause);
         ExitCommand = new Command(OnExitCommand);
         ConnectHangboardCommand = new Command(OnConnectHangboardCommand);

         PrevRepCommand = new Command(OnPrevRepCommand, CanPrevRep);
         NextRepCommand = new Command(OnNextRepCommand, CanNextRep);
         PrevSetCommand = new Command(OnPrevSetCommand, CanPrevSet);
         NextSetCommand = new Command(OnNextSetCommand, CanNextSet);
         CompleteTimerCommand = new Command(async () => await OnCompleteTimerCommand());
         CancelTimerCommand = new Command(async () => await OnCancelTimerCommand());
      }

      private void OnPlayPauseCommand(object obj) {
         _timerSM.PlayPause();
      }

      private bool CanPlayPause(object obj) {
         return _timerSM != null;
      }

      private async void OnExitCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         _timerSM.Dispose();
         _timerSM.StateChanged -= _timerSM_StateChanged;
         _timerSM = null;
         if (null == _activity) {
            await Shell.Current.Navigation.PopToRootAsync();
         } else {
            await Shell.Current.GoToAsync($"{nameof(WorkoutPage)}?{nameof(WorkoutViewModel.WorkoutId)}={_activity.WorkoutId}");
         }
      }

      private async void OnConnectHangboardCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"{nameof(BleDevicesPage)}");
      }

      private void UpdateCommands() {
         ((Command) PlayPauseCommand).ChangeCanExecute();
         ((Command) ExitCommand).ChangeCanExecute();
         ((Command) ConnectHangboardCommand).ChangeCanExecute();

         ((Command) PrevRepCommand).ChangeCanExecute();
         ((Command) NextRepCommand).ChangeCanExecute();
         ((Command) PrevSetCommand).ChangeCanExecute();
         ((Command) NextSetCommand).ChangeCanExecute();
         ((Command) NextSetCommand).ChangeCanExecute();
         ((Command) CompleteTimerCommand).ChangeCanExecute();
         ((Command) CancelTimerCommand).ChangeCanExecute();
      }

      private bool CanNextSet(object obj) {
         return _timerSM != null &&
            _timerSM.CurrentSet < _timerSM.TotalSets;
      }

      private bool CanPrevSet(object obj) {
         return _timerSM != null &&
            (_timerSM.CurrentRepetition > 0 || _timerSM.CurrentSet > 0);
      }

      private bool CanNextRep(object obj) {
         return _timerSM != null &&
            (_timerSM.CurrentSet < _timerSM.TotalSets ||
            _timerSM.CurrentRepetition < _timerSM.TotalRepetitions);
      }

      private bool CanPrevRep(object obj) {
         return _timerSM != null &&
            (_timerSM.CurrentRepetition > 0 || _timerSM.CurrentSet > 0);
      }

      private void OnPrevRepCommand(object obj) {
         _timerSM.PreviousRepetition();
      }

      private void OnNextRepCommand(object obj) {
         _timerSM.NextRepetition();
      }

      private void OnPrevSetCommand(object obj) {
         _timerSM.PreviousSet();

      }

      private void OnNextSetCommand(object obj) {
         _timerSM.NextSet();
      }

      private async Task OnCompleteTimerCommand() {
         if (!_timerDoneExecuted) {
            _timerDoneExecuted = true;

            if (IsRunning) {
               PlayPauseCommand?.Execute(null);
            }

            var note = await DisplayPromptAsync("Save log", "Note");
            
            if (note != null) {
               var dict = new Dictionary<string, object>();
               dict.Add("work", _activity.Work);
               dict.Add("rep_rest", _activity.RestBWReps);
               dict.Add("set_rest", _activity.RestBWSets);
               dict.Add("reps", _activity.Reps);
               dict.Add("sets", _activity.Sets);

               var added = await DataStore.ActivityLogs.AddItemAsync(new ActivityLog() {
                  ActivityId = _activity.Id,
                  ActivityName = _exercise.Name,
                  ActivityDetailsJson = JsonConvert.SerializeObject(dict),
                  Note = note,
                  Created = DateTime.Now
               });

               foreach (var e in _timerEvents) {
                  e.ActivityLogId = added.Id;
                  await DataStore.TimerEvents.AddItemAsync(e);
               }

               ExitCommand?.Execute(null);
            }
         }
      }

      private async Task OnCancelTimerCommand() {
         if (IsRunning) {
            PlayPauseCommand?.Execute(null);
         }

         if (await DisplayAlertAsync("Cancel", "Are you sure you want to cancel the timer?", "Yes", "No")) {
            ExitCommand?.Execute(null);
         }
      }

      private async Task LoadActivity() {
         try {
            _activity = await DataStore.Activities.GetItemAsync(ActivityId);
            _exercise = await DataStore.Exercises.GetItemAsync(_activity.ExerciseId);

            _timerSM = new TimerStateMachine(
               _activity.Prep,
               _activity.Work,
               _activity.RestBWReps,
               _activity.RestBWSets,
               _activity.Reps,
               _activity.Sets,
               _activity.SkipLastRepRest,
               true);
            _timerSM.StateChanged += _timerSM_StateChanged;
            _timerSM_StateChanged(this, EventArgs.Empty);

            TotalReps = _activity.Reps;
            TotalSets = _activity.Sets;
            TrainingName = _exercise.Name;
            Title = _exercise.Name;

            SetCurrentPeriod();
            UpdateCommands();
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
      }

      private void _timerSM_StateChanged(object sender, EventArgs e) {
         SetCurrentPeriod();
         if (_playSound) {
            PlayTones();
         }
         UpdateCommands();
         if (_currentState != _timerSM.State) {
            _previousState = _currentState;
            _currentState = _timerSM.State;
            _currentStateMaxCounter = (uint) _timerSM.Counter;
         }
         LogTimerEvent();

         if (_timerSM.State == TimerStateMachine.TimerState.Done) {
            MainThread.BeginInvokeOnMainThread(() => {
               CompleteTimerCommand.Execute(null);
            });

         }
      }

      private void LogTimerEvent() {
         if (_timerSM.IsRunning &&
            (_timerSM.State == TimerStateMachine.TimerState.Work ||
            (_timerSM.NextState == TimerStateMachine.TimerState.Work && _timerSM.Counter < 2) ||
            (_previousState == TimerStateMachine.TimerState.Work && (_currentStateMaxCounter - _timerSM.Counter) < 2 ))) {
            _timerEvents.Add(
               new TimerEvent() {
               Counter = (uint) _timerSM.Counter,
               Load = Load,
               State = _timerSM.State,
               Repetition = _timerSM.CurrentRepetition,
               Set = _timerSM.CurrentSet,
               SubCounter = (uint) _timerSM.SubCounter,
               Order = _timerEventCounter++});
         }
      }

      private void SetCurrentPeriod() {
         IsRunning = _timerSM.IsRunning;
         Rep = _timerSM.CurrentRepetition + 1;
         Set = _timerSM.CurrentSet + 1;

         _playSound = Counter != (uint) _timerSM.Counter;

         Counter = (uint) _timerSM.Counter;
         Color = StateToColor(_timerSM.State);

         RepsProgress = (float) Rep / TotalReps;
         SetsProgress = (float) Set / TotalSets;

         if (_timerSM.NextState != null) {
            NextColor = StateToColor(_timerSM.NextState.Value);
            NextPeriod = StateToName(_timerSM.NextState.Value);
            NextPeriodTime = (uint) _timerSM.NextCounter.Value;
            ISNextVisible = true;
         } else {
            NextColor = Color.White;
            NextPeriod = "";
            NextPeriodTime = 0;
            ISNextVisible = false;
         }
      }

      private string StateToName(TimerStateMachine.TimerState state) {
         switch (state) {
            case TimerStateMachine.TimerState.Preparation:
               return "Prepare";
            case TimerStateMachine.TimerState.RepetitionRest:
            case TimerStateMachine.TimerState.SetRest:
               return "Rest";
            case TimerStateMachine.TimerState.Work:
               return "Work";
            case TimerStateMachine.TimerState.Done:
               return "Done";
            default:
               return "";
         }
      }

      private Color StateToColor(TimerStateMachine.TimerState state) {
         switch (state) {
            case TimerStateMachine.TimerState.Preparation:
               return Color.Orange;
            case TimerStateMachine.TimerState.RepetitionRest:
            case TimerStateMachine.TimerState.SetRest:
               return Color.Blue;
            case TimerStateMachine.TimerState.Work:
               return Color.Green;
            case TimerStateMachine.TimerState.Done:
               return Color.Gray;
            default:
               return Color.White;
         }
      }
   }
}