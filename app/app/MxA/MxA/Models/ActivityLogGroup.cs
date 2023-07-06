using MxA.Database.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MxA.Models {
   public class ActivityLogGroup : ObservableCollection<ActivityActivityLog> {
      public DateTime Date { get; set; }
   }
}
