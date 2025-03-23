using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class LocalFileService : IFileService
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
        public async Task<Guid> UploadFileChunkAsync(Guid uploadId, Stream chunkStream, string destinationPath, int chunkIndex, int totalChunks, Guid userId)
        {
            string tempDir = Path.Combine(_basePath, uploadId.ToString());
            Directory.CreateDirectory(tempDir);

            string chunkPath = Path.Combine(tempDir, $"{chunkIndex}.part");

            using (var fileStream = new FileStream(chunkPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                await chunkStream.CopyToAsync(fileStream);
            }

            var uploadedChunks = Directory.GetFiles(tempDir).Length;
            int progress = (uploadedChunks * 100) / totalChunks;

            await _uploadProgressNotifier.NotifyProgressAsync(userId.ToString(), destinationPath, progress);

            if (uploadedChunks == totalChunks)
            {
                var guid = await MergeChunks(destinationPath, tempDir);
                await _uploadProgressNotifier.NotifyProgressAsync(userId.ToString(), destinationPath, 100);
                return guid;
            }
            return Guid.Empty;
        }


        private async Task<Guid> MergeChunks(string destinationPath, string tempDir)
        {
            var guid = Guid.NewGuid();
            string fullPath = Path.Combine(_basePath, destinationPath);

            string extension = Path.GetExtension(fullPath);

            string directory = Path.GetDirectoryName(fullPath);

            string newFileName = guid.ToString() + extension;

            string newFullPath = Path.Combine(directory, newFileName);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var files = Directory.GetFiles(tempDir)
                                 .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)))
                                 .ToList();

            using (var outputStream = new FileStream(newFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                foreach (var file in files)
                {
                    using (var inputStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        await inputStream.CopyToAsync(outputStream);
                    }
                    File.Delete(file);
                }
            }

            Directory.Delete(tempDir);

            return guid;
        }


        public async Task<Stream> DownloadFileAsync(string remoteFilePath, string contentType)
        {
            string fullPath = Path.Combine(_basePath, $"{remoteFilePath}.{contentType}");

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Plik {remoteFilePath} nie istnieje.");

            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
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

        public async Task MoveFileAsync(string from, string to, string name)
        {
            string fullFromPath = Path.Combine(_basePath, from);
            string fullToPath = Path.Combine(_basePath, to, name);

            if (!File.Exists(fullFromPath))
                throw new FileNotFoundException($"Plik '{fullFromPath}' nie istnieje.");

            // Upewnij się, że katalog docelowy istnieje
            string destinationDirectory = Path.GetDirectoryName(fullToPath);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            await Task.Run(() => File.Move(fullFromPath, fullToPath));
        }
    }
}
