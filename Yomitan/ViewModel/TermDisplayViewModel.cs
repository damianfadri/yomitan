using CommunityToolkit.Mvvm.ComponentModel;
using log4net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Yomitan.Shared.Term;

namespace Yomitan.ViewModel
{
    public class TermDisplayViewModel : ObservableRecipient
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ObservableCollection<Term> _terms;

        public ObservableCollection<Term> Terms
        {
            get => _terms;
            private set => SetProperty(ref _terms, value);
        }

        public TermDisplayViewModel()
        {
            Terms = new ObservableCollection<Term>();
        }

        public void LoadTerms(IEnumerable<Term> terms)
        {
            Terms.Clear();
            foreach (Term term in terms)
                Terms.Add(term);
        }
    }
}
