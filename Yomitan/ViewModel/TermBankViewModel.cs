using Yomitan.Core.Models;

namespace Yomitan.ViewModel
{
    public class TermBankViewModel
    {
        public string Title { get; set; }
        public string Revision { get; set; }
        public bool Enabled { 
            get; 
            set; 
        }

        public TermBankViewModel(TermBank metadata)
        {
            Title = metadata.Title;
            Revision = metadata.Revision;

            // TODO: Get from last saved setting.
            Enabled = true;
        }
    }
}
