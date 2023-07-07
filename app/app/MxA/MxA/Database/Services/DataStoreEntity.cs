using MxA.Database.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MxA.Database.Services {
   public class DataStoreEntity<T> : IDataStoreEntity<T> where T : IModel, new() {
      private readonly DataStore _dataStore;

      internal DataStoreEntity(DataStore dataStore) {
         _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
      }

      public async Task<T> AddOrUpdateItemAsync(T item) {
         if (string.IsNullOrEmpty(item.Id)) {
            return await AddItemAsync(item);
         } else {
            var existingItem = await GetItemAsync(item.Id);
            if (null == existingItem) {
               return await AddItemAsync(item);
            } else {
               return await UpdateItemAsync(item);
            }
         }
      }

      public async Task<T> AddItemAsync(T item) {
         var database = await GetDatabase();
         if (string.IsNullOrEmpty(item.Id)) {
            item.Id = Guid.NewGuid().ToString("N").ToLower();
         }
         if (item is IVersion ver) {
            ver.Active = true;
            ver.Created = DateTime.Now;
            ver.Updated = DateTime.Now;
            ver.Version = 0;
         }
         await database.InsertAsync(item);
         return await GetItemAsync(item.Id);
      }

      public async Task<T> UpdateItemAsync(T item) {
         var database = await GetDatabase();
         var id = item.Id;
         if (item is IVersion verNew) {
            // Disable existing item
            var existingItem = await GetItemAsync(item.Id);

            existingItem.Id = $"{id}_inactive_{Guid.NewGuid().ToString("N")}";
            var ver = existingItem as IVersion;
            ver.Updated = DateTime.Now;
            ver.Active = false;
            await database.InsertAsync(existingItem);

            // Add new item
            item.Id = id;
            verNew.Updated = DateTime.Now;
            verNew.Active = true;
            verNew.Version++;

            await database.UpdateAsync(item);
         } else {
            await database.UpdateAsync(item);
         }

         return await GetItemAsync(item.Id);
      }

      public async Task<bool> DeleteItemAsync(string id) {
         var database = await GetDatabase();
         T item = await GetItemAsync(id);
         if (item != null) {
            if (item is IVersion vers) {
               //item.Id = $"{id}_inactive_{Guid.NewGuid().ToString("N")}";
               vers.Updated = DateTime.Now;
               vers.Active = false;

               return await database.UpdateAsync(item) == 1;
            } else {
               return await database.DeleteAsync(item) == 1;
            }
         } else {
            return false;
         }
      }

      public async Task<T> GetItemAsync(string id) {
         var database = await GetDatabase();
         bool isVers = typeof(IVersion).IsAssignableFrom(typeof(T));
         // return await database.Table<T>().Where(i => i.Id == id && (isVers ? ((IVersion) i).Active : true)).FirstOrDefaultAsync();
         return await database.Table<T>().Where(i => i.Id == id).FirstOrDefaultAsync();
      }

      public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false) {
         var database = await GetDatabase();
         bool isVers = typeof(IVersion).IsAssignableFrom(typeof(T));
         if (isVers) {
            return await database.Table<T>().Where(i => ((IVersion) i).Active).ToArrayAsync();
         } else {
            return await database.Table<T>().ToArrayAsync();
         }
      }

      public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predExpr, bool forceRefresh = false) {
         var database = await GetDatabase();
         bool isVers = typeof(IVersion).IsAssignableFrom(typeof(T));
         if (isVers) {
            return await database.Table<T>().Where(predExpr).Where(i => ((IVersion) i).Active).ToArrayAsync();
         } else {
            return await database.Table<T>().Where(predExpr).ToArrayAsync();
         }
      }

      private Task<SQLiteAsyncConnection> GetDatabase() {
         return _dataStore.Database;
      }
   }
}
