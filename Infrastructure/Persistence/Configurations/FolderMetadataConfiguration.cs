using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class FolderMetadataConfiguration : IEntityTypeConfiguration<FolderMetadata>
    {
        public void Configure(EntityTypeBuilder<FolderMetadata> builder)
        {

        }
    }
}
