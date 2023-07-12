using MxA.Helpers;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(MxA.Droid.DownloadFolderExporter))]

namespace MxA.Droid {
   internal class DownloadFolderExporter : IDownloadFolderExporter {
      public Task ExportAsync(string baseName, string content) {
         string downloads = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
         File.WriteAllText(Path.Combine(downloads, baseName), content);

         return Task.CompletedTask;
      }
   }
}