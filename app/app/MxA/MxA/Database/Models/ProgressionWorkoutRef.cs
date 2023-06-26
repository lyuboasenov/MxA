using SQLite;

namespace MxA.Database.Models {
   public class ProgressionWorkoutRef : IModel {
      [PrimaryKey]
      public string Id { get; set; }
      public string ProgressionId { get; set; }
      public string WorkoutRefId { get; set; }
      public uint Order { get; set; }
   }
}
