using System.Collections.Generic;

namespace MxA.Models {
   public abstract class Training {
      public string Id { get; set; }
      public string Name { get; set; }

      public uint Reps { get; set; }
      public uint Sets { get; set; }

      public abstract IEnumerable<Period> Expand();
   }
}
