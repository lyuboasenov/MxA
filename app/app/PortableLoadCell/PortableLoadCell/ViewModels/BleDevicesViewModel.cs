using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using PortableLoadCell.Models;
using PortableLoadCell.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PortableLoadCell.ViewModels {
   public class BleDevicesViewModel : BaseViewModel {
      private BleDevice _selectedItem;

      private readonly IAdapter _adapter;
      private List<IDevice> _gattDevices = new List<IDevice>();

      public ObservableCollection<BleDevice> Items { get; }
      public Command ScanCommand { get; }
      public Command<BleDevice> ItemTapped { get; }

      public BleDevicesViewModel() {
         Title = "Browse";
         Items = new ObservableCollection<BleDevice>();
         ScanCommand = new Command(async () => await ExecuteScanCommand());

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

      async Task ExecuteScanCommand() {
         IsBusy = true;

         try {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted) {
               if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location)) {
                  Debug.WriteLine("Need location", "App needs location permission", "OK");
               }

               var status1 = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();


               if (status1 == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                  status = status1;
            }

            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted) {
               Debug.WriteLine("Need location", "App need location permission", "OK");
               return;
            }

            Items.Clear();
            _gattDevices.Clear();
            await _adapter.StartScanningForDevicesAsync();
         } catch (Exception ex) {
            Debug.WriteLine(ex);
         } finally {
            IsBusy = false;
         }
      }

      public void OnAppearing() {
         IsBusy = true;
         SelectedItem = null;
      }

      public BleDevice SelectedItem {
         get => _selectedItem;
         set {
            SetProperty(ref _selectedItem, value);
            OnItemSelected(value);
         }
      }

      async void OnItemSelected(BleDevice item) {
         if (item != null) {
            Preferences.Set("BLE_ADDRESS", item.Address);
            await Shell.Current.GoToAsync("..");
         }

      }
   }
}