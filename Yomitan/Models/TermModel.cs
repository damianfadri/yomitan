using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models;

namespace Yomitan.Models
{
    public class TermModel
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<RubyTextModel> Expression { get; set; }
        public IEnumerable<TagModel> Tags { get; set; }
        public IEnumerable<DefinitionModel> Definitions { get; set; }

        public TermModel(Term term)
        {
            Expression = GetSegments(term.Text, term.Reading);
            Tags = GetTags(term.Tags);
            Definitions = GetDefinitions(term.DefinitionEntries);
        }

        private IEnumerable<DefinitionModel> GetDefinitions(IEnumerable<DefinitionEntry> definitionEntries)
        {
            return definitionEntries.Select(de => new DefinitionModel(de));
        }

        private IEnumerable<TagModel> GetTags(IEnumerable<Tag> tags)
        {
            return tags.Select(t => {

                return new TagModel(t.Name, t.Category);
            });
        }

        private IEnumerable<RubyTextModel> GetSegments(string expression, string reading)
        {
            var pattern = new StringBuilder();
            var expressionSegment = NihongoHelper.Segment(expression).ToList();

            foreach (string segment in expressionSegment)
            {
                pattern.Append("(");
                if (NihongoHelper.IsKana(segment))
                    pattern.Append(segment);
                else
                    pattern.Append(".+");
                pattern.Append(")");
            }

            var matches = Regex.Match(reading, pattern.ToString());
            if (!matches.Success)
            {
                Logger.Debug($"Reading does not match generated pattern: {reading} {pattern}");
                yield break;
            }

            if (expressionSegment.Count != matches.Groups.Count - 1)
            {
                Logger.Debug($"Segments do not match reading. {expression} {reading}");
                yield break;
            }

            for (int i = 1; i < matches.Groups.Count; i++)
            {
                var group = matches.Groups[i];

                string kanjiSegment = expressionSegment[i - 1];
                string readingSegment = group.Value;

                if (kanjiSegment.Equals(readingSegment))
                    yield return new RubyTextModel(kanjiSegment);
                else
                    yield return new RubyTextModel(kanjiSegment, readingSegment);
            }
        }
    }
}
