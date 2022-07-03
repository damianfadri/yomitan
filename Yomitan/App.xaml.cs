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
                    .AddSingleton<IRequiredFilesService, RequiredFilesService>()
                    .AddSingleton<IPreferencesService, PreferencesService>()
                    .AddSingleton<ITagColorService, TagColorService>()
                    .AddSingleton<IDeinflectionService, DeinflectionService>()
                    .AddSingleton<ITermBankService, TermBankService>()
                    .AddSingleton<ITermSearchService, TermSearchService>()
                    .AddSingleton<ITermDisplayService, TermDisplayService>()
                    .AddSingleton<ITextDetector, KanjitomoTextDetector>()
                    .AddSingleton<ITextRecognizer, TesseractTextRecognizer>()
                    .AddSingleton<IInitializerService, InitializerService>()

                    .AddTransient<HoverMode>()
                    .AddTransient<MainViewModel>()
                    .AddTransient<SearchResultsViewModel>()
                    .BuildServiceProvider());
        }

        private async void InitializeServices(object sender, StartupEventArgs e)
        {
            var configurationService = Ioc.Default.GetService<IInitializerService>();
            if (!configurationService.Loaded)
                await configurationService.InitializeAsync();
        }
    }
}
