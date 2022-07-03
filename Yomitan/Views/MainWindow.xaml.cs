using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Forms;
using Yomitan.ViewModel;

namespace Yomitan.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            MoveWindowToCorner();

            DataContext = Ioc.Default.GetService<MainViewModel>();
        }

        private void OnWindowContentRendered(object sender, EventArgs e)
        {
            MoveWindowToCorner();
        }

        private void MoveWindowToCorner()
        {
            Left = SystemParameters.WorkArea.Width - Width;
            Top = SystemParameters.WorkArea.Height - Height;
        }
    }
}
