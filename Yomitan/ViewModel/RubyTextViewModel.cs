namespace Yomitan.ViewModel
{
    public class RubyTextViewModel
    {
        public string Text { get; set; }
        public string Ruby { get; set; }

        public RubyTextViewModel(string text) : this(text, string.Empty) { }

        public RubyTextViewModel(string text, string ruby)
        {
            Text = text;
            Ruby = ruby;
        }
    }
}
