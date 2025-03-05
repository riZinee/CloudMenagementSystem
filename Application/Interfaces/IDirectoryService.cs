namespace Application.Interfaces
{
    public interface IDirectoryService
    {
        Task CreateAsync(string path);
        Task MoveAsync(string from, string to, string name);
        Task DeleteAsync(string path);
        Task<IEnumerable<string>> ListAsync(string directory);
    }
}
