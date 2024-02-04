using MxA.Models;
using SQLite;

namespace MxA.Database.Models {
   public class Workout : IModel {
      [PrimaryKey]
      public string Id { get; set; }
      public string Name { get; set; }
      public string Note { get; set; }
      public uint Prep { get; set; }
      public uint Work { get; set; }
      public uint RepRest { get; set; }
      public uint SetRest { get; set; }
      public uint CoolDown { get; set; }
      public uint Reps { get; set; }
      public uint Sets { get; set; }
   }
}
