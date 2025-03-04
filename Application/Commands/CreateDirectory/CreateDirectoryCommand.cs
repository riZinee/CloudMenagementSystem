using MediatR;

namespace Application.Commands.CreateDirectory
{
    public record CreateDirectoryCommand(string Name, Guid ParentId, Guid UserId) : IRequest<Guid>;
}
