using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public record Email
    {
        public string Value { get; }

        public Email(string value)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(value);
            if (string.IsNullOrWhiteSpace(value) || !match.Success)
            {
                throw new ValidationDomainException(Messages.InvalidEmail);
            }

            Value = value;
        }

        public override string ToString() => Value;
    }
}
