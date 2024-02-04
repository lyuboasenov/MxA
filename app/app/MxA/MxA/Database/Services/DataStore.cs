// Ignore Spelling: Mx

using MxA.Database.Models;
using MxA.Services;
using SQLite;
using System.Threading.Tasks;

namespace MxA.Database.Services {
   internal class DataStore : IDataStore {
      public IDataStoreEntity<Workout> Workouts { get; }
      public IDataStoreEntity<WorkoutLog> WorkoutLogs { get; }
      public IDataStoreEntity<TimerEvent> TimerEvents { get; }
      public Task<SQLiteAsyncConnection> Database => _databaseLazy.Value;

      public static SQLiteAsyncConnection _database;

      private static readonly AsyncLazy<SQLiteAsyncConnection> _databaseLazy = new AsyncLazy<SQLiteAsyncConnection>(async () => {
         _database = new SQLiteAsyncConnection(Constants.DB.DatabasePath, Constants.DB.Flags);
         await _database.CreateTableAsync<Workout>();
         await _database.CreateTableAsync<WorkoutLog>();
         await _database.CreateTableAsync<TimerEvent>();

         return _database;
      });

      public DataStore() {
         Workouts = new DataStoreEntity<Workout>(this);
         WorkoutLogs = new DataStoreEntity<WorkoutLog>(this);
         TimerEvents = new DataStoreEntity<TimerEvent>(this);
      }
   }
}
