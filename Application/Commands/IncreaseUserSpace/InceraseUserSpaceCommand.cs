using MediatR;

namespace Application.Commands.IncreaseUserSpace
{
    public record InceraseUserSpaceCommand(Guid UserId, long Space) : IRequest;
}
