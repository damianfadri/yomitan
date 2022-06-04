using CommunityToolkit.Mvvm.DependencyInjection;
using log4net;
using System.Windows;
using Yomitan.ViewModel;

namespace Yomitan.View
{
    /// <summary>
    /// Interaction logic for TermDisplayWindow.xaml
    /// </summary>
    public partial class TermDisplayWindow : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TermDisplayViewModel ViewModel => (TermDisplayViewModel)DataContext;

        public TermDisplayWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<TermDisplayViewModel>();
        }

        private void OnWindowCLosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Prevent closing.
            e.Cancel = true;
            Hide();
        }
    }
}
