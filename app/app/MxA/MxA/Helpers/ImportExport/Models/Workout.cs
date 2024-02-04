using MxA.Models;

namespace MxA.Helpers.Import.Models {
   public class Workout {
      public string Parent { get; set; }
      public string Id { get; set; }
      public string Name { get; set; }
      public string[] Equipment { get; set; }
      public uint Duration { get; set; }
      public uint WorkDuration { get; set; }
      public string[] Activities { get; set; }
      public string Notes { get; set; }
      public string Summary { get; set; }
      public string Thumbnail { get; set; }
      public string Type { get; set; }
      public string Target { get; set; }
      public string Progression { get; set; }
      public uint Version { get; set; }
      public System.DateTime Updated { get; set; }
      public System.DateTime Created { get; set; }
      public bool WorkoutList { get; set; }
   }
}
