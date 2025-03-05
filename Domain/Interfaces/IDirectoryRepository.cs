using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDirectoryRepository
    {
        Task AddDirectoryAsync(DirectoryMetadata directory);
        Task<DirectoryMetadata?> GetDirectoryAsync(Guid id);
        Task<DirectoryMetadata?> GetDirectoryWithSubstorageAsync(Guid directoryId);
        Task<bool> CheckLoopAsync(DirectoryMetadata child, DirectoryMetadata parent);
        Task DeleteDirectoryAsync(Guid id);
        Task MoveDirectoryAsync(DirectoryMetadata from, DirectoryMetadata to);
        void Update(DirectoryMetadata directory);
    }
}
