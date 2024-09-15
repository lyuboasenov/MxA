using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MxA.Helpers {
   public class BLEHelper {

      private BLEHelper() {

      }

      private static BLEHelper _instance = new BLEHelper();
      public static BLEHelper Instance => _instance;

      public class LoadValueEventArgs : EventArgs {
         public double Value { get; set; }
      }

      public class BatteryLevelEventArgs : EventArgs {
         public uint BatteryLevel { get; set; }
      }

      public event EventHandler<LoadValueEventArgs> LoadValueEvent;
      public event EventHandler<BatteryLevelEventArgs> BatteryLevelEvent;
      private IDevice _ble;

      public bool IsConnected { get; private set; }
      public uint BatteryLevel { get; private set; }

      public async Task ConnectBleDevice() {
         if (Preferences.ContainsKey("BLE_ADDRESS")) {
            var bleAddress = Preferences.Get("BLE_ADDRESS", "");
            if (!string.IsNullOrEmpty(bleAddress) && _ble?.Id != Guid.Parse(bleAddress)) {
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
                     characteristic.ValueUpdated += BatteryLevel_ValueUpdated;
                     await characteristic.StartUpdatesAsync();
                  }
               }

               IsConnected = true;
            }
         }
      }

      private void BatteryLevel_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e) {
         if (e.Characteristic.Value?.Length > 0) {
            var uIntValue = BitConverter.ToUInt16(e.Characteristic.Value, 0);

            BatteryLevelEvent?.Invoke(this, new BatteryLevelEventArgs() { BatteryLevel = (uint) uIntValue });
            BatteryLevel = uIntValue;
         }
      }
      
      private void Load_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e) {
         if (e.Characteristic.Value?.Length > 0) {
            var doubleValue = BitConverter.ToDouble(e.Characteristic.Value, 0);
            LoadValueEvent?.Invoke(this, new LoadValueEventArgs() { Value = doubleValue >= 0 ? doubleValue : 0 });
         
         }
      }
   }
}
