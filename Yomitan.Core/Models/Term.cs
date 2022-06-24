using System;
using System.Collections.Generic;
using System.Linq;
using Yomitan.Core.Helpers;

namespace Yomitan.Core.Models
{
    public class Term
    {
        public string Text { get; set; }
        public string Reading { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<DefinitionEntry> DefinitionEntries { get; set; }
        public int SequenceNumber { get; set; }
        public int PopularityScore { get; set; }

        public void Merge(Term term)
        {
            if (!Equals(term))
                throw new InvalidOperationException($"Cannot merge terms. {Text} <- {term.Text}");

            DefinitionEntries = DefinitionEntries.Concat(term.DefinitionEntries);
        }

        public bool Matches(string keyword)
        {
            string convertedText = NihongoHelper.ConvertKatakanaToHiragana(Text);
            string convertedReading = NihongoHelper.ConvertKatakanaToHiragana(Reading);

            return convertedText.Equals(keyword) || convertedReading.Equals(keyword);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Term))
                return false;

            Term other = (Term)obj;
            return Text.Equals(other.Text) && Reading.Equals(other.Reading);
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode() * Reading.GetHashCode();
        }
    }
}
