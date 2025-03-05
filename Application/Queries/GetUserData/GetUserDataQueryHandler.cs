using Application.DTOs.Response;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.GetUserData
{
    public class GetUserDataQueryHandler : IRequestHandler<GetUserDataQuery, UserResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetUserDataQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> Handle(GetUserDataQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(request.userId));

            return new UserResponse(user.Name, user.Email.Value, user.UserStorage.UsedSpace, user.UserStorage.TotalSpace);
        }
    }
}
