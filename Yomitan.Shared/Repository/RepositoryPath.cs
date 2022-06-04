using IO = System.IO;

namespace Yomitan.Shared.Repository
{
    public class RepositoryPath
    {
        public string Path { get; set; }
        public string Name { get; set; }

        public RepositoryPath(string path, string name)
        {
            Path = path;
            Name = name;
        }

        public RepositoryPath(string path) : this(path, IO.Path.GetFileNameWithoutExtension(path)) { }
    }
}
