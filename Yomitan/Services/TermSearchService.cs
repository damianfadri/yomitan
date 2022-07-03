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
        private readonly IDeinflectionService _deinflectionService;

        public TermSearchService(IDeinflectionService deinflectionService)
        {
            _terms = new Dictionary<TermKey, TermValues>();
            _deinflectionService = deinflectionService;
        }

        public async override Task InitializeAsync()
        {
            if (!_deinflectionService.Loaded)
                await _deinflectionService.InitializeAsync();

            await base.InitializeAsync();
        }

        public void Index(string key, IEnumerable<Term> terms)
        {
            var termKey = new TermKey(key);
            var termValues = new TermValues();
            if (_terms.ContainsKey(termKey))
                throw new ArgumentException("Dictionary with the same name already exists.");

            foreach (var term in terms)
                termValues.Add(term);

            _terms[termKey] = termValues;
        }

        public void Unindex(string key)
        {
            var foundKey = _terms.Keys.FirstOrDefault(termKey => termKey.Key.Equals(key));
            if (foundKey == null)
                throw new ArgumentException("Dictionary to be unindexed is not found.");

            _terms.Remove(foundKey);
        }

        public void Enable(string key)
        {
            var foundKey = _terms.Keys.FirstOrDefault(termKey => termKey.Key.Equals(key));
            if (foundKey == null)
                throw new ArgumentException("Dictionary to be enabled is not found.");

            foundKey.Enabled = true;
        }

        public void Disable(string key)
        {
            var foundKey = _terms.Keys.FirstOrDefault(termKey => termKey.Key.Equals(key));
            if (foundKey == null)
                throw new ArgumentException("Dictionary to be disabled is not found.");

            foundKey.Enabled = false;
        }

        public IEnumerable<Term> Search(string text)
        {
            var results = new List<Term>();

            string current = NihongoHelper.ConvertKatakanaToHiragana(text);
            while (current.Length > 0)
            {
                var deinflections = _deinflectionService.Deinflect(current);
                var currSearchResults = deinflections.SelectMany(deinflection => Get(deinflection.Text));

                foreach (var currSearchResult in currSearchResults)
                    yield return currSearchResult;

                current = current.Substring(0, current.Length - 1);
            }
        }

        private IEnumerable<Term> Get(string text)
        {
            foreach (var key in _terms.Keys)
            {
                if (!key.Enabled)
                    continue;

                var values = _terms[key];
                foreach (var term in values.Get(text))
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

            var other = (TermKey)obj;
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

            if (!_terms.TryGetValue(key, out IList<Term> savedTerms))
            {
                savedTerms = new List<Term> { term };
                _terms[key] = savedTerms;
            }
            else
            {
                var savedTerm = savedTerms.FirstOrDefault(t => t.Equals(term));
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
