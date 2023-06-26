using MxA.Database.Services;
using MxA.Helpers;
using MxA.Services;
using MxA.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MxA.ViewModels {
   public class TrainingViewModel : BaseViewModel {
      private readonly IDataStore _dataStore;

      public Command AddItemCommand { get; }
      public Command ImportWorkoutsCommand { get; }


      public TrainingViewModel() {
         _dataStore = DependencyService.Resolve<IDataStore>() ?? throw new ArgumentNullException(nameof(DataStore));

         Title = "Trainings";
         AddItemCommand = new Command(OnAddItem);
         ImportWorkoutsCommand = new Command(async() => await OnImportWorkoutsAsync());
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
