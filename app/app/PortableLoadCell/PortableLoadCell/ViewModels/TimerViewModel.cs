using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.SimpleAudioPlayer;
using PortableLoadCell.Models;
using PortableLoadCell.Services;
using PortableLoadCell.Views;
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

namespace PortableLoadCell.ViewModels {

   [QueryProperty(nameof(TrainingId), nameof(TrainingId))]
   public class TimerViewModel : BaseViewModel {

      #region members
      private static ImageSource _playImage;
      private static ImageSource _pauseImage;

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
      public float RepsProgress { get; set; }
      public float SetsProgress { get; set; }
      public Color Color { get; set; }
      public Color NextColor { get; set; }
      public string TrainingId { get; set; }
      public string TrainingName { get; set; }
      public string NextPeriod { get; set; }
      public uint NextPeriodTime { get; set; }
      public ImageSource PlayPauseImageSource { get; set; }
      public ImageSource ConnectHangboardImageSource { get; set; }
      #endregion

      #region constructors
      static TimerViewModel() {
         LoadImagesAndSounds();
      }

      public TimerViewModel() {
         InitializeCommands();

         Title = "Timer";
         PlayPauseImageSource = _playImage;
         ConnectHangboardImageSource = _btImage;
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

      public async void OnTrainingIdChanged() {
         await LoadTraining();
      }

      public void OnIsRunningChanged() {
         if (IsRunning) {
            PlayPauseImageSource = _pauseImage;
         } else {
            PlayPauseImageSource = _playImage;
         }
      }

      public async void ConnectBleDevice() {
         try {
            if (Preferences.ContainsKey("BLE_ADDRESS")) {
               var bleAddress = Preferences.Get("BLE_ADDRESS", "");
               if (!string.IsNullOrEmpty(bleAddress)) {
                  _ble = await CrossBluetoothLE.Current.Adapter.
                     ConnectToKnownDeviceAsync(Guid.Parse(bleAddress));

                  var service = await _ble.GetServiceAsync(Guid.Parse("0000181d-0000-1000-8000-00805f9b34fb"));
                  if (service != null) {
                     var characteristic = await service.GetCharacteristicAsync(Guid.Parse("00002a98-0000-1000-8000-00805f9b34fb"));
                     if (characteristic != null) {
                        characteristic.ValueUpdated += Characteristic_ValueUpdated;
                        await characteristic.StartUpdatesAsync();
                     }
                  }


                  ConnectHangboardImageSource = _btConnectedImage;
               }
            }
         } catch (Exception) {
            Debug.WriteLine("Connect ble device");
         }
      }

      private void Characteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e) {
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

         _playImage = ImageSource.FromResource("PortableLoadCell.Resources.icons.play_arrow_FILL0_wght400_GRAD0_opsz48.png", typeof(TimerViewModel).GetTypeInfo().Assembly);
         _pauseImage = ImageSource.FromResource("PortableLoadCell.Resources.icons.pause_FILL0_wght400_GRAD0_opsz48.png", typeof(TimerViewModel).GetTypeInfo().Assembly);
         _btImage = ImageSource.FromResource("PortableLoadCell.Resources.icons.bluetooth_FILL0_wght400_GRAD0_opsz48.png", typeof(TimerViewModel).GetTypeInfo().Assembly);
         _btConnectedImage = ImageSource.FromResource("PortableLoadCell.Resources.icons.bluetooth_connected_FILL0_wght400_GRAD0_opsz48.png", typeof(TimerViewModel).GetTypeInfo().Assembly);

         _tone1 = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
         _tone2 = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
         _tone3 = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();

         Stream tone1Stream = assembly.GetManifestResourceStream("PortableLoadCell.Resources.sounds.countdown.wav");
         Stream tone3Stream = assembly.GetManifestResourceStream("PortableLoadCell.Resources.sounds.end_rep.mp3");
         Stream tone2Stream = assembly.GetManifestResourceStream("PortableLoadCell.Resources.sounds.Tones.ogg");

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

      private async Task LoadTraining() {
         try {
            var item = await DataStore.GetItemAsync(TrainingId);
            _trainingWorker = new TrainingWorker(item);
            _trainingWorker.WorkerChanged += _trainingWorker_WorkerChanged;

            TotalReps = item.Reps;
            TotalSets = item.Sets;
            TrainingName = item.Name;

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