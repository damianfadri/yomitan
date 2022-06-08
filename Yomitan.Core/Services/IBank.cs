using System.Collections.Generic;

namespace Yomitan.Core.Services
{
    public interface IBank<T>
    {
        void Load(string filepath);

        IEnumerable<T> FindAll();

        IEnumerable<T> FindBy(string keyword);
    }
}
