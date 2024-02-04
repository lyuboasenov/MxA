using Microcharts;
using MxA.Database.Models;
using MxA.Helpers;
using MxA.Helpers.ImportExport;
using MxA.Models;
using MxA.Views;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static MxA.Services.TimerStateMachine;

namespace MxA.ViewModels {

   [QueryProperty(nameof(WorkoutLogId), nameof(WorkoutLogId))]
   public class ActivityLoadReportViewModel : BaseViewModel {

      private IEnumerable<TimerEvent> _timerEvents;
      public ObservableCollection<RepetitionReport> Repetitions { get; } = new ObservableCollection<RepetitionReport>();

      public string WorkoutLogId { get; set; }
      public WorkoutLog WorkoutLog { get; set; }
      public Workout Workout { get; set; }

      public uint TotalRepetitions { get; set; }
      public uint TotalSets { get; set; }
      public double TotalAverageLoad { get; set; }

      public ICommand DeleteActivityLogCommand { get; }
      public ICommand ExitCommand { get; }
      public ICommand ExportCommand { get; }
      public ICommand CopyCommand { get; }

      public ActivityLoadReportViewModel() {
         DeleteActivityLogCommand = new Command(async () => await OnDeleteActivityLogCommand());
         ExitCommand = new Command(async () => await OnExitCommand());
         ExportCommand = new Command(async () => await OnExportCommand());
         CopyCommand = new Command(async () => await OnCopyCommand());
      }

      public async void OnActivityLogIdChanged() {
         WorkoutLog = await DataStore.WorkoutLogs.GetItemAsync(WorkoutLogId);
         Workout = await DataStore.Workouts.GetItemAsync(WorkoutLog.WorkoutId);
         _timerEvents = await DataStore.TimerEvents.GetItemsAsync(e => e.WorkoutLogId == WorkoutLogId);

         Title = $"{Workout.Name} on {WorkoutLog.Created.ToString("yyyy/MM/dd")}";

         var workEvents = _timerEvents.Where(w => w.State == TimerState.Work);
         TotalRepetitions = (uint) workEvents.Select(s => s.Repetition).Distinct().Count();
         TotalSets = (uint) workEvents.Select(s => s.Set).Distinct().Count();
         TotalAverageLoad = workEvents.Sum(s => s.Load) / workEvents.Count();

         var group = _timerEvents.
            OrderBy(o => o.Set).
            ThenBy(oo => oo.Repetition).
            GroupBy(e => new { Repetition = e.Repetition, Set = e.Set });

         var dict = group.ToDictionary(d => d.Key, dd => dd.ToList());

         uint maxOrder = 0;
         for(int i = 0; i < dict.Count; i++) {
            var kvp = dict.ElementAt(i);
            var elementsToMove = new List<TimerEvent>();
            foreach (var item in kvp.Value) {
               if ((item.State == TimerState.RepetitionRest || item.State == TimerState.SetRest)
                  && item.Counter < 2
                  && item.Order > maxOrder) {
                  elementsToMove.Add(item);
               }
            }

            kvp.Value.RemoveAll(e => elementsToMove.Contains(e));
            if (elementsToMove.Any()) {
               maxOrder = elementsToMove.Max(m => m.Order);
            }
            var next = dict.ElementAtOrDefault(i + 1);
            next.Value?.AddRange(elementsToMove);
         }

         foreach (var g in dict) {
            var ordered = g.Value.OrderBy(o => o.Order).ToArray();
            Repetitions.Add(
               new RepetitionReport() {
                  Repetition = g.Key.Repetition + 1,
                  Set = g.Key.Set + 1,
                  AverageLoad = g.Value.Where(w => w.State == TimerState.Work).Sum(s => s.Load) / g.Value.Where(w => w.State == TimerState.Work).Count(),
                  MinLoad = g.Value.Where(w => w.State == TimerState.Work).Min(s => s.Load),
                  MaxLoad = g.Value.Where(w => w.State == TimerState.Work).Max(s => s.Load),
                  Chart = new LineChart() {
                     Entries = g.Value.
                        OrderBy(o => o.Order).
                        Select(CreateEntry).ToArray(),
                  }
               });
         }
      }

      private Task OnCopyCommand() {
         StringBuilder sb = new StringBuilder();
         sb.AppendLine(WorkoutLog.WorkoutName);
         sb.AppendLine("------------------");
         sb.AppendLine($"Notes: {WorkoutLog.Note}");
         sb.AppendLine("------------------");
         sb.AppendLine($"Total:");
         sb.AppendLine($"  Avg Load: {TotalAverageLoad:0.00}");
         sb.AppendLine($"  Reps/Sets: {TotalRepetitions}/{TotalSets}");
         sb.AppendLine("------------------");

         var sets = _timerEvents.
            Where(w => w.State == TimerState.Work).
            OrderBy(o => o.Set).
            ThenBy(oo => oo.Repetition).
            GroupBy(e => e.Set);
            //GroupBy(e => new { Repetition = e.Repetition, Set = e.Set });

         foreach (var s in sets) {
            sb.Append($"Set {s.Key + 1}: ");
            var reps = s.GroupBy(g => g.Repetition);

            foreach (var r in reps) {
               sb.Append($"{r.Average(a => a.Load):0.00}/");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine();
         }

         return Xamarin.Essentials.Clipboard.SetTextAsync( sb.ToString() );
      }

      private Task OnExportCommand() {
         return LogbookExporter.ExportAsync(WorkoutLog, _timerEvents);
      }

      private Task OnExitCommand() {
         return Shell.Current.GoToAsync($"//{nameof(ActivityLogsPage)}");
      }

      private async Task OnDeleteActivityLogCommand() {
         if (await DisplayAlertAsync("Delete", "Are you sure, you want to delete this activity?", "Yes", "No")) {
            var tasks = new List<Task>();
            foreach(var e in _timerEvents) {
               tasks.Add(DataStore.TimerEvents.DeleteItemAsync(e.Id));
            }

            tasks.Add(DataStore.WorkoutLogs.DeleteItemAsync(WorkoutLog.Id));

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
               return Settings.PreparationColor.ToSKColor();
            case TimerState.Work:
               return Settings.WorkColor.ToSKColor();
            case TimerState.RepetitionRest:
               return Settings.RepetitionRestColor.ToSKColor();
            case TimerState.SetRest:
               return Settings.SetRestColor.ToSKColor();
            default:
               return SKColors.Transparent;
         }
      }
   }
}
