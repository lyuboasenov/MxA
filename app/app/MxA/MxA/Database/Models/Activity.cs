using SQLite;
using System;

namespace MxA.Database.Models {
   public class Activity : IModel, IVersion {
      [PrimaryKey]
      public string Id { get; set; }
      public string ExerciseId { get; set; }
      public uint Work { get; set; }
      public uint RestBWReps { get; set; }
      public uint RestBWSets { get; set; }
      public uint Reps { get; set; }
      public uint Sets { get; set; }
      public uint Prep { get; set; }
      public bool SkipLastRepRest { get; set; }
      public bool SkipLastSetRest { get; set; }
      public uint Version { get; set; }
      public DateTime Created { get; set; }
      public DateTime? Updated { get; set; }
      public bool Active { get; set; }
      public string WorkoutId { get; set; }
      public uint Order { get; set; }
   }
}
