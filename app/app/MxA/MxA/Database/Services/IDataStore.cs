using MxA.Database.Models;

namespace MxA.Database.Services {
   public interface IDataStore {
      IDataStoreEntity<Workout> Workouts { get; }

      IDataStoreEntity<WorkoutLog> WorkoutLogs { get; }
      IDataStoreEntity<TimerEvent> TimerEvents { get; }
   }
}