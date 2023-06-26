using MxA.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxA.Models {
   public class ActivityExercise {
      public Activity Activity { get; set; }
      public Exercise Exercise { get; set; }
   }
}
