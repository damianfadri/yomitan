using System.Collections.Generic;
using System.Drawing;
using Yomitan.Shared.Term;

namespace Yomitan.Message
{
    public class SearchResultsMessage
    {
        public IEnumerable<Term> SearchResults { get; }
        public Rectangle Bounds { get; }
        public Rectangle Screen { get; }

        public SearchResultsMessage(IEnumerable<Term> searchResults, Rectangle bounds, Rectangle screen)
        {
            SearchResults = searchResults;
            Bounds = bounds;
            Screen = screen;
        }
    }
}
