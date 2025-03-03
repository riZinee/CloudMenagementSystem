using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value).HasColumnName("Email").IsRequired();
            });
            builder.OwnsOne(u => u.UserStorage, storage =>
            {
                storage.Property(s => s.UsedSpace).HasColumnName("UsedSpace").IsRequired();
                storage.Property(s => s.TotalSpace).HasColumnName("TotalSpace").IsRequired();
            });

            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Salt).IsRequired();
        }
    }
}
