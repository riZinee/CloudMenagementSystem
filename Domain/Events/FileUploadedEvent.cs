using Domain.Common;

namespace Domain.Events
{
    public record FileUploadedEvent : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
        public Guid FileId { get; set; }
        public Guid UserId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }

        public FileUploadedEvent(Guid fileId, Guid userId, string fileName, long fileSize)
        {
            FileId = fileId;
            UserId = userId;
            FileName = fileName;
            FileSize = fileSize;
        }
    }
}
