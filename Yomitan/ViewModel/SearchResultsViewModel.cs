using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using log4net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Yomitan.Contracts;
using Yomitan.Core.Models;
using Yomitan.Models;

namespace Yomitan.ViewModel
{
    public class SearchResultsViewModel : ObservableRecipient
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ITagColorService _tagColorService;

        private ObservableCollection<TermModel> _terms;

        public ObservableCollection<TermModel> Terms
        {
            get => _terms;
            private set => SetProperty(ref _terms, value);
        }

        public IAsyncRelayCommand InitializeServicesCommand { get; }

        public SearchResultsViewModel(ITagColorService tagColorService)
        {
            _tagColorService = tagColorService;

            Terms = new ObservableCollection<TermModel>();
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
            foreach (var term in terms.Select(t => new TermModel(t)))
                Terms.Add(term);
        }
    }
}
