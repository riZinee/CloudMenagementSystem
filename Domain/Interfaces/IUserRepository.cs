using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        void Update(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(Email email);
        Task<string?> GetPasswordHashByEmailAsync(Email email);
        Task<string?> GetSaltByEmailAsync(Email email);
    }
}
