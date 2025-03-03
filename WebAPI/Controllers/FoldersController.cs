using Application.Commands.CreateFolder;
using Application.Commands.CreateHomeCatalog;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[Route("api/folders")]
[ApiController]
public class FoldersController : ControllerBase
{
    private readonly IMediator _mediator;

    public FoldersController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FolderDTO folderDTO)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name));

        if (userId == null)
        {
            throw new ApplicationException("");
        }

        var command = new CreateFolderCommand(folderDTO.name, folderDTO.parentId, userId);
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

        var command = new CreateHomeCatalogCommand(path, userId);
        var catalogId = await _mediator.Send(command);
        return Ok(catalogId);
    }

}
