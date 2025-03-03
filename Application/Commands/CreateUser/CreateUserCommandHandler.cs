using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;
namespace Application.Commands.CreateUser
{


    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _identityService = identityService;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var email = new Email(request.Email);

            var existingUser = await _userRepository.GetByEmailAsync(email);

            if (existingUser is not null)
            {
                throw new DomainEntityAlreadyExistsException(Messages.UserAlreadyExists);
            }

            var user = new User(request.Name, new Email(request.Email), request.Password);
            user.Salt = _identityService.GenerateSalt();
            user.PasswordHash = _identityService.HashPassword(user.PasswordHash, user.Salt);

            await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }
    }
}
