using System.Collections.Generic;
using Xamarin.Forms;

namespace MxA.Models {
   public class SimpleTraining : Training {
      public uint PrepTime { get; set; }
      public uint WorkTime { get; set; }
      public uint RestTime { get; set; }
      public uint RestBwSetsTime { get; set; }
      public uint CoolDownTime { get; set; }

      public override IEnumerable<Period> Expand() {
         List<Period> _periods = new List<Period>();
         uint totalTime = 0;
         _periods.Add(new Period {
            Color = Color.Orange,
            Name = "Prepare",
            Time = PrepTime,
            From = totalTime,
            To = PrepTime,
            Rep = 0,
            Set = 0,
            PeriodType = PeriodType.Prepare
         });
         totalTime += PrepTime;

         for (uint set = 0; set < Sets; set++) {
            for (uint rep = 0; rep < Reps; rep++) {
               _periods.Add(new Period {
                  Color = Color.Green,
                  Name = "Work",
                  Time = WorkTime,
                  From = totalTime,
                  To = WorkTime + totalTime,
                  Rep = rep + 1,
                  Set = set + 1,
                  PeriodType = PeriodType.Work
               });
               totalTime += WorkTime;

               if (rep < Reps - 1) {
                  _periods.Add(new Period {
                     Color = Color.Red,
                     Name = "Rest",
                     Time = RestTime,
                     From = totalTime,
                     To = RestTime + totalTime,
                     Rep = rep + 1,
                     Set = set + 1,
                     PeriodType = PeriodType.Rest
                  });
                  totalTime += RestTime;
               }
            }

            if (set == Sets - 1) {
               _periods.Add(new Period {
                  Color = Color.DarkRed,
                  Name = "Long Rest",
                  Time = RestBwSetsTime,
                  From = totalTime,
                  To = RestBwSetsTime + totalTime,
                  Rep = Reps,
                  Set = set + 1,
                  PeriodType = PeriodType.Rest
               });
               totalTime += RestBwSetsTime;
            }
         }

         _periods.Add(new Period {
            Color = Color.Red,
            Name = "CoolDown",
            Time = CoolDownTime,
            From = totalTime,
            To = CoolDownTime + totalTime,
            Rep = Reps,
            Set = Sets,
            PeriodType = PeriodType.CoolDown
         });
         totalTime += CoolDownTime;

         return _periods;
      }
   }
}
