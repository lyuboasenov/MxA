using MxA.Helpers.ImportExport;
using MxA.Models;
using MxA.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MxA.ViewModels {
   public class ActivityLogsViewModel : BaseViewModel {
      public ObservableCollection<ActivityLogGroup> LogEntries { get; }
      public Command LoadLogEntriesCommand { get; }
      public ICommand ExportCommand { get; }
      public Command<ActivityActivityLog> ItemTappedCommand { get; }

      public ActivityLogsViewModel() {
         Title = "Activity logs";
         LogEntries = new ObservableCollection<ActivityLogGroup>();
         LoadLogEntriesCommand = new Command(async () => await OnLoadLogEntriesCommand());
         ExportCommand = new Command(async () => await OnExportCommand());
         ItemTappedCommand = new Command<ActivityActivityLog>(OnItemTappedCommand);
      }

      private void OnItemTappedCommand(ActivityActivityLog l) {
         SelectedItem  = l;
      }

      async Task OnLoadLogEntriesCommand() {
         IsRefreshingData = true;

         try {
            LogEntries.Clear();
            var allItems = await DataStore.WorkoutLogs.GetItemsAsync(true);
            var groups = allItems.GroupBy(i => new {
               Year = i.Created.Year,
               Month = i.Created.Month,
               Day = i.Created.Day
            }).OrderByDescending(o => new DateTime(o.Key.Year, o.Key.Month, o.Key.Day));


            foreach (var item in groups) {
               var g = new ActivityLogGroup() {
                  Date = new DateTime(item.Key.Year, item.Key.Month, item.Key.Day)
               };

               foreach(var l in item) {
                  var workout = await DataStore.Workouts.GetItemAsync(l.WorkoutId);
                  g.Add(new ActivityActivityLog() {
                     Workout = workout,
                     WorkoutLog = l
                  });
               }
               LogEntries.Add(g);
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

      public ActivityActivityLog SelectedItem { get; set; }

      public void OnSelectedItemChanged() {
         OnItemSelected(SelectedItem);
      }

      async void OnItemSelected(ActivityActivityLog item) {
         if (item == null)
            return;

         // This will push the ItemDetailPage onto the navigation stack
         await Shell.Current.GoToAsync($"//{nameof(ActivityLogsPage)}/{nameof(ActivityLoadReportPage)}?{nameof(ActivityLoadReportViewModel.WorkoutLogId)}={item.WorkoutLog.Id}");

      }

      private async Task OnExportCommand() {
         await LogbookExporter.ExportAsync(
            await DataStore.WorkoutLogs.GetItemsAsync(),
            await DataStore.TimerEvents.GetItemsAsync());
      }
   }
}