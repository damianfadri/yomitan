using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Constants;
using Yomitan.Core.Contracts;
using Yomitan.Core.Models;
using Yomitan.Repositories;

namespace Yomitan.Services
{
    public class DeinflectionService : BaseService, IDeinflectionService
    {
        private readonly RuleRepository _ruleRepository;
        private IEnumerable<Rule> _rules;

        public DeinflectionService()
        {
            _ruleRepository = new RuleRepository();
        }

        public async override Task InitializeAsync()
        {
            await Task.Run(async () =>
            {
                string rulesFilepath = YomitanFilepaths.DeinflectRulesFilePath;
                await _ruleRepository.LoadAsync(rulesFilepath);
            });

            _rules = await _ruleRepository.FindAllAsync();
            await base.InitializeAsync();
        }

        public IEnumerable<Deinflection> Deinflect(string text)
        {
            IList<Deinflection> deinflections = new List<Deinflection>
            {
                new Deinflection(text),
            };

            for (int i = 0; i < deinflections.Count; i++)
            {
                Deinflection current = deinflections[i];
                foreach (Rule rule in _rules)
                {
                    foreach (RuleVariant variant in rule.Variants)
                    {
                        if ((current.Rules != RuleType.None && ((current.Rules & variant.RulesIn) == RuleType.None))
                                || (current.Text.Length - variant.KanaIn.Length + variant.KanaOut.Length) <= 0
                                || !current.Text.EndsWith(variant.KanaIn))
                            continue;

                        string updatedTerm = current.Text.Substring(
                                0, current.Text.Length - variant.KanaIn.Length) + variant.KanaOut;

                        string[] updatedReasons = current.Reasons != null 
                                ? current.Reasons.Append(rule.Name).ToArray() 
                                : new string[] { rule.Name };

                        deinflections.Add(new Deinflection(updatedTerm, variant.RulesOut, updatedReasons));
                    }
                }
            }

            return deinflections;
        }
    }
}
