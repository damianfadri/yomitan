using System.Collections.Generic;
using System.Drawing;
using Yomitan.Shared.OCR;
using Yomitan.Shared.Term;
using Yomitan.View;

namespace Yomitan.Service
{
    public class TermDisplayService
    {
        private TermDisplayWindow _window;

        public void Display(IEnumerable<Term> terms, TextRegion region)
        {
            if (_window == null)
                _window = new TermDisplayWindow();


            _window.ViewModel.LoadTerms(terms);
            if (_window.ViewModel.Terms.Count == 0)
            {
                _window.Hide();
                return;
            }

            Rectangle location = CalculateScreenLocation(region);
            _window.Left = location.Left;
            _window.Top = location.Top;

            _window.Show();
            _window.Focus();
        }

        private Rectangle CalculateScreenLocation(TextRegion region)
        {
            int windowTop = region.Bounds.Top;
            int windowLeft = region.Bounds.Left - (int)_window.Width;
            int windowRight = region.Bounds.Left;
            int windowBottom = region.Bounds.Top + (int)_window.Height;

            // Put window to right side of text instead.
            if (windowLeft < region.Screen.Left)
                windowLeft = region.Bounds.Right;

            if (windowBottom > region.Screen.Bottom)
                windowTop = region.Screen.Bottom - (int)_window.Height;

            return new Rectangle(windowLeft, windowTop, (int)_window.Width, (int)_window.Height);
        }
    }
}
