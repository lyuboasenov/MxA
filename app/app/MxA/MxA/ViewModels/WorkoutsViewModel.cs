using MxA.Database.Models;
using MxA.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MxA.ViewModels {
   public class WorkoutsViewModel : BaseViewModel {
      public ObservableCollection<Workout> Items { get; }
      public Command LoadItemsCommand { get; }
      public Command AddItemCommand { get; }
      public Command<Workout> ItemTapped { get; }
      public Command<Workout> StartWorkoutCommand { get; }
      public string SearchTerm { get; set; }

      public WorkoutsViewModel() {
         Title = "Workouts";
         Items = new ObservableCollection<Workout>();
         LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

         ItemTapped = new Command<Workout>(this.OnItemSelected);
         StartWorkoutCommand = new Command<Workout>(this.OnStartWorkoutCommand);

         AddItemCommand = new Command(OnAddItem);

         OnAppearing();
      }

      public void OnSearchTermChanged() {
         IsRefreshingData = true;
      }

      async Task ExecuteLoadItemsCommand() {
         IsRefreshingData = true;

         try {
            Items.Clear();

            var workouts = await DataStore.Workouts.GetItemsAsync();
            workouts = workouts.OrderBy(t => t.Name);
            workouts = workouts.Where(w => string.IsNullOrEmpty(SearchTerm) || w.Name.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0);

            foreach (var w in workouts) {
               Items.Add(w);
            }
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

      public Workout SelectedItem { get; set; }

      public void OnSelectedItemChanged() {
         OnItemSelected(SelectedItem);
      }

      private async void OnAddItem(object obj) {
         await Shell.Current.GoToAsync(nameof(NewItemPage));
      }

      async void OnItemSelected(Workout item) {
         if (item == null)
            return;

         // This will push the ItemDetailPage onto the navigation stack
         await Shell.Current.GoToAsync($"//{nameof(TrainingsPage)}/{nameof(WorkoutPage)}?{nameof(WorkoutViewModel.WorkoutId)}={item.Id}");
      }
      async void OnStartWorkoutCommand(Workout item) {
         if (item == null)
            return;

         await Shell.Current.GoToAsync($"//{nameof(TimerPage)}?{nameof(TimerViewModel.WorkoutId)}={item.Id}");
      }
   }
}