using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models;

namespace Yomitan.Services
{
    public class TermSearchService : BaseService, ITermSearchService
    {
        private readonly IDictionary<TermKey, TermValues> _terms;
        private readonly IRuleBankService _ruleBankService;
        private IEnumerable<Rule> _rules;

        public TermSearchService(IRuleBankService ruleBankService)
        {
            _terms = new Dictionary<TermKey, TermValues>();
            _ruleBankService = ruleBankService;
        }

        public async override Task InitializeAsync()
        {
            if (!_ruleBankService.Loaded)
                await _ruleBankService.InitializeAsync();

            _rules = await _ruleBankService.GetAllAsync();
            await base.InitializeAsync();
        }

        public void Index(string key, IEnumerable<Term> terms)
        {
            TermKey termKey = new TermKey(key);
            TermValues termValues = new TermValues();
            if (_terms.ContainsKey(termKey))
                throw new ArgumentException("Dictionary with the same name already exists.");

            foreach (Term term in terms)
                termValues.Add(term);

            _terms[termKey] = termValues;
        }

        public void Unindex(string key)
        {
            TermKey foundKey = _terms.Keys.FirstOrDefault(termKey => termKey.Key.Equals(key));
            if (foundKey == null)
                throw new ArgumentException("Dictionary to be unindexed is not found.");

            _terms.Remove(foundKey);
        }

        public void Enable(string key)
        {
            TermKey foundKey = _terms.Keys.FirstOrDefault(termKey => termKey.Key.Equals(key));
            if (foundKey == null)
                throw new ArgumentException("Dictionary to be enabled is not found.");

            foundKey.Enabled = true;
        }

        public void Disable(string key)
        {
            TermKey foundKey = _terms.Keys.FirstOrDefault(termKey => termKey.Key.Equals(key));
            if (foundKey == null)
                throw new ArgumentException("Dictionary to be disabled is not found.");

            foundKey.Enabled = false;
        }

        public IEnumerable<Term> Search(string text)
        {
            IList<Term> results = new List<Term>();

            string current = NihongoHelper.ConvertKatakanaToHiragana(text);
            while (current.Length > 0)
            {
                IEnumerable<Deinflection> deinflections = Deinflect(current);

                IEnumerable<Term> currSearchResults = deinflections.SelectMany(
                        deinflection => Get(deinflection.Text));

                foreach (Term currSearchResult in currSearchResults)
                    yield return currSearchResult;

                current = current.Substring(0, current.Length - 1);
            }
        }

        public IEnumerable<Deinflection> Deinflect(string raw)
        {
            IList<Deinflection> deinflections = new List<Deinflection>
            {
                new Deinflection(raw),
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

                        string[] updatedReasons = current.Reasons != null ?
                                current.Reasons.Append(rule.Name).ToArray() : new string[] { rule.Name };

                        deinflections.Add(new Deinflection(updatedTerm, variant.RulesOut, updatedReasons));
                    }
                }
            }

            return deinflections;
        }

        private IEnumerable<Term> Get(string text)
        {
            foreach (TermKey key in _terms.Keys)
            {
                if (!key.Enabled)
                    continue;

                TermValues values = _terms[key];
                foreach (Term term in values.Get(text))
                    yield return term;
            }
        }
    }

    internal class TermKey
    {
        public string Key { get; set; }
        public bool Enabled { get; set; }

        public TermKey(string key) : this(key, true) { }

        public TermKey(string key, bool enabled)
        {
            Key = key;
            Enabled = enabled;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TermKey))
                return false;

            TermKey other = (TermKey)obj;
            return Key.Equals(other.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    internal class TermValues
    {
        private readonly IDictionary<string, IList<Term>> _terms;

        public TermValues()
        {
            _terms = new Dictionary<string, IList<Term>>();
        }

        public void Add(Term term)
        {
            AddTerm(term.Text, term);
            AddTerm(term.Reading, term);
        }

        private void AddTerm(string key, Term term)
        {
            if (string.IsNullOrEmpty(key))
                return;

            IList<Term> savedTerms = null;
            if (!_terms.TryGetValue(key, out savedTerms))
            {
                savedTerms = new List<Term> { term };
                _terms[key] = savedTerms;
            }
            else
            {
                Term savedTerm = savedTerms.FirstOrDefault(t => t.Equals(term));
                if (savedTerm != null)
                    savedTerm.Merge(term);
                else
                    savedTerms.Add(term);
            }
        }

        public IList<Term> Get(string keyword)
        {
            return _terms.ContainsKey(keyword) ? _terms[keyword] : new List<Term>();
        }
    }
}
