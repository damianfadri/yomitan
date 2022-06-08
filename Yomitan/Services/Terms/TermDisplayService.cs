using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Yomitan.Core.Models.Terms;
using Yomitan.Core.Services;
using Yomitan.Views;

namespace Yomitan.Service
{
    public class TermDisplayService : ITermDisplayService
    {
        private SearchResultsWindow _window;

        public void Display(IEnumerable<Term> terms, Rectangle bounds)
        {
            if (_window == null)
                _window = new SearchResultsWindow();

            _window.ViewModel.LoadTerms(terms);
            if (_window.ViewModel.Terms.Count == 0)
            {
                _window.Hide();
                return;
            }

            Rectangle location = CalculateScreenLocation(bounds);
            _window.Left = location.Left;
            _window.Top = location.Top;

            _window.Show();
            _window.Focus();
        }

        private Rectangle CalculateScreenLocation(Rectangle bounds)
        {
            Rectangle screen = SystemInformation.VirtualScreen;

            int windowTop = bounds.Top;
            int windowLeft = bounds.Left - (int)_window.Width;
            int windowRight = bounds.Left;
            int windowBottom = bounds.Top + (int)_window.Height;

            // Put window to right side of text instead.
            if (windowLeft < screen.Left)
                windowLeft = bounds.Right;

            if (windowBottom > screen.Bottom)
                windowTop = screen.Bottom - (int)_window.Height;

            return new Rectangle(windowLeft, windowTop, (int)_window.Width, (int)_window.Height);
        }
    }
}
