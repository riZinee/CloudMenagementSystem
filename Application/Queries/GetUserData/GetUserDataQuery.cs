using Application.DTOs;
using MediatR;

namespace Application.Queries.GetUserData
{
    public record GetUserDataQuery(string userId) : IRequest<UserDTO>;
}
