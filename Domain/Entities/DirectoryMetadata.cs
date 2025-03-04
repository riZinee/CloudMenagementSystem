using Domain.Exceptions;

namespace Domain.Entities
{
    public class DirectoryMetadata : StorageMetadata
    {
        public ICollection<StorageMetadata> SubStorage { get; private set; } = new List<StorageMetadata>();

        public DirectoryMetadata()
        {
        }

        public DirectoryMetadata(string name, Guid ownerId, string path, DirectoryMetadata? parent = null) : base(name, ownerId, path, parent)
        {

        }

        public DirectoryMetadata(string name, Guid ownerId, DirectoryMetadata? parent) : base(name, ownerId, parent)
        {

        }

        public DirectoryMetadata CreateSubDirectory(string name)
        {
            var directory = new DirectoryMetadata(name, OwnerId, this);
            SubStorage.Add(directory);
            return directory;
        }

        public DirectoryMetadata CreateSubDirectory(string name, Guid ownerId)
        {
            var directory = new DirectoryMetadata(name, ownerId, this);
            SubStorage.Add(directory);
            return directory;
        }

        public void AddSubDirectory(DirectoryMetadata directory)
        {
            if (directory == null)
            {
                throw new DomainException("The directory cannot be null");
            }

            if (directory.ContainsDirectory(this) || this.ContainsDirectory(directory))
            {
                throw new DomainException("A directory cannot contain itself");
            }
            directory.Parent = this;

            SubStorage.Add(directory);
        }

        public void RemoveSubDirectory(DirectoryMetadata directory)
        {
            if (directory == null)
                throw new DomainException("Directory nie może być null.");

            if (!SubStorage.Contains(directory))
                throw new DomainException("Directory nie istnieje w tym katalogu.");

            SubStorage.Remove(directory);
        }


        public void AddFile(FileMetadata file)
        {
            if (SubStorage.Contains(file))
            {
                throw new DomainException("The file already exists in this directory");
            }

            file.Parent = this;
            SubStorage.Add(file);
        }

        public void RemoveFile(FileMetadata file)
        {
            if (file == null)
                throw new DomainException("Plik nie może być null.");

            if (!SubStorage.Contains(file))
                throw new DomainException("Plik nie istnieje w tym directoryze.");

            SubStorage.Remove(file);
        }


        public bool ContainsDirectory(DirectoryMetadata directory)
        {
            if (directory == this)
            {
                return true;
            }

            foreach (StorageMetadata subStorage in SubStorage)
            {
                if (subStorage == directory)
                {
                    return true;
                }

                if (subStorage is DirectoryMetadata subDirectory)
                {
                    if (subDirectory.ContainsDirectory(directory))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
