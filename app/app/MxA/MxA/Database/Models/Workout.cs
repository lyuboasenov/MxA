using MxA.Models;
using SQLite;

namespace MxA.Database.Models {
   public class Workout : IModel, IVersion {
      [PrimaryKey]
      public string Id { get; set; }
      public string ParentId { get; set; }
      public string Name { get; set; }
      public Category Category { get; set; }
      public uint Duration { get; set; }
      public string Notes { get; set; }
      public string Summary { get; set; }
      public string Thumbnail { get; set; }
      public string TypeId { get; set; }
      public string TargetId { get; set; }
      public string ProgressionId { get; set; }
      public uint Version { get; set; }
      public System.DateTime? Updated { get; set; }
      public System.DateTime Created { get; set; }
      public bool Active { get; set; }
   }
}
