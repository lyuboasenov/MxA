using MxA.Database.Services;
using MxA.Helpers;
using MxA.Models;
using MxA.Views;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Shapes;

namespace MxA.ViewModels {
   public class ActivityLogsViewModel : BaseViewModel {
      public ObservableCollection<ActivityLogGroup> LogEntries { get; }
      public Command LoadItemsCommand { get; }
      public ICommand ExportCommand { get; }

      public ActivityLogsViewModel() {
         Title = "Activity logs";
         LogEntries = new ObservableCollection<ActivityLogGroup>();
         LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
         ExportCommand = new Command(async () => await OnExportCommand());
      }

      async Task ExecuteLoadItemsCommand() {
         IsRefreshingData = true;

         try {
            LogEntries.Clear();
            var allItems = await DataStore.ActivityLogs.GetItemsAsync(true);
            var groups = allItems.GroupBy(i => new {
               Year = i.Created.Year,
               Month = i.Created.Month,
               Day = i.Created.Day
            });


            foreach (var item in groups) {
               var g = new ActivityLogGroup() {
                  Date = new DateTime(item.Key.Year, item.Key.Month, item.Key.Day)
               };

               foreach(var l in item) {
                  var activity = await DataStore.Activities.GetItemAsync(l.ActivityId);
                  var exercise = await DataStore.Exercises.GetItemAsync(activity.ExerciseId);
                  g.Add(new ActivityActivityLog() {
                     Activity = activity,
                     Exercise = exercise,
                     ActivityLog = l
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
         LoadItemsCommand.Execute(true);
      }

      public ActivityActivityLog SelectedItem { get; set; }

      public void OnSelectedItemChanged() {
         OnItemSelected(SelectedItem);
      }

      async void OnItemSelected(ActivityActivityLog item) {
         if (item == null)
            return;

         // This will push the ItemDetailPage onto the navigation stack
         await Shell.Current.GoToAsync($"{nameof(ActivityLoadReportPage)}?{nameof(ActivityLoadReportViewModel.ActivityLogId)}={item.ActivityLog.Id}");

      }

      private async Task OnExportCommand() {
         var exportBundle = new {
            LogEntries = await DataStore.ActivityLogs.GetItemsAsync(),
            Events = await DataStore.TimerEvents.GetItemsAsync(),
         };

         var exporter = DependencyService.Get<IDownloadFolderExporter>();
         await exporter.ExportAsync($"mxa-logbook-export-{DateTime.Now.ToString("yyyy.MM.dd")}.json", JsonConvert.SerializeObject(exportBundle, Formatting.None));
      }
   }
}