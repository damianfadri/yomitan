using System.Collections.Generic;
using System.Linq;
using Yomitan.Shared.Repository;

namespace Yomitan.Shared.Term
{
    public class TermRepositoryCollection : IRepository<Term>
    {
        private IDictionary<string, IRepository<Term>> _repositories;

        public TermRepositoryCollection()
        {
            _repositories = new Dictionary<string, IRepository<Term>>();
        }

        public IEnumerable<Term> FindAll()
        {
            return _repositories.Values.SelectMany(repo => repo.FindAll());
        }

        public IEnumerable<Term> FindBy(string keyword)
        {
            return _repositories.Values.SelectMany(repo => repo.FindBy(keyword));
        }

        public void Load(RepositoryPath source)
        {
            TermRepository termRepository = new TermRepository();
            termRepository.Load(source);

            _repositories[source.Name] = termRepository;
        }
    }
}
