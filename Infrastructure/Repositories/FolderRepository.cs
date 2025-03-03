
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        private readonly AppDbContext _context;

        public FolderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddFolderAsync(FolderMetadata folder)
        {
            await _context.Folders.AddAsync(folder);
        }

        public async Task<FolderMetadata?> GetFolderAsync(Guid id)
        {
            return await _context.Folders.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FolderMetadata?> GetFolderWithSubfolders(Guid folderId)
        {
            return await _context.Folders
                .Include(f => f.SubStorage)
                .FirstOrDefaultAsync(f => f.Id == folderId);
        }

        public async Task<bool> CheckLoopAsync(Guid parentId, Guid childId)
        {
            var visited = new HashSet<Guid>();
            var queue = new Queue<Guid>();
            queue.Enqueue(parentId);

            while (queue.Count > 0)
            {
                var currentFolderId = queue.Dequeue();
                if (currentFolderId == childId)
                    return true;

                var subFolderIds = await _context.Folders
                    .Where(f => f.Parent.Id == currentFolderId)
                    .Select(f => f.Id)
                    .ToListAsync();

                foreach (var subFolderId in subFolderIds)
                {
                    if (!visited.Contains(subFolderId))
                    {
                        visited.Add(subFolderId);
                        queue.Enqueue(subFolderId);
                    }
                }
            }
            return false;
        }
    }
}
