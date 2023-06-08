using PortableLoadCell.Models;
using PortableLoadCell.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PortableLoadCell.ViewModels {
   public class ItemsViewModel : BaseViewModel {
      public ObservableCollection<Training> Items { get; }
      public Command LoadItemsCommand { get; }
      public Command AddItemCommand { get; }
      public Command<Training> ItemTapped { get; }

      public ItemsViewModel() {
         Title = "Browse";
         Items = new ObservableCollection<Training>();
         LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

         ItemTapped = new Command<Training>(OnItemSelected);

         AddItemCommand = new Command(OnAddItem);
      }

      async Task ExecuteLoadItemsCommand() {
         IsBusy = true;

         try {
            Items.Clear();
            var items = await DataStore.GetItemsAsync(true);
            foreach (var item in items) {
               Items.Add(item);
            }
         } catch (Exception ex) {
            Debug.WriteLine(ex);
         } finally {
            IsBusy = false;
         }
      }

      public void OnAppearing() {
         IsBusy = true;
         SelectedItem = null;
      }

      public Training SelectedItem { get; set; }

      public void OnSelectedItemChanged() {
         OnItemSelected(SelectedItem);
      }

      private async void OnAddItem(object obj) {
         await Shell.Current.GoToAsync(nameof(NewItemPage));
      }

      async void OnItemSelected(Training item) {
         if (item == null)
            return;

         // This will push the ItemDetailPage onto the navigation stack
         await Shell.Current.GoToAsync($"{nameof(TimerPage)}?{nameof(TimerViewModel.TrainingId)}={item.Id}");
      }
   }
}