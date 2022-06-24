using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomitan.Core.Helpers
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

        public static async Task<string> ReadAllTextAsync(string filepath)
        {
            using (var stream = File.Open(filepath, FileMode.Open))
            {
                var result = new byte[stream.Length];
                await stream.ReadAsync(result, 0, (int)stream.Length);

                return Encoding.UTF8.GetString(result);
            }
        }

        public static async Task<bool> Exists(string path)
        {
            await Task.CompletedTask;
            return File.Exists(path);
        }
    }
}
