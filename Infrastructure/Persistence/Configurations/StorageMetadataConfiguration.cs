using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class StorageMetadataConfiguration : IEntityTypeConfiguration<StorageMetadata>
    {
        public void Configure(EntityTypeBuilder<StorageMetadata> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.OwnerId)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasDiscriminator<string>("StorageType")
                .HasValue<DirectoryMetadata>("Directory")
                .HasValue<FileMetadata>("File");

            builder.HasOne(f => f.Parent)
            .WithMany(f => f.SubStorage)
            .HasForeignKey("ParentDirectoryId")
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
