﻿using Application.Commands.StatUploadFile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/files")]
    public class FileUploadController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FileUploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFile(
            [FromForm] IFormFile file,
            [FromForm] string destinationPath)
        {
            if (file == null)
                return BadRequest("Brak pliku do przesłania.");

            var uploadId = await _mediator.Send(new StartUploadFileCommand(file.FileName, file.OpenReadStream(), destinationPath, User.FindFirstValue(ClaimTypes.NameIdentifier)));

            return Ok(uploadId);
        }
    }
}
