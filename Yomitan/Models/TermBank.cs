using CommunityToolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Core.Models.Terms;
using Yomitan.Core.Services;
using Yomitan.Helpers;
using Yomitan.Services.Extractors;

namespace Yomitan.Models
{
    public class TermBank : IBank<Term>
    {
        private static readonly string _termBankInfoPattern = @"index.json";

        private static readonly string _termsPattern = @"term_bank_*.json";

        public TermBankInfo Info { get; set; }
        
        private TagBank _tagBank;
        private IEnumerable<Term> _terms;

        public bool Enabled { get; set; }

        public TermBank(string dictionaryPath)
        {
            if (!Directory.Exists(dictionaryPath))
                throw new DirectoryNotFoundException("Term bank directory does not exist.");

            _tagBank = new TagBank(dictionaryPath);

            LoadInfo(dictionaryPath);
            Load(dictionaryPath);   
        }

        public static async Task<TermBank> LoadAsync(string termBankPath)
        {
            return await Task.Run(() => new TermBank(termBankPath));
        }

        public void Load(string dictionaryPath)
        {
            IList<Term> terms = new List<Term>();
            foreach (string termsPath in FileHelper.GetFiles(dictionaryPath, _termsPattern))
            {
                IEnumerable<Term> currTerms = null;
                IExtractor<Term> foundExtractor = GetTermExtractors()
                        .FirstOrDefault(extractor => extractor.TryExtract(termsPath, out currTerms));

                if (foundExtractor == null)
                    continue;

                foreach (Term term in currTerms)
                    terms.Add(term);
            }

            _terms = terms;
        }

        public IEnumerable<Term> FindAll()
        {
            return _terms;
        }

        public IEnumerable<Term> FindBy(string keyword)
        {
            return _terms.Where(term => term.Expression.Matches(keyword));
        }

        private IExtractor<TermBankInfo> GetTermBankInfoExtractor()
        {
            return new TermBankInfoExtractor();
        }

        private IExtractor<Term>[] GetTermExtractors()
        {
            return new IExtractor<Term>[]
            {
                new TermV3Extractor(Info, _tagBank),
            };
        }

        private void LoadInfo(string dictionaryPath)
        {
            string termBankInfoPath = FileHelper.GetFile(dictionaryPath, _termBankInfoPattern);
            if (!File.Exists(termBankInfoPath))
                throw new FileNotFoundException("index.json not found for term bank.");

            if (!GetTermBankInfoExtractor().TryExtract(termBankInfoPath, out IEnumerable<TermBankInfo> termBankInfos))
                throw new InvalidDataException("Invalid index.json file.");

            Info = termBankInfos.FirstOrDefault();
        }
    }
}
