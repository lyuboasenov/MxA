using Microcharts;
using MxA.Database.Models;
using MxA.Models;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using static MxA.Services.TimerStateMachine;

namespace MxA.ViewModels {

   [QueryProperty(nameof(ActivityLogId), nameof(ActivityLogId))]
   public class ActivityLoadReportViewModel : BaseViewModel {
      private ActivityLog _activityLog;
      private Activity _activity;
      private Exercise _exercise;
      private IEnumerable<TimerEvent> _timerEvents;
      public ObservableCollection<RepetitionReport> Reps { get; } = new ObservableCollection<RepetitionReport>();

      public string ActivityLogId { get; set; }
      public ActivityLog ActivityLog { get; set; }
      public Activity Activity { get; set; }
      public Exercise Exercise { get; set; }

      public async void OnActivityLogIdChanged() {
         ActivityLog = await DataStore.ActivityLogs.GetItemAsync(ActivityLogId);
         Activity = await DataStore.Activities.GetItemAsync(ActivityLog.ActivityId);
         Exercise = await DataStore.Exercises.GetItemAsync(Activity.ExerciseId);
         _timerEvents = await DataStore.TimerEvents.GetItemsAsync(e => e.ActivityLogId == ActivityLogId);

         Title = $"{Exercise.Name} on {ActivityLog.Created.ToString("yyyy/MM/dd")}";

         var group = _timerEvents.
            OrderBy(o => o.Set).
            ThenBy(oo => oo.Set).
            GroupBy(e => new { Repetition = e.Repetition, Set = e.Set });

         foreach (var g in group) {
            Reps.Add(
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
