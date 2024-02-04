using SQLite;
using System;

namespace MxA.Database.Models {
   public class WorkoutLog : IModel {
      [PrimaryKey]
      public string Id { get; set; }
      public string WorkoutId { get; set; }
      public string WorkoutName { get; set; }
      public string ActivityDetailsJson { get; set; }
      public string Note { get; set; }
      public DateTime Created { get; set; }
   }
}
