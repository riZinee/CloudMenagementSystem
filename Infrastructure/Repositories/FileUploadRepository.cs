using Application.Entities;
using Application.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class FileUploadRepository : IFileUploadRepository
    {
        private readonly AppDbContext _context;

        public FileUploadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FileUpload fileUpload)
        {
            await _context.FileUploads.AddAsync(fileUpload);
        }

        public async Task<FileUpload?> GetByIdAsync(Guid id)
        {
            return await _context.FileUploads.FindAsync(id);
        }

        public void Update(FileUpload fileUpload)
        {
            _context.FileUploads.Update(fileUpload);
        }
    }
}
