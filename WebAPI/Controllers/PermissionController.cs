using Application.Commands.AddPermisions;
using Application.Commands.RemovePermissions;
using Application.Commands.UpdatePermissions;
using Application.DTOs.Requests;
using Application.Queries.ListStoragePermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[Route("api/permissions")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    public async Task<IActionResult> Add([FromBody] PermissionRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == Guid.Empty)
        {
            throw new ApplicationException("");
        }

        var command = new AddPermissionsCommand(userId, request.UserId, request.StorageId, request.Values);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("remove")]
    public async Task<IActionResult> DeleteDirectory(RemovePermissionRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == Guid.Empty)
        {
            throw new ApplicationException("");
        }

        var command = new RemovePermissionsCommand(userId, request.UserId, request.StorageId);

        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("update")]
    public async Task<IActionResult> Move([FromBody] PermissionRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == Guid.Empty)
        {
            throw new ApplicationException("");
        }

        var command = new UpdatePermissionsCommand(userId, request.UserId, request.StorageId, request.Values);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("list")]
    public async Task<IActionResult> ListDirectory(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == Guid.Empty)
        {
            throw new ApplicationException("");
        }

        var query = new ListPermisionsQuery(userId, id);
        var response = await _mediator.Send(query);

        return Ok(response);
    }

}
