using Domain.Events;
using Domain.Exceptions;

namespace Domain.Entities
{
    public class FileMetadata : StorageMetadata
    {
        public long Size { get; private set; }
        public string ContentType { get; private set; }

        public FileMetadata()
        {
        }

        public FileMetadata(Guid userId, string fileName, long size, string contentType, string path, DirectoryMetadata parent) : base(fileName, userId, path, parent)
        {
            if (size <= 0)
                throw new DomainException("Rozmiar pliku musi być większy niż 0.");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("Nazwa pliku nie może być pusta.");

            if (string.IsNullOrWhiteSpace(contentType))
                throw new DomainException("Rozszerzenie pliku nie może być puste.");

            Size = size;
            ContentType = contentType;

            _domainEvents.Add(new FileUploadedEvent(Id, userId, fileName, size));
        }

        public FileMetadata(Guid userId, string fileName, long size, string contentType, DirectoryMetadata parent) : base(fileName, userId, parent)
        {
            if (size <= 0)
                throw new DomainException("Rozmiar pliku musi być większy niż 0.");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("Nazwa pliku nie może być pusta.");

            if (string.IsNullOrWhiteSpace(contentType))
                throw new DomainException("Rozszerzenie pliku nie może być puste.");

            Size = size;
            ContentType = contentType;

            _domainEvents.Add(new FileUploadedEvent(Id, userId, fileName, size));
        }


    }
}
