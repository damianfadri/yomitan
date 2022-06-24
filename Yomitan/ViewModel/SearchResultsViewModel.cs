using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using log4net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Contracts;
using Yomitan.Core.Models;

namespace Yomitan.ViewModel
{
    public class SearchResultsViewModel : ObservableRecipient
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ITagColorService _tagColorService;

        private ObservableCollection<TermViewModel> _terms;
        private bool _hasNewTerms;

        public ObservableCollection<TermViewModel> Terms
        {
            get => _terms;
            private set => SetProperty(ref _terms, value);
        }

        public bool HasNewTerms
        {
            get => _hasNewTerms;
            set => SetProperty(ref _hasNewTerms, value);
        }

        public IAsyncRelayCommand InitializeServicesCommand { get; }

        public SearchResultsViewModel(ITagColorService tagColorService)
        {
            _tagColorService = tagColorService;

            Terms = new ObservableCollection<TermViewModel>();
            InitializeServicesCommand = new AsyncRelayCommand(InitializeServices);
        }

        private async Task InitializeServices()
        {
            if (!_tagColorService.Loaded)
                await _tagColorService.InitializeAsync();
        }

        public void LoadTerms(IEnumerable<Term> terms)
        {
            Terms.Clear();
            foreach (var term in terms.Select(t => new TermViewModel(t)))
                Terms.Add(term);

            HasNewTerms = true;
        }
    }
}
