using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class FileRepository : IFileRepository
{
    private readonly AppDbContext _context;

    public FileRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(FileMetadata fileMetadata)
    {
        if (fileMetadata == null)
            throw new ArgumentNullException(nameof(fileMetadata));

        await _context.Files.AddAsync(fileMetadata);
    }

    public async Task DeleteAsync(FileMetadata fileMetadata)
    {
        if (fileMetadata == null)
            throw new ArgumentNullException(nameof(fileMetadata));

        _context.Files.Remove(fileMetadata);
    }

    public async Task<FileMetadata?> GetAsync(Guid id)
    {
        return await _context.Files.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<FileMetadata>> GetAllAsync()
    {
        return await _context.Files.ToListAsync();
    }

    public void Update(FileMetadata fileMetadata)
    {
        if (fileMetadata == null)
            throw new ArgumentNullException(nameof(fileMetadata));

        _context.Files.Update(fileMetadata);
    }

    public void MoveAsync(FileMetadata from, DirectoryMetadata to)
    {
        string oldPath = from.Path;
        string newPath = Path.Combine(to.Path, from.Id.ToString());

        from.Parent = to;
        from.Path = newPath;

        _context.Update(from);
    }
}
