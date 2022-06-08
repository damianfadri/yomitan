﻿using log4net;
using System.Collections.Generic;
using System.Linq;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models.Deinflects;
using Yomitan.Core.Models.Terms;
using Yomitan.Core.Services;
using Yomitan.Models;

namespace Yomitan.Services.Terms
{
    internal class TermSearchService : ITermSearchService
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDictionary<TermKey, TermValues> _terms;
        private IBank<Rule> _ruleBank;

        public TermSearchService(IBank<Rule> ruleBank)
        {
            _terms = new Dictionary<TermKey, TermValues>();
            _ruleBank = ruleBank;
        }

        public void Add(string key, IEnumerable<Term> terms)
        {
            TermKey termKey = new TermKey(key);
            TermValues termValues = new TermValues();
            if (_terms.ContainsKey(termKey))
                termValues = _terms[termKey];

            foreach (Term term in terms)
                termValues.Add(term);

            _terms[termKey] = termValues;
        }

        public IEnumerable<Term> Search(string text)
        {
            IList<Term> results = new List<Term>();

            string current = JapaneseHelper.ConvertKatakanaToHiragana(text);
            while (current.Length > 0)
            {
                Logger.Debug("Searching deinflections...");
                IEnumerable<Deinflection> deinflections = Deinflect(current);

                Logger.Debug("Searching matching terms...");
                IEnumerable<Term> currSearchResults = deinflections.SelectMany(
                        deinflection => Get(deinflection.Text));

                foreach (Term currSearchResult in currSearchResults)
                {
                    Term savedSearchResult = results.FirstOrDefault(result => result.Equals(currSearchResult));
                    if (savedSearchResult != null)
                        savedSearchResult.Merge(currSearchResult);
                    else
                        results.Add(currSearchResult);
                }

                current = current.Substring(0, current.Length - 1);
            }

            return results;
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
                foreach (Rule rule in _ruleBank.FindAll())
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
        private IDictionary<string, IList<Term>> _terms;

        public TermValues()
        {
            _terms = new Dictionary<string, IList<Term>>();
        }

        public void Add(Term term)
        {
            AddTerm(term.Expression.Text, term);
            AddTerm(term.Expression.Reading, term);
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
                Term savedTerm = savedTerms.FirstOrDefault(t => t.Expression.Equals(term.Expression));
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
