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

         await database.InsertAsync(item);
         return await GetItemAsync(item.Id);
      }

      public async Task<T> UpdateItemAsync(T item) {
         var database = await GetDatabase();
         await database.UpdateAsync(item);

         return await GetItemAsync(item.Id);
      }

      public async Task<bool> DeleteItemAsync(string id) {
         var database = await GetDatabase();
         T item = await GetItemAsync(id);
         if (item != null) {
            return await database.DeleteAsync(item) == 1;
         } else {
            return false;
         }
      }

      public async Task<T> GetItemAsync(string id) {
         var database = await GetDatabase();
         return await database.Table<T>().Where(i => i.Id == id).FirstOrDefaultAsync();
      }

      public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false) {
         var database = await GetDatabase();
         return await database.Table<T>().ToArrayAsync();
      }

      public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predExpr, bool forceRefresh = false) {
         var database = await GetDatabase();
         return await database.Table<T>().Where(predExpr).ToArrayAsync();
      }

      private Task<SQLiteAsyncConnection> GetDatabase() {
         return _dataStore.Database;
      }
   }
}
