using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Yomitan.Kanjitomo.OCR;
using Yomitan.Service;
using Yomitan.Shared.Deinflect;
using Yomitan.Shared.OCR;
using Yomitan.Shared.Repository;
using Yomitan.Shared.Term;
using Yomitan.Tesseract.OCR;
using Yomitan.View;
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
                .AddSingleton<IDetector, KanjiTomoDetector>()
                .AddSingleton<IRecognizer, TesseractRecognizer>()
                .AddSingleton<IRepository<Term>, TermRepositoryCollection>()
                .AddSingleton<IRepository<Rule>, RuleRepository>()
                .AddTransient<HoverMode>()
                .AddTransient<TermLookupService>()
                .AddTransient<TermDisplayService>()
                .AddTransient<MainViewModel>()
                .AddTransient<TermDisplayViewModel>()
                .BuildServiceProvider());

            InitializeComponent();
        }
    }
}
