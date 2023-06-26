namespace MxA.Helpers.Import.Models {
   public class Progression {
      public string Id { get; set; }
      public string Name { get; set; }
      public WorkoutRef[] Workouts { get; set; }
   }
}
