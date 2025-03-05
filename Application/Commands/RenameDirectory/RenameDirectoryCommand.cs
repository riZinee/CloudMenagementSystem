using MediatR;

namespace Application.Commands.RenameDirectory
{
    public record RenameDirectoryCommand(string Name, Guid DirectoryId, Guid UserId) : IRequest;
}
