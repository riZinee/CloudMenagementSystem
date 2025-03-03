using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class LocalFileService : IStorageService
    {
        private readonly string _basePath;
        private readonly IUploadProgressNotifier _uploadProgressNotifier;
        private readonly int _grainSize = 8192;

        public LocalFileService(IConfiguration configuration, IUploadProgressNotifier uploadProgressNotifier)
        {
            _uploadProgressNotifier = uploadProgressNotifier;
            _basePath = configuration["LocalStorage:BasePath"] ?? "uploads";
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task UploadFileAsync(Stream fileStream, string destinationPath, Guid userId)
        {
            string fullPath = Path.Combine(_basePath, destinationPath);
            Console.WriteLine("przesyłanie");

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            if (fileStream == null)
            {
                throw new ApplicationNullException();
            }

            try
            {
                fileStream.Position = 0;

                long totalBytes = fileStream.Length;
                long uploadedBytes = 0;
                byte[] buffer = new byte[_grainSize];

                var fileName = destinationPath.Split("/")[destinationPath.Split().Length];

                var lastProgress = 0;

                using (var uploadStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, _grainSize, true))
                {
                    int bytesRead;
                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await uploadStream.WriteAsync(buffer, 0, bytesRead);
                        uploadedBytes += bytesRead;

                        int progress = (int)((uploadedBytes * 100) / totalBytes);

                        Console.WriteLine(progress);
                        //Thread.Sleep(100);

                        if (progress != lastProgress)
                        {
                            await _uploadProgressNotifier.NotifyProgressAsync(userId.ToString(), fileName, progress);

                        }
                    }
                    uploadStream.Position = 0;

                    await _uploadProgressNotifier.NotifyProgressAsync(userId.ToString(), fileName, 100);
                }
            }
            finally
            {
                fileStream.Dispose();
            }
        }

        public async Task<Stream> DownloadFileAsync(string remoteFilePath)
        {
            string fullPath = Path.Combine(_basePath, remoteFilePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Plik {remoteFilePath} nie istnieje.");

            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            string fullPath = Path.Combine(_basePath, filePath);

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
            }
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string directory)
        {
            string fullPath = Path.Combine(_basePath, directory);

            if (!Directory.Exists(fullPath))
                return new List<string>();

            var files = await Task.Run(() => Directory.GetFiles(fullPath));
            return files.Select(Path.GetFileName);
        }
    }
}
