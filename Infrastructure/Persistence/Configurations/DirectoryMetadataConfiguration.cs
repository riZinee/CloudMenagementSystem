using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class DirectoryMetadataConfiguration : IEntityTypeConfiguration<DirectoryMetadata>
    {
        public void Configure(EntityTypeBuilder<DirectoryMetadata> builder)
        {

        }
    }
}
