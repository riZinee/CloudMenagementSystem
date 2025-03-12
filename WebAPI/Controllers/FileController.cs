using Application.Commands.StatUploadFile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FileController(IMediator mediator)
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

        [HttpPost("upload/chunk")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFileChunk(
            [FromForm] IFormFile chunk,
            [FromForm] string destinationPath,
            [FromForm] int chunkIndex,
            [FromForm] int totalChunks)
        {
            if (chunk == null)
                return BadRequest("Brak fragmentu pliku.");

            var uploadId = await _mediator.Send(new UploadFileChunkCommand(
                chunk.OpenReadStream(),
                destinationPath,
                chunkIndex,
                totalChunks,
                User.FindFirstValue(ClaimTypes.NameIdentifier)));

            return Ok(uploadId);
        }

    }
}
