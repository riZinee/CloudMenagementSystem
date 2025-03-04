
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
            return await _context.Directories.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<DirectoryMetadata?> GetDirectoryWithSubdirectories(Guid directoryId)
        {
            return await _context.Directories
                .Include(f => f.SubStorage)
                .FirstOrDefaultAsync(f => f.Id == directoryId);
        }

        public async Task<bool> CheckLoopAsync(Guid parentId, Guid childId)
        {
            var visited = new HashSet<Guid>();
            var queue = new Queue<Guid>();
            queue.Enqueue(parentId);

            while (queue.Count > 0)
            {
                var currentDirectoryId = queue.Dequeue();
                if (currentDirectoryId == childId)
                    return true;

                var subDirectoryIds = await _context.Directories
                    .Where(f => f.Parent.Id == currentDirectoryId)
                    .Select(f => f.Id)
                    .ToListAsync();

                foreach (var subDirectoryId in subDirectoryIds)
                {
                    if (!visited.Contains(subDirectoryId))
                    {
                        visited.Add(subDirectoryId);
                        queue.Enqueue(subDirectoryId);
                    }
                }
            }
            return false;
        }
    }
}
