using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Shared.Deinflect;
using Yomitan.Shared.Repository;
using Yomitan.Shared.Term;
using Yomitan.Shared.Utils;

namespace Yomitan.Service
{
    public class TermLookupService
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRepository<Term> _termRepository;
        private readonly IRepository<Rule> _ruleRepository;

        public TermLookupService(IRepository<Term> termRepository, IRepository<Rule> ruleRepository) 
        {
            _termRepository = termRepository;
            _ruleRepository = ruleRepository;
            _ruleRepository.Load(new RepositoryPath(@"D:\Desktop\deinflect.json"));
        }

        public async Task LoadAsync(RepositoryPath repositoryPath)
        {
            await Task.Run(() => _termRepository.Load(repositoryPath));
        }

        public IEnumerable<Term> Search(string keyword)
        {
            string current = JapaneseUtils.ConvertKatakanaToHiragana(keyword);
            while (current.Length > 0)
            {
                Logger.Debug("Searching deinflections...");
                IEnumerable<Deinflection> deinflections = Deinflect(current);

                Logger.Debug("Searching matching terms...");
                IEnumerable<Term> currSearchResults = deinflections.SelectMany(
                        deinflection => _termRepository.FindBy(deinflection.Text));

                foreach (Term currSearchResult in currSearchResults)
                    yield return currSearchResult;

                current = current.Substring(0, current.Length - 1);
            }
        }

        private IEnumerable<Deinflection> Deinflect(string raw)
        {
            IList<Deinflection> deinflections = new List<Deinflection>();
            deinflections.Add(new Deinflection(raw));

            for (int i = 0; i < deinflections.Count; i++)
            {
                Deinflection current = deinflections[i];
                foreach (Rule rule in _ruleRepository.FindAll())
                {
                    foreach (RuleVariant variant in rule.Variants)
                    {
                        if ((current.Rules != RuleType.None && ((current.Rules & variant.RulesIn) == RuleType.None))
                                || (current.Text.Length - variant.KanaIn.Length + variant.KanaOut.Length) <= 0
                                || !current.Text.EndsWith(variant.KanaIn))
                            continue;

                        string updatedTerm = current.Text.Substring(
                                0, current.Text.Length - variant.KanaIn.Length) + variant.KanaOut;

                        string[] updatedReasons = current.Reasons != null ? 
                                current.Reasons.Append(rule.Name).ToArray() : new string[] { rule.Name };

                        deinflections.Add(new Deinflection(updatedTerm, variant.RulesOut, updatedReasons));
                    }
                }
            }

            return deinflections;
        }
    }
}
