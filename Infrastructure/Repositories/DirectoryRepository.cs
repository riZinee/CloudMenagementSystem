
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DirectoryRepository : IDirectoryRepository
    {
        private readonly AppDbContext _context;

        public DirectoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddDirectoryAsync(DirectoryMetadata directory)
        {
            await _context.Directories.AddAsync(directory);
        }

        public async Task<DirectoryMetadata?> GetDirectoryAsync(Guid id)
        {
            return await _context.Directories.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DirectoryMetadata?> GetDirectoryWithSubstorageAsync(Guid directoryId)
        {
            return await _context.Directories
                .Include(d => d.SubStorage)
                .FirstOrDefaultAsync(d => d.Id == directoryId);
        }

        public async Task<bool> CheckLoopAsync(DirectoryMetadata child, DirectoryMetadata parent)
        {
            var parentPath = parent.Path;

            var childPath = child.Path;

            if (string.IsNullOrEmpty(parentPath) || string.IsNullOrEmpty(childPath))
                return false;

            return parentPath.StartsWith(childPath);
        }

        public async Task DeleteDirectoryAsync(Guid id)
        {
            var directory = await GetDirectoryAsync(id);
            if (directory == null)
                throw new DirectoryNotFoundException();

            string directoryPath = directory.Path;

            await _context.Database.ExecuteSqlRawAsync(@"
                DELETE FROM StorageMetadata 
                WHERE Path LIKE @p0 + '%'",
                directoryPath);
        }

        public async Task MoveDirectoryAsync(DirectoryMetadata from, DirectoryMetadata to)
        {

            if (await CheckLoopAsync(from, to))
            {
                throw new InvalidOperationException();
            }

            string oldPath = from.Path;
            string newPath = Path.Combine(to.Path, from.Id.ToString());

            from.Parent = to;
            from.Path = newPath;

            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE StorageMetadata
                SET Path = REPLACE(Path, @p0, @p1)
                WHERE Path LIKE @p0 + '%'",
                oldPath, newPath);
        }

        public void Update(DirectoryMetadata directory)
        {
            _context.Update(directory);
        }
    }
}
