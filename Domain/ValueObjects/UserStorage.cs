using Domain.Exceptions;

namespace Domain.ValueObjects
{
    public record UserStorage
    {
        public long UsedSpace { get; private set; }
        public long TotalSpace { get; private set; }

        public UserStorage(long totalSpace)
        {
            if (totalSpace < 0)
                throw new DomainException("Całkowita przestrzeń musi być większa niż 0.");
            TotalSpace = totalSpace;
            UsedSpace = 0;
        }
        public UserStorage(long usedSpace, long totalSpace)
        {
            if (totalSpace < 0)
                throw new DomainException("Całkowita przestrzeń musi być większa niż 0.");
            if (usedSpace < 0)
                throw new DomainException("Użyta przestrzeń musi być większa niż 0.");
            TotalSpace = totalSpace;
            UsedSpace = usedSpace;
        }

        public bool CanStoreFile(long fileSize)
        {
            return UsedSpace + fileSize <= TotalSpace;
        }

    }
}

