using MediatR;

namespace Application.Commands.MoveFile
{
    public record MoveFileCommand(Guid FromId, Guid ToId, Guid UserId) : IRequest;
}
