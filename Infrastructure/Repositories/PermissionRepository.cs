using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _context;

        public PermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
        }

        public async Task<List<Permission>> GetByStorageAsync(Guid storageId)
        {
            return await _context.Permissions
                .Where(p => p.StorageId == storageId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Permission>> GetByUserAsync(Guid userId)
        {
            return await _context.Permissions
                .Where(p => p.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Permission?> GetByUserAndStorageAsync(Guid userId, Guid storageId)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.UserId == userId && p.StorageId == storageId);
        }

        public void Update(Permission permission)
        {
            _context.Permissions.Update(permission);
        }

        public async Task RemovePermissionsAsync(Guid id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
            }
        }

        public async Task AddPermissionsToSubStoragesAsync(Permission permission)
        {
            var permissionValuesJson = JsonSerializer.Serialize(permission.Values);

            var sql = "EXEC AddPermissionsToSubStorages @UserId, @StorageId, @PermissionValues";

            await _context.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@UserId", permission.UserId),
                new SqlParameter("@StorageId", permission.StorageId),
                new SqlParameter("@PermissionValues", permissionValuesJson));
        }

        public async Task UpdatePermissionsForSubStoragesAsync(Permission permission)
        {
            var permissionValuesJson = JsonSerializer.Serialize(permission.Values);

            var sql = "EXEC UpdatePermissionsForSubStorages @UserId, @StorageId, @PermissionValues";

            await _context.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@UserId", permission.UserId),
                new SqlParameter("@StorageId", permission.StorageId),
                new SqlParameter("@PermissionValues", permissionValuesJson));
        }

    }
}
