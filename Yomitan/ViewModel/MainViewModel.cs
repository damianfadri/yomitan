using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Yomitan.Service;
using Yomitan.Shared.OCR;
using Yomitan.Shared.Repository;
using Yomitan.Shared.Term;

namespace Yomitan.ViewModel
{

    public class MainViewModel : ObservableRecipient
    {
        private static readonly string[] _strRepositoryPaths = new string[]
        {
            @"D:\Desktop\jmdict_english",
            @"D:\Desktop\kireicake",
        };

        private ObservableCollection<RepositoryPath> _repositories;
        private bool _isRepositoriesLoaded;

        public ObservableCollection<RepositoryPath> Repositories
        {
            get => _repositories;
            set => SetProperty(ref _repositories, value);
        }

        public bool IsRepositoriesLoaded
        {
            get => _isRepositoriesLoaded;
            set => SetProperty(ref _isRepositoriesLoaded, value);
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
            LoadRepositoriesCommand = new AsyncRelayCommand(async () => await LoadDictionaries());
        }

        private async Task LoadDictionaries()
        {
            StopServices();

            foreach (string strRepositoryPath in _strRepositoryPaths)
            {
                RepositoryPath repositoryPath = new RepositoryPath(strRepositoryPath);
                await _termLookup.LoadAsync(repositoryPath);
                Repositories.Add(new RepositoryPath(strRepositoryPath));
            }

            StartServices();
        }

        private void StartServices()
        {
            IsRepositoriesLoaded = true;
            _hoverMode?.Start();
        }

        private void StopServices()
        {
            IsRepositoriesLoaded = false;
            _hoverMode?.Stop();
        }

        private void DisplaySearchResults(object sender, TextRegion e)
        {
            IEnumerable<Term> results = _termLookup.Search(e.Text);
            _termDisplay.Display(results, e);
        }
    }
}
