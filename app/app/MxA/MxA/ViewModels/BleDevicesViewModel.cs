using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using MxA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using MxA.Helpers.Permissions;

namespace MxA.ViewModels {
   public class BleDevicesViewModel : BaseViewModel {

      private readonly IAdapter _adapter;
      private List<IDevice> _gattDevices = new List<IDevice>();

      public ObservableCollection<BleDevice> Items { get; }
      public Command ScanCommand { get; }
      public Command<BleDevice> ItemTapped { get; }

      public BleDevicesViewModel() {
         Title = "Browse";
         Items = new ObservableCollection<BleDevice>();
         ScanCommand = new Command(ExecuteScanCommand);

         ItemTapped = new Command<BleDevice>(OnItemSelected);

         _adapter = CrossBluetoothLE.Current.Adapter;
         _adapter.DeviceDiscovered += (s, a) =>
         {
            _gattDevices.Add(a.Device);
            Items.Add(new BleDevice {
               Name = a.Device.Name,
               Address = a.Device.Id.ToString(),
            });
         };
      }

      void ExecuteScanCommand() {
         MainThread.BeginInvokeOnMainThread(async () => {
            await StartScanningForBLEDevices();
         });
      }

      private async Task StartScanningForBLEDevices() {
         IsRefreshingData = true;

         try {
            await RequestPermissions();

            Items.Clear();
            _gattDevices.Clear();
            await _adapter.StartScanningForDevicesAsync();
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         } finally {
            IsRefreshingData = false;
         }
      }

      public void OnAppearing() {
         IsRefreshingData = true;
         SelectedItem = null;
      }

      public BleDevice SelectedItem { get; set; }

      public void OnSelectedItemChanged() {
         OnItemSelected(SelectedItem);
      }

      private async Task RequestPermissions() {
         if (DeviceInfo.Platform == DevicePlatform.Android) {

            if (DeviceInfo.Version.Major < 12) {
               var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

               if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted) {
                  if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location)) {
                     await DisplayAlertAsync("Need location", "App needs location permission", "OK");
                  }

                  var status1 = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();

                  if (status1 == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                     status = status1;
               }

               if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted) {
                  await DisplayAlertAsync("Need location permission", "App need location permission to connect to MxA device. Re-scan to grant the permission", "OK");
               }
            }

            var blePermission = DependencyService.Get<IBLEPermission>();
            Xamarin.Essentials.PermissionStatus bleStatus = await blePermission.CheckStatusAsync();

            if (bleStatus != Xamarin.Essentials.PermissionStatus.Granted) {
               if (await blePermission.RequestAsync() != Xamarin.Essentials.PermissionStatus.Granted) {
                  await DisplayAlertAsync(
                     "Nearby devices permission",
                     "The app needs Nearby devices permission to connect to MxA device. Re-scan to grant the permission", "OK");
               }
            }
         } else {
            throw new NotSupportedException("This device platform is not supported. Contact developer team.");
         }
      }

      private async void OnItemSelected(BleDevice item) {
         if (item != null) {
            Preferences.Set("BLE_ADDRESS", item.Address);
            await Shell.Current.GoToAsync("..");
         }
      }
   }
}