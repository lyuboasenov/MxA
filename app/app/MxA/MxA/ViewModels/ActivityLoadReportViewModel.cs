using Microcharts;
using MxA.Database.Models;
using MxA.Helpers;
using MxA.Models;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static MxA.Services.TimerStateMachine;

namespace MxA.ViewModels {

   [QueryProperty(nameof(ActivityLogId), nameof(ActivityLogId))]
   public class ActivityLoadReportViewModel : BaseViewModel {

      private IEnumerable<TimerEvent> _timerEvents;
      public ObservableCollection<RepetitionReport> Repetitions { get; } = new ObservableCollection<RepetitionReport>();

      public string ActivityLogId { get; set; }
      public ActivityLog ActivityLog { get; set; }
      public Activity Activity { get; set; }
      public Exercise Exercise { get; set; }

      public uint TotalRepetitions { get; set; }
      public uint TotalSets { get; set; }
      public double TotalAverageLoad { get; set; }

      public ICommand DeleteActivityLogCommand { get; }
      public ICommand ExitCommand { get; }
      public ICommand ExportCommand { get; }

      public ActivityLoadReportViewModel() {
         DeleteActivityLogCommand = new Command(async () => await OnDeleteActivityLogCommand());
         ExitCommand = new Command(async () => await OnExitCommand());
         ExportCommand = new Command(async () => await OnExportCommand());
      }

      public async void OnActivityLogIdChanged() {
         ActivityLog = await DataStore.ActivityLogs.GetItemAsync(ActivityLogId);
         Activity = await DataStore.Activities.GetItemAsync(ActivityLog.ActivityId);
         Exercise = await DataStore.Exercises.GetItemAsync(Activity.ExerciseId);
         _timerEvents = await DataStore.TimerEvents.GetItemsAsync(e => e.ActivityLogId == ActivityLogId);

         Title = $"{Exercise.Name} on {ActivityLog.Created.ToString("yyyy/MM/dd")}";

         var workEvents = _timerEvents.Where(w => w.State == TimerState.Work);
         TotalRepetitions = (uint) workEvents.Select(s => s.Repetition).Distinct().Count();
         TotalSets = (uint) workEvents.Select(s => s.Set).Distinct().Count();
         TotalAverageLoad = workEvents.Sum(s => s.Load) / workEvents.Count();

         var group = _timerEvents.
            OrderBy(o => o.Set).
            ThenBy(oo => oo.Repetition).
            GroupBy(e => new { Repetition = e.Repetition, Set = e.Set });

         foreach (var g in group) {
            Repetitions.Add(
               new RepetitionReport() {
                  Repetition = g.Key.Repetition + 1,
                  Set = g.Key.Set + 1,
                  AverageLoad = g.Where(w => w.State == TimerState.Work).Sum(s => s.Load) / g.Where(w => w.State == TimerState.Work).Count(),
                  Chart = new LineChart() {
                     Entries = g.
                        OrderBy(o => o.Order).
                        Select(CreateEntry)
                  }
               });
         }
      }

      private Task OnExportCommand() {
         var exportBundle = new {
            LogEntries = new[] { ActivityLog },
            Events = _timerEvents,
         };

         var exporter = DependencyService.Get<IDownloadFolderExporter>();
         return exporter.ExportAsync($"mxa-logbook-export-{DateTime.Now.ToString("yyyy.MM.dd")}.json", JsonConvert.SerializeObject(exportBundle, Formatting.None));
      }

      private Task OnExitCommand() {
         return Shell.Current.GoToAsync("..");
      }

      private async Task OnDeleteActivityLogCommand() {
         if (await DisplayAlertAsync("Delete", "Are you sure, you want to delete this activity?", "Yes", "No")) {
            var tasks = new List<Task>();
            foreach(var e in _timerEvents) {
               tasks.Add(DataStore.TimerEvents.DeleteItemAsync(e.Id));
            }

            tasks.Add(DataStore.ActivityLogs.DeleteItemAsync(ActivityLog.Id));

            await Task.WhenAll(tasks);
         }

         await Shell.Current.GoToAsync("..");
      }

      private ChartEntry CreateEntry(TimerEvent e) {
         string label = $"{e.Counter}.{e.SubCounter}";
         SKColor color = GetColor(e.State);
         SKColor textcolor = color.WithAlpha(180);

         return new ChartEntry((float)e.Load) {
            ValueLabel = e.Load.ToString("0.0"),
            TextColor = textcolor,
            Label = label,
            Color = color.WithAlpha(225),
            ValueLabelColor = color
         };
      }

      private SKColor GetColor(TimerState state) {
         switch (state) {
            case TimerState.Preparation:
               return SKColors.Orange;
            case TimerState.Work:
               return SKColors.Green;
            case TimerState.RepetitionRest:
            case TimerState.SetRest:
               return SKColors.Blue;
            default:
               return SKColors.Transparent;
         }
      }
   }
}
