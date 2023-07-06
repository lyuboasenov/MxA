using MxA.Database.Models;

namespace MxA.Models {
   public class ActivityActivityLog {
      public Activity Activity { get; set; }
      public Exercise Exercise { get; set; }
      public ActivityLog ActivityLog { get; set; }
   }
}
