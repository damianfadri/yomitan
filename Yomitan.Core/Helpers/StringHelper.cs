using System.Collections.Generic;
using System.Linq;

namespace Yomitan.Core.Helpers
{
    public static class StringHelper
    {
        public static IEnumerable<string> Tokenize(this string text, char delimiter = ' ')
        {
            return text.Split(delimiter).Where(t => !string.IsNullOrWhiteSpace(t));
        }
    }
}
