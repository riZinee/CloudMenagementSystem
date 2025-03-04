using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;

namespace Infrastructure.Services
{
    public class SftpService : IFileService
    {
        private readonly SftpClient _client;

        public SftpService(IConfiguration configuration)
        {
            _client = new SftpClient(
                configuration["Sftp:Host"],
                int.Parse(configuration["Sftp:Port"]),
                configuration["Sftp:User"],
                configuration["Sftp:Password"]);
        }

        public async Task UploadFileAsync(Stream fileStream, string destinationPath)
        {
            _client.Connect();
            await Task.Run(() => _client.UploadFile(fileStream, destinationPath));
            _client.Disconnect();
        }

        public async Task<Stream> DownloadFileAsync(string remoteFilePath)
        {
            _client.Connect();
            var stream = new MemoryStream();
            await Task.Run(() => _client.DownloadFile(remoteFilePath, stream));
            _client.Disconnect();
            stream.Position = 0;
            return stream;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            _client.Connect();
            await Task.Run(() => _client.DeleteFile(filePath));
            _client.Disconnect();
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string directory)
        {
            _client.Connect();
            var files = await Task.Run(() => _client.ListDirectory(directory));
            _client.Disconnect();

            // Zwracamy tylko nazwy plików zamiast całych obiektów SftpFile
            List<string> fileNames = new List<string>();
            foreach (var file in files)
            {
                if (!file.Name.StartsWith(".")) // Pomijamy ukryte pliki i katalogi (np. ".", "..")
                {
                    fileNames.Add(file.Name);
                }
            }

            return fileNames;
        }

        public Task UploadFileAsync(Stream fileStream, string destinationPath, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
