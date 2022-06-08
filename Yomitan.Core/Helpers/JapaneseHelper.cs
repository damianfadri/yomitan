using System.Collections.Generic;
using System.Text;

namespace Yomitan.Core.Helpers
{
    public static class JapaneseHelper
    {
        private static readonly int[] HIRAGANA_CONVERSION_RANGE = new int[] { 0x3041, 0x3096 };
        private static readonly int[] KATAKANA_CONVERSION_RANGE = new int[] { 0x30a1, 0x30f6 };

        public static string ConvertKatakanaToHiragana(string text)
        {
            StringBuilder result = new StringBuilder();

            int offset = HIRAGANA_CONVERSION_RANGE[0] - KATAKANA_CONVERSION_RANGE[0];
            foreach (char letter in text)
            {
                int codePoint = (int)letter;
                if (IsCodePointInRange(codePoint, KATAKANA_CONVERSION_RANGE[0], KATAKANA_CONVERSION_RANGE[1]))
                    result.Append((char)(codePoint + offset));
                else
                    result.Append(letter);
            }

            return result.ToString();
        }

        public static bool IsKana(string expression)
        {
            foreach (char letter in expression)
                if (!IsKana(letter))
                    return false;

            return true;
        }

        public static bool IsKana(char character)
        {
            int codePoint = (int)character;
            return IsCodePointInRange(codePoint, HIRAGANA_CONVERSION_RANGE[0], HIRAGANA_CONVERSION_RANGE[1])
                    || IsCodePointInRange(codePoint, KATAKANA_CONVERSION_RANGE[0], KATAKANA_CONVERSION_RANGE[1]);
        }

        public static IEnumerable<string> Segment(string expression)
        {
            bool? isPrevKana = null;
            StringBuilder currentSegment = new StringBuilder();

            foreach (char exprChar in expression)
            {
                bool isKana = IsKana(exprChar);
                if (isPrevKana != null && !isKana.Equals(isPrevKana))
                {
                    yield return currentSegment.ToString();
                    currentSegment = new StringBuilder();
                }

                currentSegment.Append(exprChar);
                isPrevKana = isKana;
            }

            yield return currentSegment.ToString();
        }

        private static bool IsCodePointInRange(int codePoint, int min, int max)
        {
            return (codePoint >= min && codePoint <= max);
        }
    }
}
