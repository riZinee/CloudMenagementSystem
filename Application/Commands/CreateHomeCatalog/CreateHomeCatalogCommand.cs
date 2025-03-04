using MediatR;

namespace Application.Commands.CreateHomeCatalog
{
    public record CreateHomeCatalogCommand(string Path, Guid UserId) : IRequest<Guid>;
}
