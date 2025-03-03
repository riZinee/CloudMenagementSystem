using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IFolderRepository
    {
        Task AddFolderAsync(FolderMetadata folder);
        Task<FolderMetadata?> GetFolderAsync(Guid id);
        Task<FolderMetadata?> GetFolderWithSubfolders(Guid folderId);
        Task<bool> CheckLoopAsync(Guid parentId, Guid childId);
    }
}
