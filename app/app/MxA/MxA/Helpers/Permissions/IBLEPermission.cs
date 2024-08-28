using Xamarin.Essentials;
using System.Threading.Tasks;

namespace MxA.Helpers.Permissions {
   public interface IBLEPermission {
      Task<PermissionStatus> CheckStatusAsync();
      Task<PermissionStatus> RequestAsync();
   }
}
