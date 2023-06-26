using SQLite;
using System;
using System.IO;

namespace MxA {
   internal class Constants {
      public static class DB {
         public static readonly string DatabasePath = Path.Combine(
                  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                  "local.db3");
         public static readonly SQLiteOpenFlags Flags =
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.SharedCache;
      }
   }
}
