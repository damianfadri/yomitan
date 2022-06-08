using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Yomitan.Helpers
{
    public static class FileHelper
    {
        public static string GetFile(string directory, string pattern)
        {
            return GetFiles(directory, pattern).FirstOrDefault();
        }

        public static IEnumerable<string> GetFiles(string directory, string pattern)
        {
            return Directory.EnumerateFiles(directory, pattern, SearchOption.TopDirectoryOnly);
        }
    }
}
