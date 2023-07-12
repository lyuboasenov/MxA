using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MxA.Helpers {
   public interface IDownloadFolderExporter {
      Task ExportAsync(string baseName, string content);
   }
}
