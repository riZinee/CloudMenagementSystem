namespace Domain.Exceptions
{
    public class DomainEntityAlreadyExistsException : DomainException
    {
        public DomainEntityAlreadyExistsException()
        {
        }

        public DomainEntityAlreadyExistsException(string? message) : base(message)
        {
        }
    }
}
