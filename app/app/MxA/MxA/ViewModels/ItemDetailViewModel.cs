using MxA.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MxA.ViewModels {
   [QueryProperty(nameof(ItemId), nameof(ItemId))]
   public class ItemDetailViewModel : BaseViewModel {
      public string Id { get; set; }
      public string Text { get; set; }
      public string Description { get; set; }

      public string ItemId { get; set; }

      public void OnItemIdChanged() {

         LoadItemId(ItemId);
      }

      public async void LoadItemId(string itemId) {
         try {
            var item = await DataStore.GetItemAsync(itemId);
            Id = item.Id;
            //Text = item.Text;
            //Description = item.Description;
         } catch (Exception) {
            Debug.WriteLine("Failed to Load Item");
         }
      }
   }
}
