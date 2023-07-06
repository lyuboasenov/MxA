using MxA.Models;
using MxA.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MxA.ViewModels {
   public class ActivityLogsViewModel : BaseViewModel {
      public ObservableCollection<ActivityLogGroup> LogEntries { get; }
      public Command LoadItemsCommand { get; }

      public ActivityLogsViewModel() {
         Title = "Activity logs";
         LogEntries = new ObservableCollection<ActivityLogGroup>();
         LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
      }

      async Task ExecuteLoadItemsCommand() {
         IsBusy = true;

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
            IsBusy = false;
         }
      }

      public void OnAppearing() {
         IsBusy = true;
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
   }
}