using Newtonsoft.Json;

namespace MxA.Helpers.Import.Models {
   public class WorkoutRef {
      [JsonProperty("_id")]
      public string Id { get; set; }
      public string Label { get; set; }
      [JsonProperty("_workout")]
      public string Workout { get; set; }
   }
}
