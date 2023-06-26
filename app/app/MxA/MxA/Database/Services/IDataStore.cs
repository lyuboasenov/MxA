using MxA.Database.Models;

namespace MxA.Database.Services {
   public interface IDataStore {
      IDataStoreEntity<Activity> Activities { get; }
      IDataStoreEntity<Equipment> Equipments { get; }
      IDataStoreEntity<Exercise> Exercises { get; }
      IDataStoreEntity<ExerciseFocusPoint> ExerciseFocusPoints { get; }
      IDataStoreEntity<Progression> Progression { get; }
      IDataStoreEntity<ProgressionWorkoutRef> ProgressionWorkoutRefs { get; }
      IDataStoreEntity<Target> Targets { get; }
      IDataStoreEntity<Type> Types { get; }
      IDataStoreEntity<Workout> Workouts { get; }
      IDataStoreEntity<WorkoutActivity> WorkoutActivities { get; }
      IDataStoreEntity<WorkoutEquipment> WorkoutEquipments { get; }
      IDataStoreEntity<WorkoutRef> WorkoutRefs { get; }
   }
}