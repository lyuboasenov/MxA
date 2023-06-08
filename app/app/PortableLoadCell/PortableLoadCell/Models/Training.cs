using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PortableLoadCell.Models {
   public class Training {
      public string Id { get; set; }
      public string Name { get; set; }

      public uint PrepTime { get; set; }
      public uint WorkTime { get; set; }
      public uint RestTime { get; set; }
      public uint RestBwSetsTime { get; set; }
      public uint CooldownTime { get; set; }

      public uint Reps { get; set; }
      public uint Sets { get; set; }

      public IEnumerable<Period> Expand() {
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
                  });
                  totalTime += RestTime;
               }
            }

            _periods.Add(new Period {
               Color = Color.DarkRed,
               Name = "Long Rest",
               Time = RestBwSetsTime,
               From = totalTime,
               To = RestBwSetsTime + totalTime,
               Rep = Reps,
               Set = set + 1,
            });
            totalTime += RestBwSetsTime;
         }

         _periods.Add(new Period {
            Color = Color.Red,
            Name = "Cooldown",
            Time = CooldownTime,
            From = totalTime,
            To = CooldownTime + totalTime,
            Rep = Reps,
            Set = Sets,
         });
         totalTime += CooldownTime;

         return _periods;
      }
   }
}
