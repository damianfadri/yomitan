using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Yomitan.Constants;
using Yomitan.Contracts;

namespace Yomitan.Services
{
    public class RequiredFilesService : BaseService, IRequiredFilesService
    {
        public async override Task InitializeAsync()
        {
            Directory.CreateDirectory(YomitanFilepaths.ConfigurationDirectoryPath);
            await CopyFile(YomitanFilepaths.DeinflectRulesDefaultFilePath, YomitanFilepaths.DeinflectRulesFilePath);
            await CopyFile(YomitanFilepaths.TagColorsDefaultFilePath, YomitanFilepaths.TagColorsFilePath);

            await base.InitializeAsync();
        }

        private async Task CopyFile(string source, string destination)
        {
            if (File.Exists(destination))
                return;

            var content = await ReadEmbeddedResource(source);
            using (var stream = new FileStream(destination, FileMode.OpenOrCreate))
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(content);
            }
        }

        private async Task<string> ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
