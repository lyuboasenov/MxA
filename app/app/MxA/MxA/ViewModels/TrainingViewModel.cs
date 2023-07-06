using MxA.Database.Services;
using MxA.Helpers;
using MxA.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MxA.ViewModels {
   public class TrainingViewModel : BaseViewModel {
      private readonly IDataStore _dataStore;
      public bool IsSearchBoxVisible { get; set; }
      public string SearchTerm { get; set; }

      public Command AddItemCommand { get; }
      public Command ToggleSearchBoxCommand { get; }
      public Command ImportWorkoutsCommand { get; }


      public TrainingViewModel() {
         _dataStore = DependencyService.Resolve<IDataStore>() ?? throw new ArgumentNullException(nameof(DataStore));

         Title = "Trainings";
         AddItemCommand = new Command(OnAddItem);
         ImportWorkoutsCommand = new Command(async() => await OnImportWorkoutsAsync());
         ToggleSearchBoxCommand = new Command(() => { IsSearchBoxVisible = !IsSearchBoxVisible; });
      }

      private async Task OnImportWorkoutsAsync() {
         try {
            var result = await FilePicker.PickAsync();
            if (result != null && result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase)) {
               using (var stream = await result.OpenReadAsync()) {
                  await WorkoutImporter.Import(stream, _dataStore);
               }
            }
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
      }

      internal void OnAppearing() {
      }

      private async void OnAddItem(object obj) {
         await Shell.Current.GoToAsync(nameof(NewItemPage));
      }
   }
}
