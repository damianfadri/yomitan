using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Yomitan.Core.Models.Deinflects;
using Yomitan.Core.Services;
using Yomitan.Models;
using Yomitan.Service;
using Yomitan.Services.OCR;
using Yomitan.Services.Terms;
using Yomitan.ViewModel;

namespace Yomitan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public App()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<ITextDetector, KanjitomoTextDetector>()
                .AddSingleton<ITextRecognizer, TesseractTextRecognizer>()
                .AddSingleton<IBank<Rule>, RuleBank>(sp => 
                {
                    RuleBank ruleBank = new RuleBank();
                    ruleBank.Load(@"D:\Desktop\deinflect.json");

                    return ruleBank;
                })
                .AddSingleton<ITermSearchService, TermSearchService>()
                .AddSingleton<ITermDisplayService, TermDisplayService>()
                .AddTransient<ITermBankService, TermBankService>()
                .AddTransient<HoverMode>()
                .AddTransient<MainViewModel>()
                .AddTransient<SearchResultsViewModel>()
                .BuildServiceProvider());

            InitializeComponent();
        }
    }
}
