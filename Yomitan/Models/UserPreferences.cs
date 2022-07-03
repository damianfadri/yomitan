using System.Collections.Generic;
using Yomitan.ViewModel;

namespace Yomitan.Models
{
    public class UserPreferences
    {
        public IDictionary<string, TermBankModel> TermBanks { get; set; }
    }
}
