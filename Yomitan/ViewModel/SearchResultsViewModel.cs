using CommunityToolkit.Mvvm.ComponentModel;
using log4net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Yomitan.Core.Models.Terms;

namespace Yomitan.ViewModel
{
    public class SearchResultsViewModel : ObservableRecipient
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ObservableCollection<Term> _terms;
        private bool _hasNewTerms;

        public ObservableCollection<Term> Terms
        {
            get => _terms;
            private set => SetProperty(ref _terms, value);
        }

        public bool HasNewTerms
        {
            get => _hasNewTerms;
            set => SetProperty(ref _hasNewTerms, value);
        }

        public SearchResultsViewModel()
        {
            Terms = new ObservableCollection<Term>();
        }

        public void LoadTerms(IEnumerable<Term> terms)
        {
            Terms.Clear();
            foreach (Term term in terms)
                Terms.Add(term);

            HasNewTerms = true;
        }
    }
}
