using MediatR;

namespace Application.Commands.CreateFolder
{
    public record CreateFolderCommand(string Name, Guid ParentId, Guid UserId) : IRequest<Guid>;
}
