using System.Collections.Generic;
using System.Threading.Tasks;
using Yomitan.Core.Models;

namespace Yomitan.Core.Contracts
{
    public interface IRuleBankService : IService
    {
        Task<IEnumerable<Rule>> GetAllAsync();
    }
}
