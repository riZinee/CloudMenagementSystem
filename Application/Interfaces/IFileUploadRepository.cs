using Application.Entities;

namespace Application.Interfaces
{
    public interface IFileUploadRepository
    {
        Task<FileUpload?> GetByIdAsync(Guid id);
        Task AddAsync(FileUpload fileUpload);
        void Update(FileUpload fileUpload);
    }
}
