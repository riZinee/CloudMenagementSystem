using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;
using MediatR;
using System.Text.RegularExpressions;
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

            existingUser = await _userRepository.GetByNameAsync(request.Name);

            if (existingUser is not null)
            {
                throw new DomainEntityAlreadyExistsException(Messages.NameIsAlreadyTaken);
            }

            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=!]).{8,}$";
            Regex regex = new Regex(pattern);

            if (!regex.IsMatch(request.Password))
            {
                throw new ValidationDomainException(Messages.InvalidPasswordFormat);
            }


            var salt = _identityService.GenerateSalt();
            var passwordHash = _identityService.HashPassword(request.Password, salt);

            var user = new User(request.Name, email, passwordHash, salt);

            await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }
    }
}
