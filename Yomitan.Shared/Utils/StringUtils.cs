using System.Collections.Generic;
using System.Linq;

namespace Yomitan.Shared.Utils
{
    public static class StringUtils
    {
        public static IEnumerable<string> Tokenize(this string text, char delimiter = ' ')
        {
            return text.Split(delimiter).Where(t => !string.IsNullOrWhiteSpace(t));
        }
    }
}
