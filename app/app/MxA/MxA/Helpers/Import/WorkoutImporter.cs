using MxA.Database.Services;
using MxA.Helpers.Import.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MxA.Helpers {
   internal static class WorkoutImporter {

      public static async Task Import(Stream stream, IDataStore ds) {
         using(var reader = new StreamReader(stream)) {
            var data = JsonConvert.DeserializeObject<Container>(await reader.ReadToEndAsync());

            // Types
            await ImportEntity(
               data.Types,
               a => a.Select(s => new Database.Models.Type() {
                  Id = s.Id,
                  Name = s.Name,
                  Description = s.Description,
                  Thumbnail = s.Thumbnail,
                  Version = s.Version,
                  Color = s.Color,
                  Order = s.Order
               }),
               ds.Types);

            // Targets
            await ImportEntity(
               data.Targets,
               a => a.Select(s => new Database.Models.Target() {
                  Id = s.Id,
                  Name = s.Name,
                  Description = s.Description,
                  Thumbnail = s.Thumbnail,
                  Version = s.Version
               }),
               ds.Targets);

            // Equipment
            await ImportEntity(
               data.Equipments,
               a => a.Select(s => new Database.Models.Equipment() {
                  Id = s.Id,
                  Name = s.Name,
                  Description = s.Description,
                  Version = s.Version
               }),
               ds.Equipments);

            // Exercises
            await ImportEntity(
               data.Exercises,
               a => a.Select(s => new Database.Models.Exercise() {
                  Id = s.Id,
                  Name = s.Name,
                  Description = s.Description,
                  Thumbnail = s.Thumbnail,
                  Version = 0
               }),
               ds.Exercises);

            await ImportEntity(
               data.Exercises,
               a => {
                  var list = new List<Database.Models.ExerciseFocusPoint>();

                  foreach(var ex in a) {
                     foreach(var fp in ex.FocusPoints ?? new string[0]) {
                        list.Add(new Database.Models.ExerciseFocusPoint() {
                           ExerciseId = ex.Id,
                           FocusPoints = fp
                        });
                     }
                  }

                  return list;
               },
               ds.ExerciseFocusPoints,
               async (d) => {
                  var items = await d.GetItemsAsync();
                  foreach(var i in items) {
                     await d.DeleteItemAsync(i.Id);
                  }
               });

            // Activities
            await ImportEntity(
               data.Activities,
               a => a.Select(s => new Database.Models.Activity() {
                  Id = s.Id,
                  ExerciseId = s.Exercise,
                  Work = s.Work,
                  RestBWReps = s.RestBWReps,
                  RestBWSets = s.RestBWSets,
                  Reps = s.Reps,
                  Sets = s.Sets,
                  Prep = 10,
                  Version = 0,
                  SkipLastRepRest = false,
                  SkipLastSetRest = false
               }),
               ds.Activities);

            // Progression
            await ImportEntity(
               data.Progressions,
               a => a.Select(s => new Database.Models.Progression() {
                  Id = s.Id,
                  Version = 0,
                  Name = s.Name
               }),
               ds.Progression);

            await ImportEntity(
               data.Progressions,
               a => {
                  var list = new List<Database.Models.ProgressionWorkoutRef>();

                  uint order = 0;
                  foreach(var p in a) {
                     foreach(var wr in p.Workouts) {
                        list.Add(new Database.Models.ProgressionWorkoutRef() {
                           Id = wr.Id,
                           Order = order++,
                           ProgressionId = p.Id,
                           WorkoutRefId = wr.Id,
                        });
                     }
                  }

                  return list;
                  },
               ds.ProgressionWorkoutRefs,
               async (d) => {
                  var items = await d.GetItemsAsync();
                  foreach (var i in items) {
                     await d.DeleteItemAsync(i.Id);
                  }
               });

            await ImportEntity(
               data.Progressions,
               a => {
                  var list = new List<Database.Models.WorkoutRef>();

                  foreach (var p in a) {
                     foreach (var wr in p.Workouts) {
                        list.Add(new Database.Models.WorkoutRef() {
                           Id = wr.Id,
                           Label = wr.Label,
                           WorkoutId = wr.Workout
                        });
                     }
                  }

                  return list;
               },
               ds.WorkoutRefs,
               async (d) => {
                  var items = await d.GetItemsAsync();
                  foreach (var i in items) {
                     await d.DeleteItemAsync(i.Id);
                  }
               });

            // Workouts
            await ImportEntity(
               data.Workouts,
               a => a.Select(s => new Database.Models.Workout() {
                  Id = s.Id,
                  Version = 0,
                  Name = s.Name,
                  Category = s.Category,
                  Duration = s.Duration,
                  Notes = s.Notes,
                  ParentId = s.Parent,
                  ProgressionId = s.Progression,
                  Summary = s.Summary,
                  TargetId = s.Target,
                  TypeId = s.Type,
                  Thumbnail = s.Thumbnail,
                  WorkoutList = s.WorkoutList
               }),
               ds.Workouts);

            await ImportEntity(
               data.Workouts,
               a => {
                  var list = new List<Database.Models.WorkoutEquipment>();

                  foreach(var w in a) {
                     foreach(var e in w.Equipment) {
                        list.Add(
                           new Database.Models.WorkoutEquipment() {
                              WorkoutId = w.Id,
                              EquipmentId = e
                           });
                     }
                  }

                  return list;
                  },
               ds.WorkoutEquipments,
               async (d) => {
                  var items = await d.GetItemsAsync();
                  foreach (var i in items) {
                     await d.DeleteItemAsync(i.Id);
                  }
               });

            await ImportEntity(
               data.Workouts,
               a => {
                  var list = new List<Database.Models.WorkoutActivity>();

                  uint order = 0;
                  foreach (var w in a) {
                     foreach (var ac in w.Activities) {
                        list.Add(
                           new Database.Models.WorkoutActivity() {
                              WorkoutId = w.Id,
                              ActivityId = ac,
                              Order = order++
                           });
                     }
                  }

                  return list;
               },
               ds.WorkoutActivities,
               async (d) => {
                  var items = await d.GetItemsAsync();
                  foreach (var i in items) {
                     await d.DeleteItemAsync(i.Id);
                  }
               });
         }
      }

      private static async Task ImportEntity<T, U>(T[] sources, Func<T[], IEnumerable<U>> converter, IDataStoreEntity<U> ds, Func<IDataStoreEntity<U>, Task> clean = null) where U : Database.Models.IModel {
         if (clean != null) {
            await clean(ds);
         }

         foreach(var e in converter(sources)) {
            if (!string.IsNullOrEmpty(e.Id)) {
               await ds.DeleteItemAsync(e.Id);
            }
            await ds.AddOrUpdateItemAsync(e);
         }
      }
   }
}
