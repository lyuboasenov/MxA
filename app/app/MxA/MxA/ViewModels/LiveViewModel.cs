using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using MxA.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using MxA.Database.Models;
using System.Collections.Concurrent;
using System.Timers;
using System.Collections.ObjectModel;
using Microcharts;
using SkiaSharp;
using System.Collections.Generic;

namespace MxA.ViewModels {
   public class LiveViewModel : BaseViewModel {

      #region members
      private static ImageSource _btImage;
      private static ImageSource _btConnectedImage;

      private ConcurrentBag<TimerEvent> _timerEvents = new ConcurrentBag<TimerEvent>();

      private IDevice _ble;
      //private readonly System.Timers.Timer _timer;

      //private List<ChartEntry> _entries;

      #endregion

      #region commands
      public ICommand RecordCommand { get; private set; }
      public ICommand ExitCommand { get; private set; }
      public ICommand ConnectHangboardCommand { get; private set; }
      public ICommand CompleteStreamCommand { get; private set; }
      public ICommand CancelStreamCommand { get; private set; }
      #endregion

      #region properties
      public bool IsRecording { get; set; }
      public uint Counter { get; set; }
      public double Load { get; set; }
      public uint BatteryLevel { get; set; }
      public bool BleConnected { get; set; } = false;
      public string RecordGlyph { get; set; }
      public string ConnectHangboardGlyph { get; set; }
      public ObservableCollection<double> LoadHistory { get; } = new ObservableCollection<double>();
      public LineChart Chart { get; }
      public ObservableCollection<double> LoadValues { get; set; } = new ObservableCollection<double>();
      #endregion

      #region constructors
      static LiveViewModel() {

      }

      public LiveViewModel() {
         InitializeCommands();

         Title = "Live";
         RecordGlyph = Icons.Material.IconFont.Fiber_manual_record;
         ConnectHangboardGlyph = Icons.Material.IconFont.Bluetooth;
      }
      #endregion

      public void OnIsRunningChanged() {
         if (IsRecording) {
            RecordGlyph = Icons.Material.IconFont.Stop;
         } else {
            RecordGlyph = Icons.Material.IconFont.Fiber_manual_record;
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
                        if (batteryLevel?.Length > 0) {
                           var uIntValue = BitConverter.ToUInt16(batteryLevel, 0);
                           BatteryLevel = (uint) uIntValue;
                        }

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
         if (e.Characteristic.Value?.Length > 0) {
            var doubleValue = BitConverter.ToDouble(e.Characteristic.Value, 0);
            Load = doubleValue >= 0 ? doubleValue : 0;
            LoadValues.Add(Load);
            if (LoadValues.Count > 300) { LoadValues.RemoveAt(0); }
         }
      }

      public void OnAppearing() {
         DeviceDisplay.KeepScreenOn = true;
         ConnectBleDevice();
      }

      public void OnDisappearing() {
         DeviceDisplay.KeepScreenOn = false;
      }

      private void InitializeCommands() {
         ExitCommand = new Command(OnExitCommand);
         ConnectHangboardCommand = new Command(OnConnectHangboardCommand);
      }

      private async void OnExitCommand(object obj) {
         await Shell.Current.Navigation.PopToRootAsync();
      }

      private async void OnConnectHangboardCommand(object obj) {
         // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
         await Shell.Current.GoToAsync($"{nameof(BleDevicesPage)}");
      }
   }
}