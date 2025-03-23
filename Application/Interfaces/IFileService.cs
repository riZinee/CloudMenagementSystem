namespace Application.Interfaces
{
    public interface IFileService
    {
        Task UploadFileAsync(Stream fileStream, string destinationPath, Guid userId);
        Task<Stream> DownloadFileAsync(string remoteFilePath, string contentType);
        Task DeleteFileAsync(string filePath);
        Task<Guid> UploadFileChunkAsync(Guid uploadId, Stream chunkStream, string destinationPath, int chunkIndex, int totalChunks, Guid userId);
        Task MoveFileAsync(string from, string to, string name);
    }
}
