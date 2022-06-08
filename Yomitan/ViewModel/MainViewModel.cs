using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using log4net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Yomitan.Core.Models.OCR;
using Yomitan.Core.Models.Terms;
using Yomitan.Core.Services;
using Yomitan.Models;
using Yomitan.Service;

namespace Yomitan.ViewModel
{

    public class MainViewModel : ObservableRecipient
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string[] _strTermBankPaths = new string[]
        {
            @"D:\Desktop\jmdict_english",
            @"D:\Desktop\kireicake",
        };

        private ObservableCollection<TermBank> _termBanks;
        private bool _isTermBanksLoaded;

        public ObservableCollection<TermBank> TermBanks
        {
            get => _termBanks;
            set => SetProperty(ref _termBanks, value);
        }

        public bool IsTermBanksLoaded
        {
            get => _isTermBanksLoaded;
            set => SetProperty(ref _isTermBanksLoaded, value);
        }

        public IAsyncRelayCommand LoadTermBanksCommand { get; }
        public IAsyncRelayCommand ImportTermBankCommand { get; }

        private readonly HoverMode _hoverMode;
        private readonly ITermSearchService _termSearch;
        private readonly ITermDisplayService _termDisplay;
        private readonly ITermBankService _termBankService;

        public MainViewModel(HoverMode hoverMode, ITermSearchService termSearchService, ITermDisplayService termDisplayService, ITermBankService termBankService)
        {
            _hoverMode = hoverMode;
            _hoverMode.Hovered += DisplaySearchResults;

            _termSearch = termSearchService;
            _termDisplay = termDisplayService;
            _termBankService = termBankService;

            TermBanks = new ObservableCollection<TermBank>();
            LoadTermBanksCommand = new AsyncRelayCommand(async () => await LoadTermBanks());
            ImportTermBankCommand = new AsyncRelayCommand(async () => await ImportTermBank());
        }

        private async Task ImportTermBank()
        {
            string extractedTermBankPath = _termBankService.Import();
            if (string.IsNullOrWhiteSpace(extractedTermBankPath))
                return;

            await ImportTermBank(extractedTermBankPath);
        }

        private async Task ImportTermBank(string termBankPath)
        {
            TermBank termBank = await TermBank.LoadAsync(termBankPath);
            TermBanks.Add(termBank);

            _termSearch.Add(termBank.Info.Title, termBank.FindAll());
        }

        private async Task LoadTermBanks()
        {
            StopServices();


            foreach (string termBankPath in _strTermBankPaths)
                await ImportTermBank(termBankPath);

            StartServices();
        }

        private void StartServices()
        {
            IsTermBanksLoaded = true;
            _hoverMode?.Start();
        }

        private void StopServices()
        {
            IsTermBanksLoaded = false;
            _hoverMode?.Stop();
        }

        private void DisplaySearchResults(object _, TextRegion e)
        {
            IEnumerable<Term> results = _termSearch.Search(e.Text);
            
            _termDisplay.Display(results, e.Bounds);
        }
    }
}
