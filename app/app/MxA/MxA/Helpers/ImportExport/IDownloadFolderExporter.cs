using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MxA.Helpers.ImportExport {
   public interface IDownloadFolderExporter {
      Task<string> ExportAsync(string baseName, string content);
   }
}
