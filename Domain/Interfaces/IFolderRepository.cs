using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDirectoryRepository
    {
        Task AddDirectoryAsync(DirectoryMetadata directory);
        Task<DirectoryMetadata?> GetDirectoryAsync(Guid id);
        Task<DirectoryMetadata?> GetDirectoryWithSubdirectories(Guid directoryId);
        Task<bool> CheckLoopAsync(Guid parentId, Guid childId);
    }
}
