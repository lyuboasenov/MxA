using MxA.Database.Models;
using MxA.Models;
using MxA.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace MxA.ViewModels {
   public class WorkoutsViewModel : BaseViewModel {
      public ObservableCollection<Models.WorkoutType> Items { get; }
      public Command LoadItemsCommand { get; }
      public Command AddItemCommand { get; }
      public Command<Database.Models.Workout> ItemTapped { get; }

      public WorkoutsViewModel() {
         Title = "Workouts";
         Items = new ObservableCollection<Models.WorkoutType>();
         LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

         ItemTapped = new Command<Database.Models.Workout>(this.OnItemSelected);

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

            var types = await DataStore.Types.GetItemsAsync();
            types = types.OrderBy(t => t.Order);

            var targets = await DataStore.Targets.GetItemsAsync();
            var workouts = await DataStore.Workouts.GetItemsAsync();
            var progressions = await DataStore.Progression.GetItemsAsync();

            foreach (var type in types) {
               var list = new List<WorkoutTarget>();
               foreach(var target in targets) {
                  var filteredWorkouts = workouts.Where(w => w.TypeId == type.Id && w.TargetId == target.Id && w.WorkoutList);
                  if (filteredWorkouts.Any()) {
                     list.Add(new WorkoutTarget(target, filteredWorkouts.OrderBy(w => w.Name)));
                  }
               }
               if (list.Any()) {
                  Items.Add(new WorkoutType(type, list));
               }
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
      }

      public Database.Models.Workout SelectedItem { get; set; }

      public void OnSelectedItemChanged() {
         OnItemSelected(SelectedItem);
      }

      private async void OnAddItem(object obj) {
         await Shell.Current.GoToAsync(nameof(NewItemPage));
      }

      async void OnItemSelected(Database.Models.Workout item) {
         if (item == null)
            return;

         // This will push the ItemDetailPage onto the navigation stack
         await Shell.Current.GoToAsync($"{nameof(TimerPage)}?{nameof(TimerViewModel.TrainingId)}={item.Id}");
      }
   }
}