using Application.DTOs.Response;
using MediatR;

namespace Application.Queries.GetUserData
{
    public record GetUserDataQuery(string userId) : IRequest<UserResponse>;
}
