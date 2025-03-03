using Domain.Exceptions;

namespace Domain.Entities
{
    public class FolderMetadata : StorageMetadata
    {
        public ICollection<StorageMetadata> SubStorage { get; private set; } = new List<StorageMetadata>();

        public FolderMetadata()
        {
        }

        public FolderMetadata(string name, Guid ownerId, string path, FolderMetadata? parent = null) : base(name, ownerId, path, parent)
        {

        }

        public FolderMetadata(string name, Guid ownerId, FolderMetadata? parent) : base(name, ownerId, parent)
        {

        }

        public FolderMetadata CreateSubFolder(string name)
        {
            var folder = new FolderMetadata(name, OwnerId, this);
            SubStorage.Add(folder);
            return folder;
        }

        public FolderMetadata CreateSubFolder(string name, Guid ownerId)
        {
            var folder = new FolderMetadata(name, ownerId, this);
            SubStorage.Add(folder);
            return folder;
        }

        public void AddSubFolder(FolderMetadata folder)
        {
            if (folder == null)
            {
                throw new DomainException("The folder cannot be null");
            }

            if (folder.ContainsFolder(this) || this.ContainsFolder(folder))
            {
                throw new DomainException("A folder cannot contain itself");
            }
            folder.Parent = this;

            SubStorage.Add(folder);
        }

        public void RemoveSubFolder(FolderMetadata folder)
        {
            if (folder == null)
                throw new DomainException("Folder nie może być null.");

            if (!SubStorage.Contains(folder))
                throw new DomainException("Folder nie istnieje w tym katalogu.");

            SubStorage.Remove(folder);
        }


        public void AddFile(FileMetadata file)
        {
            if (SubStorage.Contains(file))
            {
                throw new DomainException("The file already exists in this folder");
            }

            file.Parent = this;
            SubStorage.Add(file);
        }

        public void RemoveFile(FileMetadata file)
        {
            if (file == null)
                throw new DomainException("Plik nie może być null.");

            if (!SubStorage.Contains(file))
                throw new DomainException("Plik nie istnieje w tym folderze.");

            SubStorage.Remove(file);
        }


        public bool ContainsFolder(FolderMetadata folder)
        {
            if (folder == this)
            {
                return true;
            }

            foreach (StorageMetadata subStorage in SubStorage)
            {
                if (subStorage == folder)
                {
                    return true;
                }

                if (subStorage is FolderMetadata subFolder)
                {
                    if (subFolder.ContainsFolder(folder))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
