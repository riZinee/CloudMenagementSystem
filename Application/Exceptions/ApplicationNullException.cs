namespace Application.Exceptions
{
    public class ApplicationNullException : ApplicationException
    {
        public ApplicationNullException()
        {
        }

        public ApplicationNullException(string? message) : base(message)
        {
        }
    }
}
