using System.IO;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models;
using Yomitan.Strategies;

namespace Yomitan.Models
{
    internal class LoadableTermBank : TermBank, ILoadable<TermBank>
    {
        private static readonly string _metadataPattern = @"index.json";

        public async Task LoadAsync(string filepath)
        {
            string termBankInfoPath = FileHelper.GetFile(filepath, _metadataPattern);
            if (!File.Exists(termBankInfoPath))
                throw new FileNotFoundException("index.json not found for term bank.");

            var info = (await GetLoadingStrategy().ExecuteAsync(termBankInfoPath));
            if (info == null)
                throw new InvalidDataException("index.json is not in a valid format.");

            Title = info.Title;
            Revision = info.Revision;
            Format = info.Format;
            Sequenced = info.Sequenced;
        }

        public ILoadingStrategy<TermBank> GetLoadingStrategy()
        {
            return new TermBankLoadingStrategy();
        }
    }
}
