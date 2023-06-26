// Ignore Spelling: Mx

using MxA.Database.Models;
using MxA.Services;
using SQLite;
using System.Threading.Tasks;

namespace MxA.Database.Services {
   internal class DataStore : IDataStore {
      public IDataStoreEntity<Activity> Activities { get; }
      public IDataStoreEntity<Equipment> Equipments { get; }
      public IDataStoreEntity<Exercise> Exercises { get; }
      public IDataStoreEntity<Progression> Progression { get; }
      public IDataStoreEntity<Type> Types { get; }
      public IDataStoreEntity<Target> Targets { get; }
      public IDataStoreEntity<WorkoutRef> WorkoutRefs { get; }
      public IDataStoreEntity<Workout> Workouts { get; }
      public IDataStoreEntity<ExerciseFocusPoint> ExerciseFocusPoints { get; }
      public IDataStoreEntity<ProgressionWorkoutRef> ProgressionWorkoutRefs { get; }
      public IDataStoreEntity<WorkoutActivity> WorkoutActivities { get; }
      public IDataStoreEntity<WorkoutEquipment> WorkoutEquipments { get; }
      public Task<SQLiteAsyncConnection> Database => _databaseLazy.Value;

      public static SQLiteAsyncConnection _database;

      private static readonly AsyncLazy<SQLiteAsyncConnection> _databaseLazy = new AsyncLazy<SQLiteAsyncConnection>(async () => {

         _database = new SQLiteAsyncConnection(Constants.DB.DatabasePath, Constants.DB.Flags);
         await _database.CreateTableAsync<Activity>();
         await _database.CreateTableAsync<Equipment>();
         await _database.CreateTableAsync<Exercise>();
         await _database.CreateTableAsync<ExerciseFocusPoint>();
         await _database.CreateTableAsync<Progression>();
         await _database.CreateTableAsync<ProgressionWorkoutRef>();
         await _database.CreateTableAsync<Target>();
         await _database.CreateTableAsync<Type>();
         await _database.CreateTableAsync<Workout>();
         await _database.CreateTableAsync<WorkoutActivity>();
         await _database.CreateTableAsync<WorkoutEquipment>();
         await _database.CreateTableAsync<WorkoutRef>();

         return _database;
      });

      public DataStore() {
         Activities = new DataStoreEntity<Activity>(this);
         Equipments = new DataStoreEntity<Equipment>(this);
         Exercises = new DataStoreEntity<Exercise>(this);
         Progression = new DataStoreEntity<Progression>(this);
         Types = new DataStoreEntity<Type>(this);
         Targets = new DataStoreEntity<Target>(this);
         WorkoutRefs = new DataStoreEntity<WorkoutRef>(this);
         Workouts = new DataStoreEntity<Workout>(this);
         ExerciseFocusPoints = new DataStoreEntity<ExerciseFocusPoint>(this);
         ProgressionWorkoutRefs = new DataStoreEntity<ProgressionWorkoutRef>(this);
         WorkoutActivities = new DataStoreEntity<WorkoutActivity>(this);
         WorkoutEquipments = new DataStoreEntity<WorkoutEquipment>(this);
      }
   }
}
