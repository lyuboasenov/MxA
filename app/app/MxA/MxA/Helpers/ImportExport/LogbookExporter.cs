using MxA.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MxA.Helpers.ImportExport {
   internal class LogbookExporter {
      public static Task ExportAsync(ActivityLog log, IEnumerable<TimerEvent> events) {
         return ExportAsync(new[] { log }, events);
      }
      public static async Task ExportAsync(IEnumerable<ActivityLog> logs, IEnumerable<TimerEvent> events) {
         var exportBundle = new {
            LogEntries = logs,
            Events = events,
         };

         var exporter = DependencyService.Get<IDownloadFolderExporter>();
         var result = await exporter.ExportAsync($"mxa-logbook-export-{DateTime.Now.ToString("yyyy.MM.dd.HH.mm")}.json", JsonConvert.SerializeObject(exportBundle, Formatting.None));

         await App.Current.MainPage.DisplayAlert("Export", $"Log(s) exported at {result}.", "Ok");
      }
   }
}
