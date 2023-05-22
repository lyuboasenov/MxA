using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PortableLoadCell.Models {
   public class Period {
      public string Name { get; set; }
      public uint Time { get; set; }
      public Color Color { get; set; }
      public uint From { get; set; }
      public uint To { get; set; }
      public uint Rep { get; set; }
      public uint Set { get; set; }
   }
}
