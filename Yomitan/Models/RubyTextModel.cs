namespace Yomitan.Models
{
    public class RubyTextModel
    {
        public string Text { get; set; }
        public string Ruby { get; set; }

        public RubyTextModel(string text) : this(text, string.Empty) { }

        public RubyTextModel(string text, string ruby)
        {
            Text = text;
            Ruby = ruby;
        }
    }
}
