using MxA.Database.Models;
using MxA.Models;
using MxA.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace MxA.ViewModels {
   public class ExercisesViewModel : BaseViewModel {
      public ObservableCollection<Exercise> Items { get; }
      public Command LoadItemsCommand { get; }
      public Command AddItemCommand { get; }
      public Command<Training> ItemTapped { get; }

      public ExercisesViewModel() {
         Title = "Exercises";
         Items = new ObservableCollection<Exercise>();
         LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

         ItemTapped = new Command<Training>(OnItemSelected);

         AddItemCommand = new Command(OnAddItem);
      }

      private async Task<PermissionStatus> CheckAndRequestPermission<T>() where T : BasePermission, new() {
         try {
            var status = await Permissions.CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted) {
               status = await Permissions.RequestAsync<T>();
            }

            return status;
         } catch { }

         return PermissionStatus.Unknown;
      }

      async Task ExecuteLoadItemsCommand() {
         IsBusy = true;

         try {
            await CheckAndRequestPermission<Permissions.LocationWhenInUse>();
            await CheckAndRequestPermission<Permissions.StorageRead>();
            await CheckAndRequestPermission<Permissions.StorageWrite>();

            Items.Clear();
            var items = await DataStore.Exercises.GetItemsAsync();
            foreach (var item in items) {
               Items.Add(item);
            }
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         } finally {
            IsBusy = false;
         }
      }

      public void OnAppearing() {
         IsBusy = true;
         SelectedItem = null;
         LoadItemsCommand?.Execute(null);
      }

      public Training SelectedItem { get; set; }

      public void OnSelectedItemChanged() {
         OnItemSelected(SelectedItem);
      }

      private async void OnAddItem(object obj) {
         await Shell.Current.GoToAsync(nameof(NewItemPage));
      }

      async void OnItemSelected(Training item) {
         if (item == null)
            return;

         // This will push the ItemDetailPage onto the navigation stack
         await Shell.Current.GoToAsync($"{nameof(TimerPage)}?{nameof(TimerViewModel.ActivityId)}={item.Id}");
      }
   }
}