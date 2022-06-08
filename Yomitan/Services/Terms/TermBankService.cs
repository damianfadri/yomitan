using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Compression;
using Yomitan.Core.Services;

namespace Yomitan.Services.Terms
{
    internal class TermBankService : ITermBankService
    {
        private static readonly string EXTRACT_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Yomitan", "Dictionaries");

        public string Import()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Import dictionary...",
                DefaultExt = ".zip",
                Filter = ".zip files (.zip)|*.zip"
            };

            bool? result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return null;

            string zipFilepath = dialog.FileName;

            string destFilepath = Path.Combine(EXTRACT_PATH, Guid.NewGuid().ToString());
            ZipFile.ExtractToDirectory(zipFilepath, destFilepath);

            return destFilepath;
        }

        public void Purge(string name)
        {
            throw new NotImplementedException();
        }
    }
}
