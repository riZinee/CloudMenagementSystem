using Application.Commands.CreateUser;
using Application.Commands.LoginUser;
using Application.Commands.RefreshToken;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("abc");
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Nie udało się pobrać informacji o użytkowniku.");
        }

        return Ok(new { Id = userId, Email = email });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] LoginDTO loginDTO)
    {
        var newToken = await _mediator.Send(new RefreshTokenCommand(loginDTO.jwt, loginDTO.refreshToken));
        return Ok(newToken);
    }
}
