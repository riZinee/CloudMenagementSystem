using MediatR;

namespace Application.Commands.CreateHomeCatalog
{
    public record CreateHomeDirectoryCommand(string Path, Guid UserId) : IRequest<Guid>;
}
