using Application.Commands.CreateDirectory;
using Application.Commands.CreateHomeCatalog;
using Application.DTOs;
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
    public async Task<IActionResult> Create([FromBody] DirectoryDTO directoryDTO)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name));

        if (userId == null)
        {
            throw new ApplicationException("");
        }

        var command = new CreateDirectoryCommand(directoryDTO.Name, directoryDTO.ParentId, userId);
        var catalogId = await _mediator.Send(command);
        return Ok(catalogId);
    }

    [HttpPost("home")]
    public async Task<IActionResult> CreateHomeCatalog([FromBody] string path)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name));

        if (userId == null)
        {
            throw new ApplicationException("");
        }

        var command = new CreateHomeDirectoryCommand(path, userId);
        var catalogId = await _mediator.Send(command);
        return Ok(catalogId);
    }

}
