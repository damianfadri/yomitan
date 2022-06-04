using System;
using System.Collections.Generic;
using System.Linq;

namespace Yomitan.Shared.Term
{
    public class Term
    {
        public Expression Expression { get; set; }
        public IEnumerable<Definition> Definitions { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<string> RuleIdentifiers { get; set; }
        public int SequenceNum { get; set; }
        public int PopularityScore { get; set; }

        public void Merge(Term term)
        {
            if (!Equals(term))
                throw new InvalidOperationException($"Cannot merge terms. {Expression} {term.Expression}");

            Definitions = Definitions.Concat(term.Definitions);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Term))
                return false;

            Term other = (Term)obj;
            return Expression.Equals(other.Expression);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }
    }
}
