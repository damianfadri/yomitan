using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Core.Models;
using Yomitan.Service;

namespace Yomitan.ViewModel
{
    public class MainViewModel : ObservableObject, IDisposable
    {
        private readonly ITermBankService _termBankService;
        private readonly ITermSearchService _termSearchService;
        private readonly ITermDisplayService _termDisplayService;
        private readonly HoverMode _hoverMode;

        private ObservableCollection<TermBankViewModel> _dictionaries;

        public ObservableCollection<TermBankViewModel> TermBanks
        {
            get => _dictionaries;
            set => SetProperty(ref _dictionaries, value);
        }

        public IAsyncRelayCommand InitializeServicesCommand { get; }
        public IAsyncRelayCommand<IEnumerable<string>> ImportTermBankCommand { get; }
        public IRelayCommand<TermBankViewModel> ToggleTermBankCommand { get; }

        public MainViewModel(HoverMode hoverMode, ITermBankService termBankService, ITermSearchService termSearchService, ITermDisplayService termDisplayService)
        {
            _hoverMode = hoverMode;
            _hoverMode.Hovered += DisplaySearchResults;

            _termBankService = termBankService;
            _termSearchService = termSearchService;
            _termDisplayService = termDisplayService;

            InitializeServicesCommand = new AsyncRelayCommand(InitializeServicesAsync);
            ToggleTermBankCommand = new RelayCommand<TermBankViewModel>(ToggleTermBank);
            ImportTermBankCommand = new AsyncRelayCommand<IEnumerable<string>>(ImportTermBank);
            TermBanks = new ObservableCollection<TermBankViewModel>();
        }

        private void DisplaySearchResults(object sender, TextRegion e)
        {
            IEnumerable<Term> results = _termSearchService.Search(e.Text);
            _termDisplayService.Display(results, e.Bounds);
        }

        private async Task ImportTermBank(IEnumerable<string> importFilepaths)
        {
            foreach (string importFilepath in importFilepaths)
            {
                var termBank = await _termBankService.LoadOneAsync(importFilepath);
                CacheTermBank(termBank);
            }
        }

        private void ToggleTermBank(TermBankViewModel model)
        {
            if (model.Enabled)
                _termSearchService.Disable(model.Title);
            else
                _termSearchService.Enable(model.Title);
        }

        private async Task InitializeServicesAsync()
        {
            if (!_termBankService.Loaded)
                await _termBankService.InitializeAsync();

            if (!_termSearchService.Loaded)
                await _termSearchService.InitializeAsync();

            foreach (var termBank in await _termBankService.GetAllAsync())
                CacheTermBank(termBank);

            _hoverMode.Start();
        }

        private void CacheTermBank(TermBank termBank)
        {
            _termSearchService.Index(termBank.Title, termBank.Terms);
            
            var model = new TermBankViewModel(termBank);
            TermBanks.Add(model);
        }

        public void Dispose()
        {
            _hoverMode.Dispose();
            _termDisplayService.Dispose();
        }
    }
}
