using Yomitan.Core.Models;

namespace Yomitan.Models
{
    public class TermBankModel
    {
        public string Title { get; set; }
        public string Revision { get; set; }
        public bool Enabled { get; set; }

        public TermBankModel(string title, string revision, bool enabled)
        {
            Title = title;
            Revision = revision;
            Enabled = enabled;
        }
    }
}
