using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class LocalDirectoryService : IDirectoryService
    {
        private readonly string _basePath;

        public LocalDirectoryService(IConfiguration configuration)
        {
            _basePath = configuration["LocalStorage:BasePath"] ?? "uploads";
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task CreateAsync(string name)
        {
            string fullPath = Path.Combine(_basePath, name);
            await Task.Run(() => Directory.CreateDirectory(fullPath));
        }

        public async Task DeleteAsync(string path)
        {
            string fullPath = Path.Combine(_basePath, path);
            if (Directory.Exists(fullPath))
            {
                await Task.Run(() => Directory.Delete(fullPath, true));
            }
            else
            {
                throw new DirectoryNotFoundException("");
            }
        }

        public async Task<IEnumerable<string>> ListAsync(string directory)
        {
            string fullPath = Path.Combine(_basePath, directory);
            if (!Directory.Exists(fullPath))
                return new List<string>();

            var entries = await Task.Run(() =>
                Directory.GetFiles(fullPath).Select(Path.GetFileName)
                .Concat(Directory.GetDirectories(fullPath).Select(Path.GetFileName))
            );

            return entries;
        }

        public async Task MoveAsync(string from, string to, string name)
        {
            string fullFromPath = Path.Combine(_basePath, from);
            string fullToPath = Path.Combine(_basePath, to, name);

            if (!Directory.Exists(fullFromPath))
                throw new DirectoryNotFoundException("");

            await Task.Run(() => Directory.Move(fullFromPath, fullToPath));
        }
    }
}
