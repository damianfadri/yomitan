using System.Collections.Generic;
using System.Threading.Tasks;
using Yomitan.Core.Models;

namespace Yomitan.Core.Contracts
{
    public interface ITermBankService : IService
    {
        Task<TermBank> LoadOneAsync();

        Task<TermBank> LoadOneAsync(string filepath);

        Task<IEnumerable<TermBank>> GetAllAsync();
    }
}
