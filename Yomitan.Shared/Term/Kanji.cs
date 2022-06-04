using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomitan.Shared.Term
{
    public class Kanji
    {
        public string Text { get; set; }
        public string Reading { get; set; }

        public Kanji(string text) : this(text, string.Empty) { }

        public Kanji(string text, string reading)
        {
            Text = text;
            Reading = reading;
        }
    }
}
