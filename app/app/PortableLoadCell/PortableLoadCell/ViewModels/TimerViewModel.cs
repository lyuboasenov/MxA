using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.SimpleAudioPlayer;
using PortableLoadCell.Models;
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

      private Period[] _periods;

      private int _currentPeriod = -1;
      private uint _currentTime;

      private IDevice _ble;

      private Timer Timer { get; set; }
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
         InitializeTimer();

         InitializeCommands();

         PlayPauseImageSource = _playImage;
         ConnectHangboardImageSource = _btImage;
      }
      #endregion

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

            PlayTones();
         }
      }

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
         _currentPeriod++;
         Rep = _periods[_currentPeriod].Rep;
         Set = _periods[_currentPeriod].Set;
         Counter = _periods[_currentPeriod].To - _currentTime;
         Color = _periods[_currentPeriod].Color;

         RepsProgress = (float) _periods[_currentPeriod].Rep / TotalReps;
         SetsProgress = (float) _periods[_currentPeriod].Rep / TotalSets;

         if (_periods?.Length > _currentPeriod + 1) {
            NextColor = _periods[_currentPeriod + 1].Color;
            NextPeriod = _periods[_currentPeriod + 1].Name;
            NextPeriodTime = _periods[_currentPeriod + 1].Time;
         } else {
            NextColor = Color.AliceBlue;
            NextPeriod = "";
            NextPeriodTime = 0;
         }
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

      private void InitializeTimer() {
         Title = "Timer";
         Timer = new Timer(1000);
         Timer.Elapsed += Timer_Elapsed;
         Timer.Start();
      }

      private void InitializeCommands() {
         PlayPauseCommand = new Command(OnPlayPauseCommand);
         ExitCommand = new Command(OnExitCommand);
         ConnectHangboardCommand = new Command(OnConnectHangboardCommand);

         PrevRepCommand = new Command(OnPrevRepCommand);
         NextRepCommand = new Command(OnNextRepCommand);
         PrevSetCommand = new Command(OnPrevSetCommand);
         NextSetCommand = new Command(OnNextSetCommand);
      }

      private void OnPlayPauseCommand(object obj) {
         IsRunning = !IsRunning;
      }

      private async void OnExitCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"//");
      }

      private async void OnConnectHangboardCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"{nameof(BleDevicesPage)}");
      }

      private void OnPrevRepCommand(object obj) {
         _currentTime -= 2;
      }

      private void OnNextRepCommand(object obj) {
         _currentTime += 1;
      }

      private void OnPrevSetCommand(object obj) {
         IsRunning = false;
         _currentPeriod = Math.Max(-1, _currentPeriod - 1);
         MoveNextPeriod();
      }

      private void OnNextSetCommand(object obj) {
         IsRunning = false;
         _currentPeriod = Math.Min((_periods?.Length ?? 0) - 1, _currentPeriod + 1);
         MoveNextPeriod();
      }

      private async Task LoadTraining() {
         try {
            var item = await DataStore.GetItemAsync(TrainingId);
            TotalReps = item.Reps;
            TotalSets = item.Sets;
            TrainingName = item.Name;

            _periods = item.Expand().ToArray();
            if (_periods?.Length > _currentPeriod + 1) {
               NextColor = _periods[_currentPeriod + 1].Color;
               NextPeriod = _periods[_currentPeriod + 1].Name;
               NextPeriodTime = _periods[_currentPeriod + 1].Time;
            }
         } catch (Exception) {
            Debug.WriteLine("Failed to Load Item");
         }
      }
   }
}