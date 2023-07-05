using Microcharts;
using MxA.Database.Models;
using MxA.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static MxA.Services.TimerStateMachine;

namespace MxA.ViewModels {
   public class ActivityLoadReportViewModel : BaseViewModel {
      public ObservableCollection<RepetitionReport> Reps { get; } = new ObservableCollection<RepetitionReport>();


      public ActivityLoadReportViewModel() {


      }

      private ChartEntry CreateEntry(TimerEvent e) {
         string label = $"{e.Counter}.{e.SubCounter}";
         SKColor color = GetColor(e.State);
         SKColor textcolor = SKColors.White.WithAlpha(180);

         return new ChartEntry((float)e.Load) {
            ValueLabel = e.Load.ToString(),
            TextColor = textcolor,
            Label = label,
            Color = color.WithAlpha(225)
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
