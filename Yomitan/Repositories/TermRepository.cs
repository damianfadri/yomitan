using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models;
using Yomitan.Models;
using Yomitan.Strategies;

namespace Yomitan.Repositories
{
    public class TermRepository : IRepository<Term>
    {
        private static readonly string _termsPattern = @"term_bank_*.json";

        private readonly TermBank _model;

        public TermBank Model 
        { 
            get
            {
                return _model; 
            } 
        }

        public TermRepository()
        {
            _model = new TermBank();
        }

        public async Task<IEnumerable<Term>> FindAllAsync()
        {
            await Task.CompletedTask;
            return _model.Terms;
        }

        public async Task<IEnumerable<Term>> FindByAsync(string keyword)
        {
            return (await FindAllAsync()).Where(term => term.Matches(keyword));
        }

        public async Task LoadAsync(string filepath)
        {
            if (!Directory.Exists(filepath))
                throw new DirectoryNotFoundException("Term bank directory does not exist.");

            var metadata = new LoadableTermBank();
            await metadata.LoadAsync(filepath);

            _model.Title = metadata.Title;
            _model.Revision = metadata.Revision;
            _model.Format = metadata.Format;
            _model.Sequenced = metadata.Sequenced;

            var tagRepository = new TagRepository();
            await tagRepository.LoadAsync(filepath);

            var dictionaryTag = new Tag(_model.Title, "dictionary");
            var dictionaryCache = new ConcurrentDictionary<Term, Term>();
            var extractTasks = FileHelper.GetFiles(filepath, _termsPattern).Select(async termBankPath =>
            {
                IEnumerable<Term> currTerms = await GetLoadingStrategy().ExecuteAsync(termBankPath);
                foreach (var currTerm in currTerms)
                {
                    currTerm.UpdateTags(tagRepository);
                    currTerm.AddDictionaryTag(dictionaryTag);

                    dictionaryCache.AddOrUpdate(currTerm, currTerm, (key, oldTerm) => {
                        oldTerm.Merge(currTerm);
                        return oldTerm;
                    });
                }
            });

            await Task.WhenAll(extractTasks);

            _model.Terms = dictionaryCache.Values;
        }

        public ILoadingStrategy<IEnumerable<Term>> GetLoadingStrategy()
        {
            return new TermV3LoadingStrategy();
        }
    }

    internal static class TermExtensions
    {
        public static void AddDictionaryTag(this Term term, Tag dictionaryTag)
        {
            foreach (var currTermDefinition in term.DefinitionEntries)
                currTermDefinition.Source = dictionaryTag;
        }

        public async static void UpdateTags(this Term term, IRepository<Tag> tagRepository)
        {
            foreach (var originalTag in term.Tags)
            {
                var results = await tagRepository.FindByAsync(originalTag.Name);
                var completeTag = results.FirstOrDefault();
                if (completeTag == null)
                    continue;

                originalTag.Update(completeTag);
            }
        }
    }
}
