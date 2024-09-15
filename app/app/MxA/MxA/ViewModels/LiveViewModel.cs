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
using MxA.Helpers;

namespace MxA.ViewModels {
   public class LiveViewModel : BaseViewModel, IDisposable {

      #region members
      private static ImageSource _btImage;
      private static ImageSource _btConnectedImage;

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
            BLEHelper.Instance.LoadValueEvent += Instance_LoadValueEvent;
            BLEHelper.Instance.BatteryLevelEvent += Instance_BatteryLevelEvent;
            await BLEHelper.Instance.ConnectBleDevice();
            BleConnected = BLEHelper.Instance.IsConnected;
            if (BleConnected) {
               ConnectHangboardGlyph = Icons.Material.IconFont.Bluetooth_connected;
            }
         } catch (Exception ex) {
            await HandleExceptionAsync("Connect ble device", ex);
         }
      }

      private void Instance_BatteryLevelEvent(object sender, BLEHelper.BatteryLevelEventArgs e) {
         BatteryLevel = e.BatteryLevel;
      }

      private void Instance_LoadValueEvent(object sender, BLEHelper.LoadValueEventArgs e) {
         Load = e.Value;
         LoadValues.Add(e.Value);
      }

      public void OnAppearing() {
         DeviceDisplay.KeepScreenOn = true;

         InitializeCommands();

         Title = "Live";
         RecordGlyph = Icons.Material.IconFont.Fiber_manual_record;
         ConnectHangboardGlyph = Icons.Material.IconFont.Bluetooth;

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

      private bool disposedValue;

      protected virtual void Dispose(bool disposing) {
         if (!disposedValue) {
            if (disposing) {
               // TODO: dispose managed state (managed objects)
            }

            BLEHelper.Instance.LoadValueEvent -= Instance_LoadValueEvent;
            BLEHelper.Instance.BatteryLevelEvent -= Instance_BatteryLevelEvent;

            disposedValue = true;
         }
      }

      public void Dispose() {
         // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
      }
   }
}