namespace Application.Exceptions
{
    public class ApplicationAuthorizationException : ApplicationException
    {
        public ApplicationAuthorizationException()
        {
        }

        public ApplicationAuthorizationException(string? message) : base(message)
        {
        }
    }
}
