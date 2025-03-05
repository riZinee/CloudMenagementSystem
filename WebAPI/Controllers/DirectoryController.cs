using Application.Commands.CreateDirectory;
using Application.Commands.CreateHomeCatalog;
using Application.Commands.DeleteDirectory;
using Application.Commands.MoveDirectory;
using Application.Commands.RenameDirectory;
using Application.DTOs.Requests;
using Application.Queries.ListDirectory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[Route("api/directories")]
[ApiController]
public class DirectoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public DirectoryController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDirectoryRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == Guid.Empty)
        {
            throw new ApplicationException("");
        }

        var command = new CreateDirectoryCommand(request.Name, request.ParentId, userId);
        var catalogId = await _mediator.Send(command);

        return Ok(catalogId);
    }

    [HttpPost("home")]
    public async Task<IActionResult> CreateHomeCatalog([FromBody] CreateHomeDirectoryRequest request)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (userRole != "Admin")
        {
            throw new ApplicationException();
        }

        if (request.UserId == Guid.Empty)
        {
            throw new ApplicationException("");
        }

        var command = new CreateHomeDirectoryCommand(request.Path, request.UserId);
        var catalogId = await _mediator.Send(command);

        return Ok(catalogId);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDirectory(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == Guid.Empty)
        {
            throw new ApplicationException("");
        }

        var command = new DeleteDirectoryCommand(id, userId);

        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("move")]
    public async Task<IActionResult> Move([FromBody] MoveDirectoryRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == Guid.Empty)
        {
            throw new ApplicationException("");
        }

        var command = new MoveDirectoryCommand(request.FromId, request.ToId, userId);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("rename")]
    public async Task<IActionResult> RenameDirectory(RenameDirectoryRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == Guid.Empty)
        {
            throw new ApplicationException("");
        }

        var command = new RenameDirectoryCommand(request.Name, request.Id, userId);
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

        var query = new ListDirectoryQuery(userId, id);
        var response = await _mediator.Send(query);

        return Ok(response);
    }

}
