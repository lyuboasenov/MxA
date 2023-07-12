using System;

namespace MxA.Helpers.ImportExport.Models {
   public class ActivityLog {
      public string Id { get; set; }
      public string ActivityId { get; set; }
      public string ActivityName { get; set; }
      public string ActivityDetailsJson { get; set; }
      public string Note { get; set; }
      public DateTime Created { get; set; }
   }
}
