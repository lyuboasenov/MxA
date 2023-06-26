using SQLite;

namespace MxA.Database.Models {
   public class WorkoutEquipment : IModel {
      [PrimaryKey]
      public string Id { get; set; }
      public string WorkoutId { get; set; }
      public string EquipmentId { get; set; }
   }
}
