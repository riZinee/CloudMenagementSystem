namespace Application.Interfaces
{
    public interface IFileService
    {
        Task UploadFileAsync(Stream fileStream, string destinationPath, Guid userId);
        Task<Stream> DownloadFileAsync(string remoteFilePath);
        Task DeleteFileAsync(string filePath);
    }
}
