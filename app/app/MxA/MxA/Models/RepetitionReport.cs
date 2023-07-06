using Microcharts;

namespace MxA.Models {
   public class RepetitionReport {
      public uint Repetition { get; set; }
      public uint Set { get; set; }
      public double AverageLoad { get; set; }
      public LineChart Chart { get; set; }
   }
}
