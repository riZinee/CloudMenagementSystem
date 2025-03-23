using Application.Commands.ChangeUserName;
using Application.Commands.ChangeUserPassword;
using Application.Commands.IncreaseUserSpace;
using Application.DTOs.Requests;
using Application.Queries.GetUserData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
[Authorize]
[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
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
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var command = new ChangeUserPasswordCommand(request.OldPassword, request.NewPassword, User.FindFirstValue(ClaimTypes.NameIdentifier));
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("storage")]
    public async Task<IActionResult> ChangePassword(IncreaseUserStorageRequest request)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (userRole != "Admin")
        {
            throw new ApplicationException();
        }

        var command = new InceraseUserSpaceCommand(request.UserId, request.Space);
        await _mediator.Send(command);

        return Ok();
    }
}
