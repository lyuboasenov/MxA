using MxA.Helpers.ImportExport;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(MxA.Droid.DownloadFolderExporter))]

namespace MxA.Droid {
   internal class DownloadFolderExporter : IDownloadFolderExporter {
      public Task<string> ExportAsync(string baseName, string content) {
         string downloads = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
         string location = Path.Combine(downloads, baseName);
         File.WriteAllText(location, content);

         return Task.FromResult(location);
      }
   }
}