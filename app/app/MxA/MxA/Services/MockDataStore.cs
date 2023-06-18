using MxA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MxA.Services {
   public class MockDataStore : IDataStore<Training> {
      readonly List<Training> _trainings;

      public MockDataStore() {
         _trainings = new List<Training>()
         {
                new SimpleTraining {
                    Id = "7_by_3",
                    Name = "7/3x6x6",
                    PrepTime = 10,
                    WorkTime = 7,
                    RestTime = 3,
                    RestBwSetsTime = 180,
                    CoolDownTime = 10,
                    Reps = 6,
                    Sets = 6,
                },
                new SimpleTraining {
                    Id = "one_arm_10_sec",
                    Name = "One arm 10 secs",
                    PrepTime = 10,
                    WorkTime = 10,
                    RestTime = 30,
                    RestBwSetsTime = 90,
                    CoolDownTime = 10,
                    Reps = 2,
                    Sets = 6,
                },
                new SimpleTraining {
                    Id = "one_arm_5_sec",
                    Name = "One arm 5 secs",
                    PrepTime = 10,
                    WorkTime = 5,
                    RestTime = 30,
                    RestBwSetsTime = 90,
                    CoolDownTime = 10,
                    Reps = 2,
                    Sets = 6,
                },
            };
      }

      public async Task<bool> AddItemAsync(Training item) {
         _trainings.Add(item);

         return await Task.FromResult(true);
      }

      public async Task<bool> UpdateItemAsync(Training item) {
         var oldItem = _trainings.Where((Training arg) => arg.Id == item.Id).FirstOrDefault();
         _trainings.Remove(oldItem);
         _trainings.Add(item);

         return await Task.FromResult(true);
      }

      public async Task<bool> DeleteItemAsync(string id) {
         var oldItem = _trainings.Where((Training arg) => arg.Id == id).FirstOrDefault();
         _trainings.Remove(oldItem);

         return await Task.FromResult(true);
      }

      public async Task<Training> GetItemAsync(string id) {
         return await Task.FromResult(_trainings.FirstOrDefault(s => s.Id == id));
      }

      public async Task<IEnumerable<Training>> GetItemsAsync(bool forceRefresh = false) {
         return await Task.FromResult(_trainings);
      }
   }
}