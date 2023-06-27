using MxA.Database.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MxA.Models {
   public static class ActivityExtensions {
      public static IEnumerable<Period> Expand(this Activity activity) {
         List<Period> _periods = new List<Period>();
         uint totalTime = 0;
         _periods.Add(new Period {
            Color = Color.Orange,
            Name = "Prepare",
            Time = activity.Prep,
            From = totalTime,
            To = activity.Prep,
            Rep = 0,
            Set = 0,
            PeriodType = PeriodType.Prepare
         });
         totalTime += activity.Prep;

         for (uint set = 0; set < activity.Sets; set++) {
            for (uint rep = 0; rep < activity.Reps; rep++) {
               _periods.Add(new Period {
                  Color = Color.Green,
                  Name = "Work",
                  Time = activity.Work,
                  From = totalTime,
                  To = activity.Work + totalTime,
                  Rep = rep + 1,
                  Set = set + 1,
                  PeriodType = PeriodType.Work
               });
               totalTime += activity.Work;

               if (rep < activity.Reps - 1) {
                  _periods.Add(new Period {
                     Color = Color.Red,
                     Name = "Rest",
                     Time = activity.RestBWReps,
                     From = totalTime,
                     To = activity.RestBWReps + totalTime,
                     Rep = rep + 1,
                     Set = set + 1,
                     PeriodType = PeriodType.Rest
                  });
                  totalTime += activity.RestBWReps;
               }
            }

            if (set == activity.Sets - 1) {
               _periods.Add(new Period {
                  Color = Color.DarkRed,
                  Name = "Long Rest",
                  Time = activity.RestBWSets,
                  From = totalTime,
                  To = activity.RestBWSets + totalTime,
                  Rep = activity.Reps,
                  Set = set + 1,
                  PeriodType = PeriodType.Rest
               });
               totalTime += activity.RestBWSets;
            }
         }

         return _periods;
      }
   }
}
