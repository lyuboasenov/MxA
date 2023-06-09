using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PortableLoadCell.Models {
   public abstract class Training {
      public string Id { get; set; }
      public string Name { get; set; }

      public uint Reps { get; set; }
      public uint Sets { get; set; }

      public abstract IEnumerable<Period> Expand();

   }
}
