using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PermissionConfiguration
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Storage)
                .WithMany(s => s.Permissions)
                .HasForeignKey(p => p.StorageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Values)
                .IsRequired();
        }
    }
}
