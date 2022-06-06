using System;
using System.Collections.Generic;

namespace Yomitan.Shared.Repository
{
    public interface IRepository<T>
    {
        void Load(RepositoryPath source);

        void Unload(RepositoryPath source);

        IEnumerable<T> FindAll();

        IEnumerable<T> FindBy(string keyword);
    }
}
