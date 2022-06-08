namespace Yomitan.Core.Services
{
    public interface ITermBankService
    {
        string Import();

        void Purge(string name);
    }
}
