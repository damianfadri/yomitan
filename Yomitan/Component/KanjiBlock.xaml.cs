using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yomitan.Component
{
    /// <summary>
    /// Interaction logic for KanjiBlock.xaml
    /// </summary>
    public partial class KanjiBlock : UserControl
    {
        public static readonly DependencyProperty FuriganaProperty = DependencyProperty.Register(nameof(Furigana), typeof(string), typeof(KanjiBlock));
        public static readonly DependencyProperty KanjiProperty = DependencyProperty.Register(nameof(Kanji), typeof(string), typeof(KanjiBlock));

        public KanjiBlock()
        {
            InitializeComponent();
        }

        public string Furigana
        {
            get { return GetValue(FuriganaProperty) as string; }
            set { SetValue(FuriganaProperty, value); }
        }

        public string Kanji
        {
            get { return GetValue(KanjiProperty) as string; }
            set { SetValue(KanjiProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            int kanjiFontSize = 32;
            int kanjiLength = !string.IsNullOrWhiteSpace(Kanji) ? Kanji.Length : 1;
            int kanjiWidth = kanjiFontSize * kanjiLength;
            int kanjiWidthPerChar = kanjiFontSize;
            int kanjiMarginPerChar = 0;

            int kanaFontSize = 16;
            int kanaLength = !string.IsNullOrWhiteSpace(Furigana) ? Furigana.Length : 1;
            int kanaWidth = kanaFontSize * kanaLength;
            int kanaWidthPerChar = kanaFontSize;
            int kanaMarginPerChar = 0;

            if (kanaWidth > kanjiWidth)
            {
                kanjiWidthPerChar = kanaWidth / kanjiLength;
                kanjiMarginPerChar = (kanjiWidthPerChar - kanjiFontSize) / 2;
            }
            else
            {
                kanaWidthPerChar = kanjiWidth / kanaLength;
                kanaMarginPerChar = (kanaWidthPerChar - kanaFontSize) / 2;
            }

            RenderText(drawingContext, Furigana, kanaFontSize, kanaWidthPerChar, kanaMarginPerChar, 0);
            RenderText(drawingContext, Kanji, kanjiFontSize, kanjiWidthPerChar, kanjiMarginPerChar, kanaFontSize);
        }

        private void RenderText(DrawingContext drawingContext, string text, int fontSize, int widthPerChar, int marginPerChar, int y)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            int currentPos = 0;
            foreach (var ch in text)
            {
                Typeface typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
                FormattedText formattedText = new FormattedText(ch.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontSize, Foreground, 96);

                drawingContext.DrawText(formattedText, new Point(currentPos + marginPerChar, y));
                currentPos += widthPerChar;
            }
        }
    }
}
