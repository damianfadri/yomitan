using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using Yomitan.Shared.Repository;
using Yomitan.ViewModel;

namespace Yomitan.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _icon;

        public MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainViewModel>();

            MoveWindowToCorner();

            _icon = new NotifyIcon();
            _icon.BalloonTipText = "The app has been minimized";
            _icon.BalloonTipTitle = "Yomitan";
            _icon.Text = "Yomitan";
            _icon.Icon = new System.Drawing.Icon(@"D:\Downloads\icon128.ico");
            _icon.Click += OnIconClick;
            _icon.Visible = true;
        }

        private void OnIconClick(object sender, EventArgs e)
        {
            ToggleWindow();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _icon?.Dispose();
            _icon = null;
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

        private void ToggleWindow()
        {
            if (IsVisible)
                Hide();
            else
                Show();
        }
    }
}
