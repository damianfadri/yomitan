using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Constants;
using Yomitan.Core.Contracts;
using Yomitan.Core.Models;
using Yomitan.Repositories;

namespace Yomitan.Services
{
    public class TermBankService : BaseService, ITermBankService
    {
        private readonly ConcurrentDictionary<string, TermRepository> _termBanks;

        public TermBankService()
        {
            _termBanks = new ConcurrentDictionary<string, TermRepository>();
        }

        public async Task<IEnumerable<TermBank>> GetAllAsync()
        {
            var termBanks = new List<TermBank>();
            foreach (var repository in _termBanks.Values)
            {
                var terms = await repository.FindAllAsync();
                var termBank = new TermBank(repository.Model.Title, repository.Model.Revision, terms);

                termBanks.Add(termBank);
            }

            return termBanks;
        }

        public async override Task InitializeAsync()
        {
            string dictionariesDirectory = YomitanFilepaths.DictionaryDirectoryPath;
            var loadTasks = Directory.EnumerateDirectories(dictionariesDirectory)
                    .Select(directoryPath => LoadOneAsync(directoryPath));

            await Task.WhenAll(loadTasks);
            await base.InitializeAsync();
        }

        public async Task<TermBank> LoadOneAsync(string filepath)
        {
            var termRepository = await GetTermRepository(filepath);
            _termBanks.AddOrUpdate(termRepository.Model.Title, termRepository, (key, oldTermRepository) => termRepository);

            var terms = await termRepository.FindAllAsync();
            return new TermBank(termRepository.Model.Title, termRepository.Model.Revision, terms);
        }

        public async Task<TermBank> LoadOneAsync()
        {
            // TODO: Open File Dialog to browse .zip file.
            // TODO: Unzip then load as dictionary.
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        private async Task<TermRepository> GetTermRepository(string filepath)
        {
            string extension = Path.GetExtension(filepath);
            if (".zip".Equals(extension))
                return await new TermRepositoryZipLoader().LoadAsync(filepath);
            else
                return await new TermRepositoryDirectoryLoader().LoadAsync(filepath);
        }
    }

    internal interface ITermRepositoryLoader
    {
        Task<TermRepository> LoadAsync(string filepath);
    }

    internal class TermRepositoryDirectoryLoader : ITermRepositoryLoader
    {
        public async Task<TermRepository> LoadAsync(string filepath)
        {
            var termRepository = new TermRepository();

            await Task.Run(async () => await termRepository.LoadAsync(filepath));

            return termRepository;
        }
    }

    internal class TermRepositoryZipLoader : ITermRepositoryLoader
    {
        public async Task<TermRepository> LoadAsync(string filepath)
        {
            string filename = Path.GetFileNameWithoutExtension(filepath);
            string tempDirectory = Path.Combine(YomitanFilepaths.DictionaryDirectoryPath, filename);
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);

            Directory.CreateDirectory(tempDirectory);
            ZipFile.ExtractToDirectory(filepath, tempDirectory);

            return await new TermRepositoryDirectoryLoader().LoadAsync(tempDirectory);
        }
    }
}
