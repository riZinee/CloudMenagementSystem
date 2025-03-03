using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
    {
        public void Configure(EntityTypeBuilder<FileMetadata> builder)
        {
            builder.Property(f => f.Size)
                .IsRequired();

            builder.Property(f => f.ContentType)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
