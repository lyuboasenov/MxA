using System.Drawing;

namespace PortableLoadCell.Models {
   public class Period {
      public string Name { get; set; }
      public uint Time { get; set; }
      public Color Color { get; set; }
      public uint From { get; set; }
      public uint To { get; set; }
      public uint Rep { get; set; }
      public uint Set { get; set; }
      public uint RepIndex { get; set; }
      public uint SetIndex { get; set; }
      public PeriodType PeriodType { get; set; }

      public override string ToString() {
         return $"{PeriodType} {Time} {RepIndex}/{SetIndex}";
      }
   }
}
