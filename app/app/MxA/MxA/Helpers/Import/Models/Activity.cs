namespace MxA.Helpers.Import.Models {
   public class Activity {
      public string Id { get; set; }
      public string Workout { get; set; }
      public string Exercise { get; set; }
      public string Notes { get; set; }
      public uint Work { get; set; }
      public uint RestBWReps { get; set; }
      public uint RestBWSets { get; set; }
      public uint Reps { get; set; }
      public uint Sets { get; set; }
      public uint Prep { get; set; }
      public uint TimeUnderTension { get; set; }
      public bool SkipLastRepRest { get; set; }
      public bool SkipLastSetRest { get; set; }
   }
}
