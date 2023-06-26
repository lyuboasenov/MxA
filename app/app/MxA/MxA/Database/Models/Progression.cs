using SQLite;
using System;

namespace MxA.Database.Models {
   public class Progression : IModel, IVersion {
      [PrimaryKey]
      public string Id { get; set; }
      public string Name { get; set; }
      public uint Version { get; set; }
      public DateTime Created { get; set; }
      public DateTime? Updated { get; set; }
      public bool Active { get; set; }
   }
}
