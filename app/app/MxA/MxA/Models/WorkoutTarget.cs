using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MxA.Models {
   public class WorkoutTarget : ObservableCollection<Database.Models.Workout> {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public string Thumbnail { get; set; }

      public WorkoutTarget(Database.Models.Target target, IEnumerable<Database.Models.Workout> workouts) {
         Id = target.Id;
         Name = target.Name;
         Description = target.Description;
         Thumbnail = target.Thumbnail;

         foreach(var w in workouts) {
            Add(w);
         }
      }
   }
}
