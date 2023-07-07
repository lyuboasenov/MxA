using MxA.Helpers;
using MxA.Icons.Material;
using MxA.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace MxA.ViewModels {
   public class TrainingsViewModel : BaseViewModel {
      private Lazy<WorkoutsViewModel> _workoutsViewModel = new Lazy<WorkoutsViewModel>(() => new WorkoutsViewModel());
      private Lazy<ExercisesViewModel> _exercisesViewModel = new Lazy<ExercisesViewModel>(() => new ExercisesViewModel());
      public bool IsSearchBoxVisible { get; set; }
      public string SearchTerm { get; set; }
      public string SearchGlyph { get; set; }

      public WorkoutsViewModel WorkoutsViewModel { get { return _workoutsViewModel.Value; } }
      public ExercisesViewModel ExercisesViewModel { get { return _exercisesViewModel.Value; } }

      public Command AddItemCommand { get; }
      public Command ToggleSearchBoxCommand { get; }
      public Command ImportWorkoutsCommand { get; }
      public ICommand CheckPermissionsCommand { get; }
      public ICommand SearchCommand { get; }


      public TrainingsViewModel() {
         Title = "Trainings";
         AddItemCommand = new Command(OnAddItem);
         ImportWorkoutsCommand = new Command(async() => await OnImportWorkoutsAsync());
         ToggleSearchBoxCommand = new Command(async() => await OnToggleSearchBoxCommand());
         CheckPermissionsCommand = new Command(async () => await OnCheckPermissionsCommand());
         SearchCommand = new Command<string>(async (s) => await OnSearchCommand(s));
         SearchGlyph = IconFont.Search;
      }

      public void OnSearchTermChanged() {

      }

      private Task OnSearchCommand(string s) {
         SearchTerm = s;
         if (_workoutsViewModel.IsValueCreated) {
            _workoutsViewModel.Value.SearchTerm = SearchTerm;
         }
         if (_exercisesViewModel.IsValueCreated) {
            _exercisesViewModel.Value.SearchTerm = SearchTerm;
         }

         return Task.CompletedTask;
      }

      private async Task OnCheckPermissionsCommand() {
         await CheckAndRequestPermission<Permissions.LocationWhenInUse>();
         await CheckAndRequestPermission<Permissions.StorageRead>();
         await CheckAndRequestPermission<Permissions.StorageWrite>();
      }

      private async Task<PermissionStatus> CheckAndRequestPermission<T>() where T : BasePermission, new() {
         try {
            var status = await Permissions.CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted) {
               status = await Permissions.RequestAsync<T>();
            }

            return status;
         } catch { }

         return PermissionStatus.Unknown;
      }

      private Task OnToggleSearchBoxCommand() {
         IsSearchBoxVisible = !IsSearchBoxVisible;
         SearchGlyph = IsSearchBoxVisible ? IconFont.Search_off : IconFont.Search;

         return Task.CompletedTask;
      }

      private async Task OnImportWorkoutsAsync() {
         try {
            var result = await FilePicker.PickAsync();
            if (result != null && result.FileName.EndsWith("json", StringComparison.OrdinalIgnoreCase)) {
               using (var stream = await result.OpenReadAsync()) {
                  await WorkoutImporter.Import(stream, DataStore);
               }
            }
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
      }

      internal void OnAppearing() {
         CheckPermissionsCommand?.Execute(null);
      }

      private async void OnAddItem(object obj) {
         await Shell.Current.GoToAsync(nameof(NewItemPage));
      }
   }
}
