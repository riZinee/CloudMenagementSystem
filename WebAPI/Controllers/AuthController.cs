using Application.Commands.CreateUser;
using Application.Commands.LoginUser;
using Application.Commands.RefreshToken;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public AuthController(IUserRepository userRepository, IUnitOfWork unitOfWork, IMediator mediator)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand command)
        {

            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] LoginDTO loginDTO)
        {
            var newToken = await _mediator.Send(new RefreshTokenCommand(loginDTO.Jwt, loginDTO.RefreshToken));
            return Ok(newToken);
        }

        [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount([FromQuery] string token)
        {
            var user = await _userRepository.GetByActivationTokenAsync(token);
            if (user == null)
            {
                return BadRequest("Invalid or expired token.");
            }

            user.Activate();
            await _unitOfWork.SaveChangesAsync();

            return Ok("Account activated successfully!");
        }
    }
}
