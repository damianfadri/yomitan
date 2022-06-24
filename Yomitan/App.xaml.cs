using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Yomitan.Contracts;
using Yomitan.Core.Contracts;
using Yomitan.Service;
using Yomitan.Services;
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
            InitializeComponent();
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            Ioc.Default.ConfigureServices(
                    new ServiceCollection()
                    .AddSingleton<ITagColorService, TagColorService>()
                    .AddSingleton<IRuleBankService, RuleBankService>()
                    .AddSingleton<ITermBankService, TermBankService>()
                    .AddSingleton<ITermSearchService, TermSearchService>()
                    .AddSingleton<ITermDisplayService, TermDisplayService>()
                    .AddSingleton<ITextDetector, KanjitomoTextDetector>()
                    .AddSingleton<ITextRecognizer, TesseractTextRecognizer>()

                    .AddTransient<HoverMode>()
                    .AddTransient<MainViewModel>()
                    .AddTransient<SearchResultsViewModel>()
                    .BuildServiceProvider());
        }
    }
}
