using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.GetUserData
{
    public class GetUserDataQueryHandler : IRequestHandler<GetUserDataQuery, UserDTO>
    {
        private readonly IUserRepository _userRepository;

        public GetUserDataQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO> Handle(GetUserDataQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(request.userId));

            return new UserDTO(user.Name, user.Email.Value, user.UserStorage.UsedSpace, user.UserStorage.TotalSpace);
        }
    }
}
