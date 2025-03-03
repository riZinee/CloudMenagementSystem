using MediatR;

namespace Application.Commands.CreateHomeCatalog
{
    public record CreateHomeCatalogCommand(string path, Guid userId) : IRequest<Guid>;
}
