using MediatR;

namespace Application.Commands.DeleteDirectory
{
    public record DeleteDirectoryCommand(Guid DirectoryId, Guid UserId) : IRequest;
}
