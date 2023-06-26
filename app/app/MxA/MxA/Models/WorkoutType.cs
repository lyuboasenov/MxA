using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MxA.Models {
   public class WorkoutType : ObservableCollection<WorkoutTarget> {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public string Thumbnail { get; set; }

      public string Color { get; set; }

      public WorkoutType(Database.Models.Type type, IEnumerable<WorkoutTarget> targets) {
         Id = type.Id;
         Name = type.Name;
         Description = type.Description;
         Thumbnail = type.Thumbnail;
         Color = type.Color;

         foreach(var t in targets) {
            Add(t);
         }
      }
   }
}
