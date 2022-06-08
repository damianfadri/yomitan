using System.Windows;
using System.Windows.Controls;

namespace Yomitan.Extensions
{
    public static class ScrollExtension
    {
        public static readonly DependencyProperty AutoScrollToTopProperty = DependencyProperty.RegisterAttached("AutoScrollToTop",
                typeof(bool), typeof(ScrollExtension), new PropertyMetadata(false, OnAutoScrollToTopPropertyChanged));

        public static bool GetAutoScrollToTop(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToTopProperty);
        }

        public static void SetAutoScrollToTop(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToTopProperty, value);
        }

        private static void OnAutoScrollToTopPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer == null)
                return;

            if ((bool)e.NewValue)
            {
                scrollViewer.ScrollToTop();
                SetAutoScrollToTop(d, false);
            }
        }
    }
}
