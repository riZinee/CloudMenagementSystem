using Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.UserId);
            builder.Property(u => u.ExpiresAt);
            builder.Property(u => u.IsRevoked);
            builder.Property(u => u.Token);
        }
    }
}
