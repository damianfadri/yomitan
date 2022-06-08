using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Yomitan.Core.Helpers;

namespace Yomitan.Core.Models.Terms
{
    public class Expression
    {
        public string Text { get; set; }
        public string Reading { get; set; }
        public IEnumerable<Kanji> Segments { get; set; }

        public Expression(string kanji, string reading)
        {
            Text = kanji;
            Reading = JapaneseHelper.ConvertKatakanaToHiragana(reading);
            Segments = GetSegments(kanji, reading);
        }

        public bool Matches(string keyword)
        {
            string convertedText = JapaneseHelper.ConvertKatakanaToHiragana(Text);
            string convertedReading = JapaneseHelper.ConvertKatakanaToHiragana(Reading);

            return convertedText.Equals(keyword) || convertedReading.Equals(keyword);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Expression))
                return false;

            Expression other = (Expression)obj;
            return Text.Equals(other.Text)
                    && Reading.Equals(other.Reading);
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode() + Reading.GetHashCode();
        }

        private IEnumerable<Kanji> GetSegments(string expression, string reading)
        {
            if (JapaneseHelper.IsKana(expression))
            {
                yield return new Kanji(expression);
                yield break;
            }

            StringBuilder pattern = new StringBuilder();
            IList<string> expressionSegment = JapaneseHelper.Segment(expression).ToList();

            foreach (string segment in expressionSegment)
            {
                pattern.Append("(");
                if (JapaneseHelper.IsKana(segment))
                    pattern.Append(segment);
                else
                    pattern.Append(".+");
                pattern.Append(")");
            }

            Match matches = Regex.Match(reading, pattern.ToString());
            if (!matches.Success)
                throw new ArgumentException($"Reading does not match generated pattern: {reading} {pattern}");

            if (expressionSegment.Count != matches.Groups.Count - 1)
                throw new ArgumentException($"Segments do not match reading. {expression} {reading}");

            for (int i = 1; i < matches.Groups.Count; i++)
            {
                Group group = matches.Groups[i];

                string kanjiSegment = expressionSegment[i - 1];
                string readingSegment = group.Value;

                if (kanjiSegment.Equals(readingSegment))
                    yield return new Kanji(kanjiSegment);
                else
                    yield return new Kanji(kanjiSegment, readingSegment);
            }
        }
    }
}
