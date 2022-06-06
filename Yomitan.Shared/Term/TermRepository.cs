using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yomitan.Shared.Repository;

namespace Yomitan.Shared.Term
{
    public class TermRepository : IRepository<Term>
    {
        private static readonly IExtractor<TermBankInfo> _termBankInfoExtractor = new TermBankInfoExtractor();

        private static readonly IEnumerable<IExtractor<Tag>> _tagExtractors = new IExtractor<Tag>[]
        {
            new TagV3Extractor(),
        };

        private static readonly IEnumerable<IExtractor<Term>> _termExtractors = new IExtractor<Term>[]
        {
            new TermV3Extractor(),
        };

        private static readonly string _termBankInfoPattern = @"index.json";
        private static readonly string _tagInfoPattern = @"tag_bank_*.json";
        private static readonly string _termsPattern = @"term_bank_*.json";

        private IDictionary<string, IList<Term>> _entries;

        public TermRepository() 
        {
            _entries = new Dictionary<string, IList<Term>>();
        }

        public IEnumerable<Term> FindAll()
        {
            return _entries.Values.SelectMany(terms => terms);
        }

        public IEnumerable<Term> FindBy(string keyword)
        {
            if (!_entries.ContainsKey(keyword))
                yield break;

            foreach (Term term in _entries[keyword].Where(t => t.Expression.Matches(keyword)))
                yield return term;            
        }

        public void Unload(RepositoryPath source)
        {
            _entries.Clear();
        }

        public void Load(RepositoryPath source)
        {
            if (!Directory.Exists(source.Path))
                throw new DirectoryNotFoundException("Term bank directory does not exist.");

            string termBankInfoPath = GetFile(source.Path, _termBankInfoPattern);
            if (!File.Exists(termBankInfoPath))
                throw new FileNotFoundException("index.json not found for term bank.");

            if (!_termBankInfoExtractor.TryExtract(termBankInfoPath, out IEnumerable<TermBankInfo> termBankInfos))
                throw new InvalidDataException("Invalid index.json file.");

            TermBankInfo info = termBankInfos.First();
            Tag dictionaryTag = new Tag(info.Title, "dictionary");

            IDictionary<Tag, Tag> tags = new Dictionary<Tag, Tag>();
            foreach (string tagsPath in GetFiles(source.Path, _tagInfoPattern))
            {
                IEnumerable<Tag> currTags = null;
                IExtractor<Tag> foundExtractor = _tagExtractors.FirstOrDefault(extractor => extractor.TryExtract(tagsPath, out currTags));
                if (foundExtractor == null)
                    continue;

                foreach (Tag tag in currTags)
                    tags[tag] = tag;
            }

            IDictionary<string, IList<Term>> entries = new Dictionary<string, IList<Term>>();
            foreach (string termsPath in GetFiles(source.Path, _termsPattern))
            {
                IEnumerable<Term> currTerms = null;
                IExtractor<Term> foundExtractor = _termExtractors.FirstOrDefault(extractor => extractor.TryExtract(termsPath, out currTerms));
                if (foundExtractor == null)
                    continue;

                foreach (Term term in currTerms.Select(term => UpdateTags(term, dictionaryTag, tags)))
                {
                    IList<Term> savedTerms = null;
                    if (!entries.TryGetValue(term.Expression.Text, out savedTerms))
                    {
                        savedTerms = new List<Term> { term };
                        entries[term.Expression.Text] = savedTerms;

                        if (!entries.ContainsKey(term.Expression.Reading))
                            entries[term.Expression.Reading] = savedTerms;
                        else
                            entries[term.Expression.Reading].Add(term);
                    }

                    // If Expression.Text is already in the dictionary,
                    // then Expression.Reading is already in the dictionary, too.
                    Term savedTerm = savedTerms.FirstOrDefault(t => t.Expression.Equals(term.Expression));
                    if (savedTerm != null)
                        savedTerm.Merge(term);
                    else
                        savedTerms.Add(term);
                }
            }

            _entries = entries;
        }

        private Term UpdateTags(Term term, Tag dictionaryTag, IDictionary<Tag, Tag> tagLookup)
        {
            foreach (Definition definition in term.Definitions)
                definition.PartsOfSpeech = definition.PartsOfSpeech.Concat(new Tag[] { dictionaryTag });

            term.Tags = term.Tags.Select(tag => tagLookup[tag]);
            return term;
        }

        private string GetFile(string directory, string pattern)
        {
            return GetFiles(directory, pattern).FirstOrDefault();
        }

        private IEnumerable<string> GetFiles(string directory, string pattern)
        {
            return Directory.EnumerateFiles(directory, pattern, SearchOption.TopDirectoryOnly);
        }
    }
}
