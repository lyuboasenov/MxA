using SQLite;

namespace MxA.Database.Models {
   public class WorkoutActivity : IModel {
      [PrimaryKey]
      public string Id { get; set; }
      public string WorkoutId { get; set; }
      public string ActivityId { get; set; }
      public uint Order { get; set; }
   }
}
