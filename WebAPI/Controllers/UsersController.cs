using Application.Commands.ChangeUserName;
using Application.Commands.ChangeUserPassword;
using Application.DTOs;
using Application.Queries.GetUserData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
[Authorize]
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }


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

    [HttpGet]
    public async Task<IActionResult> GetUserData()
    {
        var query = new GetUserDataQuery(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var data = await _mediator.Send(query);

        return Ok(data);
    }

    [HttpPost("name")]
    public async Task<IActionResult> ChangeName(string name)
    {
        var command = new ChangeUserNameCommand(name, User.FindFirstValue(ClaimTypes.NameIdentifier));
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("password")]
    public async Task<IActionResult> ChangePassword(PasswordDTO passwords)
    {
        var command = new ChangeUserPasswordCommand(passwords.OldPassword, passwords.NewPassword, User.FindFirstValue(ClaimTypes.NameIdentifier));
        await _mediator.Send(command);

        return Ok();
    }
}
