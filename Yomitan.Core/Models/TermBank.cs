using System.Collections.Generic;

namespace Yomitan.Core.Models
{
    public class TermBank
    {
        public string Title { get; set; }
        public int Format { get; set; }
        public string Revision { get; set; }
        public bool Sequenced { get; set; }
        public IEnumerable<Term> Terms { get; set; }

        public TermBank() : this(string.Empty, string.Empty, null) { }

        public TermBank(string title, string revision, IEnumerable<Term> terms)
        {
            Title = title;
            Revision = revision;
            Terms = terms;
        }
    }
}
