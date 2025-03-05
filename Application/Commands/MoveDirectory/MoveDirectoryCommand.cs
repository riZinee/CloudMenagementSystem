using MediatR;

namespace Application.Commands.MoveDirectory
{
    public record MoveDirectoryCommand(Guid FromId, Guid ToId, Guid UserId) : IRequest;
}
