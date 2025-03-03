using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<User?> GetByEmailAsync(Email email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Value == email.Value);
        }

        public async Task<string?> GetPasswordHashByEmailAsync(Email email)
        {
            var user = await _context.Users.Where(u => u.Email.Value == email.Value).FirstOrDefaultAsync();
            return user.PasswordHash;
        }

        public async Task<string?> GetSaltByEmailAsync(Email email)
        {
            var user = await _context.Users.Where(u => u.Email.Value == email.Value).FirstOrDefaultAsync();
            return user.Salt;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<string?> GetPasswordHashByIdAsync(Guid id)
        {
            var user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            return user.PasswordHash;
        }

        public async Task<string?> GetSaltByIdAsync(Guid id)
        {
            var user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            return user.Salt;
        }

    }
}
