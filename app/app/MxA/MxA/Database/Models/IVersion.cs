using System;

namespace MxA.Database.Models {
   public interface IVersion {
      uint Version { get; set; }
      DateTime Created { get; set; }
      DateTime? Updated { get; set; }
      bool Active { get; set; }
   }
}
