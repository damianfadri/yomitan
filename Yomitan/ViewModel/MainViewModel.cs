using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Yomitan.Contracts;
using Yomitan.Core.Contracts;
using Yomitan.Core.Models;
using Yomitan.Models;
using Yomitan.Service;

namespace Yomitan.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private readonly ITermBankService _termBankService;
        private readonly ITermSearchService _termSearchService;
        private readonly ITermDisplayService _termDisplayService;
        private readonly IPreferencesService _preferencesService;
        private readonly HoverMode _hoverMode;

        private ObservableCollection<TermBankModel> _dictionaries;

        public ObservableCollection<TermBankModel> TermBanks
        {
            get => _dictionaries;
            set => SetProperty(ref _dictionaries, value);
        }

        public IAsyncRelayCommand InitializeServicesCommand { get; }
        public IAsyncRelayCommand DisposeServicesCommand { get; }
        public IAsyncRelayCommand<IEnumerable<string>> ImportTermBankCommand { get; }
        public IRelayCommand<TermBankModel> ToggleTermBankCommand { get; }

        public MainViewModel(HoverMode hoverMode, ITermBankService termBankService, ITermSearchService termSearchService, ITermDisplayService termDisplayService, IPreferencesService preferencesService)
        {
            _hoverMode = hoverMode;
            _hoverMode.Hovered += DisplaySearchResults;

            _termBankService = termBankService;
            _termSearchService = termSearchService;
            _termDisplayService = termDisplayService;
            _preferencesService = preferencesService;

            InitializeServicesCommand = new AsyncRelayCommand(InitializeServicesAsync);
            DisposeServicesCommand = new AsyncRelayCommand(DisposeAsync);
            ToggleTermBankCommand = new RelayCommand<TermBankModel>(ToggleTermBank);
            ImportTermBankCommand = new AsyncRelayCommand<IEnumerable<string>>(ImportTermBank);
            TermBanks = new ObservableCollection<TermBankModel>();
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

        private void ToggleTermBank(TermBankModel model)
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

            if (!_preferencesService.Loaded)
                await _preferencesService.InitializeAsync();

            foreach (var termBank in await _termBankService.GetAllAsync())
                CacheTermBank(termBank);

            _hoverMode.Start();
        }

        private void CacheTermBank(TermBank termBank)
        {
            _termSearchService.Index(termBank.Title, termBank.Terms);

            if (!_preferencesService.User.TermBanks.ContainsKey(termBank.Title))
                _preferencesService.User.TermBanks[termBank.Title] = new TermBankModel(termBank.Title, termBank.Revision, true);

            var model = _preferencesService.User.TermBanks[termBank.Title];
            if (!model.Enabled)
                _termSearchService.Disable(termBank.Title);

            TermBanks.Add(model);
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;

            _hoverMode.Dispose();
            _termDisplayService.Dispose();
            
            await _preferencesService.SaveAsync();
        }
    }
}
