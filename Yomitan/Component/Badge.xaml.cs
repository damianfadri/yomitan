using System.Windows;
using System.Windows.Controls;
using Media = System.Windows.Media;

namespace Yomitan.Component
{
    /// <summary>
    /// Interaction logic for Badge.xaml
    /// </summary>
    public partial class Badge : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Badge));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Media.Brush), typeof(Badge));
        public static readonly DependencyProperty ContrastColorProperty = DependencyProperty.Register(nameof(ContrastColor), typeof(Media.Brush), typeof(Badge),
                new PropertyMetadata(new Media.SolidColorBrush(Media.Color.FromRgb(255, 255, 255))));

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }
        public Media.Brush Color
        {
            get { return GetValue(ColorProperty) as Media.Brush; }
            set { SetValue(ColorProperty, value); }
        }

        public Media.Brush ContrastColor
        {
            get { return GetValue(ContrastColorProperty) as Media.Brush; }
            set { SetValue(ContrastColorProperty, value); }
        }

        public Badge()
        {
            InitializeComponent();
        }
    }
}
