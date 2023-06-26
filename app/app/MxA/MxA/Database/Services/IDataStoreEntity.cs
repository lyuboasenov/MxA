using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MxA.Database.Services {
   public interface IDataStoreEntity<T> {
      Task<T> AddOrUpdateItemAsync(T item);
      Task<T> AddItemAsync(T item);
      Task<T> UpdateItemAsync(T item);
      Task<bool> DeleteItemAsync(string id);
      Task<T> GetItemAsync(string id);
      Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
   }
}
