using SQLite;

namespace MxA.Database.Models {
   public class ExerciseFocusPoint : IModel {
      [PrimaryKey]
      public string Id { get; set; }
      public string ExerciseId { get; set; }
      public string FocusPoints { get; set; }
   }
}
