using MediatR;

namespace Application.Commands.CreateFolder
{
    public record CreateFolderCommand(string name, Guid parentId, Guid userId) : IRequest<Guid>;
}
