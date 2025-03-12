using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task AddAsync(Permission permission);
        Task<List<Permission>> GetByUserAsync(Guid userId);
        Task<List<Permission>> GetByStorageAsync(Guid storageId);
        Task<Permission> GetByUserAndStorageAsync(Guid userId, Guid storageId);
        Task RemovePermissionsAsync(Guid Id);
        void Update(Permission permission);
        Task AddPermissionsToSubStoragesAsync(Permission permission);
        Task UpdatePermissionsForSubStoragesAsync(Permission permission);
    }
}
