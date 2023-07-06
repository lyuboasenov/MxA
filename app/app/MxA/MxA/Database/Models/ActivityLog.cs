using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxA.Database.Models {
   public class ActivityLog : IModel {
      [PrimaryKey]
      public string Id { get; set; }
      public string ActivityId { get; set; }
      public string Note { get; set; }
      public DateTime Created { get; set; }
   }
}
