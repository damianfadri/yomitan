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

        public static async Task WriteAllTextAsync(string filepath, string text)
        {
            using (var stream = File.Open(filepath, FileMode.OpenOrCreate))
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public static async Task<bool> ExistsAsync(string path)
        {
            await Task.CompletedTask;
            return File.Exists(path);
        }

        public static async Task DeleteAsync(string path)
        {
            await Task.CompletedTask;
            File.Delete(path);
        }
    }
}
