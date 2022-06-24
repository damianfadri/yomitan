using CommunityToolkit.Mvvm.DependencyInjection;
using log4net;
using System.Windows;
using Yomitan.ViewModel;

namespace Yomitan.Views
{
    /// <summary>
    /// Interaction logic for SearchResultsWindow.xaml
    /// </summary>
    public partial class SearchResultsWindow : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SearchResultsViewModel ViewModel => (SearchResultsViewModel)DataContext;
        public bool IsClosed { get; private set; }

        public SearchResultsWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<SearchResultsViewModel>();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.Debug("Closing SearchResultWindow...");
            IsClosed = true;
        }
    }
}
