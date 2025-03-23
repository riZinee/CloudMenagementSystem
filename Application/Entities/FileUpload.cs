namespace Application.Entities
{
    public class FileUpload
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public Guid DirectoryId { get; set; }
        public int TotalChunks { get; set; }
        public int UploadedChunks { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; } = null;
        public bool IsCompleted { get; set; }

        public FileUpload()
        {
        }

        public FileUpload(Guid id, string fileName, Guid directoryId, int totalChunks, int uploadedChunks, Guid userId, DateTime startTime, DateTime? endTime, bool isCompleted)
        {
            Id = id;
            FileName = fileName;
            DirectoryId = directoryId;
            TotalChunks = totalChunks;
            UploadedChunks = uploadedChunks;
            UserId = userId;
            StartTime = startTime;
            EndTime = endTime;
            IsCompleted = isCompleted;
        }
    }
}
