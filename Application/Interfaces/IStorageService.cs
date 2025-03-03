namespace Application.Interfaces
{
    public interface IStorageService
    {
        Task UploadFileAsync(Stream fileStream, string destinationPath, Guid userId);
        Task<Stream> DownloadFileAsync(string remoteFilePath);
        Task DeleteFileAsync(string filePath);
        Task<IEnumerable<string>> ListFilesAsync(string directory);
    }
}
