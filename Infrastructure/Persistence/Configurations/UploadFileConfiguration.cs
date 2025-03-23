using Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    class UploadFileConfiguration : IEntityTypeConfiguration<FileUpload>
    {
        public void Configure(EntityTypeBuilder<FileUpload> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FileName);
            builder.Property(u => u.DirectoryId);
            builder.Property(u => u.TotalChunks);
            builder.Property(u => u.UploadedChunks);
            builder.Property(u => u.UserId);
            builder.Property(u => u.StartTime);
            builder.Property(u => u.EndTime);
            builder.Property(u => u.IsCompleted);
        }


    }
}