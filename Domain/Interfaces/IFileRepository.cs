using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IFileRepository
    {
        Task AddAsync(FileMetadata fileMetadata);
        Task<FileMetadata?> GetAsync(Guid id);
        void Update(FileMetadata fileMetadata);
        Task DeleteAsync(FileMetadata fileMetadata);
        void Move(FileMetadata from, DirectoryMetadata to);
    }
}
