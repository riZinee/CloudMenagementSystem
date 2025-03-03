using Application.Entities;

namespace Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetByUserIdAsync(Guid id);
        Task AddAsync(RefreshToken refreshToken);
    }
}
