using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Yomitan.Service;
using Yomitan.Shared.OCR;
using Yomitan.Shared.Repository;
using Yomitan.Shared.Term;

namespace Yomitan.ViewModel
{

    public class MainViewModel : ObservableRecipient
    {
        private ObservableCollection<RepositoryPath> _repositories;

        public ObservableCollection<RepositoryPath> Repositories
        {
            get => _repositories;
            set => SetProperty(ref _repositories, value, true);
        }

        public IAsyncRelayCommand LoadRepositoriesCommand { get; }

        private readonly HoverMode _hoverMode;
        private readonly TermLookupService _termLookup;
        private readonly TermDisplayService _termDisplay;

        public MainViewModel(HoverMode hoverMode, TermLookupService termLookup, TermDisplayService termDisplay) 
        {
            _hoverMode = hoverMode;
            _hoverMode.Hovered += DisplaySearchResults;
            _termLookup = termLookup;
            _termDisplay = termDisplay;

            Repositories = new ObservableCollection<RepositoryPath>();
            LoadRepositoriesCommand = new AsyncRelayCommand(async () =>
            {
                // TODO: Fetch in a configuration file.
                await _termLookup.LoadAsync(new RepositoryPath(@"D:\Desktop\jmdict_english"));
                await _termLookup.LoadAsync(new RepositoryPath(@"D:\Desktop\kireicake"));
            });
        }

        private void DisplaySearchResults(object sender, TextRegion e)
        {
            IEnumerable<Term> results = _termLookup.Search(e.Text);
            _termDisplay.Display(results, e);
        }
    }
}
