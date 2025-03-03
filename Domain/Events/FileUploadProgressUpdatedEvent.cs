using MediatR;

namespace Domain.Events
{
    public class FileUploadProgressUpdatedEvent : INotification
    {
        public Guid UploadId { get; }
        public int Progress { get; }
        public string UserId { get; }

        public FileUploadProgressUpdatedEvent(Guid uploadId, int progress, string userId)
        {
            UploadId = uploadId;
            Progress = progress;
            UserId = userId;
        }
    }
}
