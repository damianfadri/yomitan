using System.Threading.Tasks;
using Yomitan.Core.Contracts;
using Yomitan.Models;

namespace Yomitan.Contracts
{
    public interface ITagColorService : IService
    {
        TagColor GetTagColor(string category);
        TagColor GetDefaultTagColor();
    }
}
