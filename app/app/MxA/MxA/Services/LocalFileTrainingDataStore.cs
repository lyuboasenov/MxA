using Newtonsoft.Json;
using MxA.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MxA.Database.Services;

namespace MxA.Services {
   public class LocalFileTrainingDataStore : IDataStoreEntity<Training> {
      private readonly List<Training> _trainings;
      private readonly string _directory;

      public LocalFileTrainingDataStore() {
         _directory = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
               "trainings",
               "simple");

         if (!Directory.Exists(_directory)) {
            Directory.CreateDirectory(_directory);
         }

         var files = Directory.GetFiles(_directory);

         _trainings = new List<Training>();

         foreach(var filePath in files) {
            _trainings.Add(JsonConvert.DeserializeObject<SimpleTraining>(File.ReadAllText(filePath)));
         }
      }

      public async Task<bool> AddItemAsync(Training item) {
         if (_trainings.Any(t => t.Id == item.Id)) {
            throw new Exception($"Training with id '{item.Id}' already exists.");
         }

         try {
            File.WriteAllText(Path.Combine(_directory, $"{item.Id}.json"), JsonConvert.SerializeObject(item));
         } catch (Exception e) {
            throw;
         }
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
         File.Delete(Path.Combine(_directory, $"{id}.json"));

         return await Task.FromResult(true);
      }

      public async Task<Training> GetItemAsync(string id) {
         return await Task.FromResult(_trainings.FirstOrDefault(s => s.Id == id));
      }

      public async Task<IEnumerable<Training>> GetItemsAsync(bool forceRefresh = false) {
         return await Task.FromResult(_trainings);
      }

      public Task<bool> AddOrUpdateItemAsync(Training item) {
         throw new NotImplementedException();
      }

      Task<Training> IDataStoreEntity<Training>.AddOrUpdateItemAsync(Training item) {
         throw new NotImplementedException();
      }

      Task<Training> IDataStoreEntity<Training>.AddItemAsync(Training item) {
         throw new NotImplementedException();
      }

      Task<Training> IDataStoreEntity<Training>.UpdateItemAsync(Training item) {
         throw new NotImplementedException();
      }
   }
}