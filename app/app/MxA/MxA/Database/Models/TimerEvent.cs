using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using static MxA.Services.TimerStateMachine;

namespace MxA.Database.Models {
   public class TimerEvent {
      [PrimaryKey]
      public string Id { get; set; }
      public string ActivityLogId { get; set; }
      public uint Counter { get; set; }
      public uint SubCounter { get; set; }
      public uint Repetition { get; set; }
      public uint Set { get; set; }
      public TimerState State { get; set; }
      public double Load { get; set; }
   }
}
