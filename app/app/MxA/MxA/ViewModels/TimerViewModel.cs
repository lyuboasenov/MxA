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
      private TrainingWorker _trainingWorker;
      #endregion

      #region commands
      public ICommand PlayPauseCommand { get; private set; }
      public ICommand ExitCommand { get; private set; }
      public ICommand PrevRepCommand { get; private set; }
      public ICommand NextRepCommand { get; private set; }
      public ICommand PrevSetCommand { get; private set; }
      public ICommand NextSetCommand { get; private set; }
      public ICommand ConnectHangboardCommand { get; private set; }
      #endregion
      #region properties
      public bool IsRunning { get; set; }
      public uint Rep { get; set; }
      public uint TotalReps { get; set; }
      public uint Set { get; set; }
      public uint TotalSets { get; set; }
      public uint Counter { get; set; }
      public uint Load { get; set; }
      public uint BatteryLevel { get; set; }
      public bool BleConnected { get; set; } = false;
      public float RepsProgress { get; set; }
      public float SetsProgress { get; set; }
      public Color Color { get; set; }
      public Color NextColor { get; set; }
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

      private void MoveNextPeriod() {
         //if (_periods?.Length > 0 && _currentPeriod + 1 < _periods?.Length) {
         //   _currentPeriod++;
         //   SetCurrentPeriod();
         //}
      }

      private void MovePrevPeriod() {
         //if (_periods?.Length > 0 && _currentPeriod - 1 >= 0) {
         //   _currentPeriod--;
         //   SetCurrentPeriod();
         //}
      }

      private void MovePrevSet() {
         //if (_periods?.Length > 0) {
         //   var currentSet = _periods[_currentPeriod].Set;

         //   while (_currentPeriod > 0 && _periods[_currentPeriod--].Set > currentSet - 2) { }
         //   _currentPeriod++;

         //   SetCurrentPeriod();
         //}
      }

      private void MoveNextSet() {
         //if (_periods?.Length > 0) {
         //   var currentSet = _periods[_currentPeriod].Set;

         //   while (_currentPeriod + 1 < _periods.Length && _periods[_currentPeriod++].Set < currentSet + 1) { }

         //   SetCurrentPeriod();
         //}
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
         Load = (uint) doubleValue;
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
      }

      private void OnPlayPauseCommand(object obj) {
         IsRunning = !IsRunning;
         _trainingWorker.PlayPause();
      }

      private bool CanPlayPause(object obj) {
         // throw new NotImplementedException();
         return true;
      }

      private async void OnExitCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         _trainingWorker.Dispose();
         _trainingWorker.WorkerChanged -= _trainingWorker_WorkerChanged;
         _trainingWorker = null;
         await Shell.Current.GoToAsync("..");
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
      }

      private bool CanNextSet(object obj) {
         return _trainingWorker?.CanNextSet ?? false;
      }

      private bool CanPrevSet(object obj) {
         return _trainingWorker?.CanPrevSet ?? false;
      }

      private bool CanNextRep(object obj) {
         return _trainingWorker?.CanNextRep ?? false;
      }

      private bool CanPrevRep(object obj) {
         return _trainingWorker?.CanPrevRep ?? false;
      }

      private void OnPrevRepCommand(object obj) {
         _trainingWorker.PrevRep();
      }

      private void OnNextRepCommand(object obj) {
         _trainingWorker.NextRep();
      }

      private void OnPrevSetCommand(object obj) {
         _trainingWorker.PrevSet();

      }

      private void OnNextSetCommand(object obj) {
         _trainingWorker.NextSet();
      }

      private async Task LoadActivity() {
         try {
            var activity = await DataStore.Activities.GetItemAsync(ActivityId);
            var exercise = await DataStore.Exercises.GetItemAsync(activity.ExerciseId);
            _trainingWorker = new TrainingWorker(activity);
            _trainingWorker.WorkerChanged += _trainingWorker_WorkerChanged;

            TotalReps = activity.Reps;
            TotalSets = activity.Sets;
            TrainingName = exercise.Name;
            Title = exercise.Name;

            SetCurrentPeriod();
            UpdateCommands();
         } catch (Exception) {
            Debug.WriteLine("Failed to Load Item");
         }
      }

      private void _trainingWorker_WorkerChanged(object sender, EventArgs e) {
         SetCurrentPeriod();
         PlayTones();
         UpdateCommands();
      }

      private void SetCurrentPeriod() {
         IsRunning = _trainingWorker.IsRunning;
         Rep = _trainingWorker.CurrentPeriod?.Rep ?? 0;
         Set = _trainingWorker.CurrentPeriod?.Set ?? 0;
         Counter = (uint) _trainingWorker.Counter;
         Color = _trainingWorker.CurrentPeriod?.Color ?? Xamarin.Forms.Color.White;

         RepsProgress = (float) Rep / TotalReps;
         SetsProgress = (float) Set / TotalSets;

         if (_trainingWorker.NextPeriod != null) {
            NextColor = _trainingWorker.NextPeriod.Color;
            NextPeriod = _trainingWorker.NextPeriod.Name;
            NextPeriodTime = _trainingWorker.NextPeriod.Time;
         } else {
            NextColor = Color.White;
            NextPeriod = "";
            NextPeriodTime = 0;
         }
      }
   }
}