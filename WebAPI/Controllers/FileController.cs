using Application.Commands.StatUploadFile;
using Application.Commands.UploadFileChunk;
using Application.Queries.DownloadFile;
using Application.Queries.GetFile;
using Application.Queries.GetFileUpload;
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
            [FromForm] Guid? uploadId,
            [FromForm] string fileName,
            [FromForm] Guid directoryId,
            [FromForm] int chunkIndex,
            [FromForm] int totalChunks,
            [FromForm] long fileSize)
        {
            if (chunk == null)
                return BadRequest("Brak fragmentu pliku.");


            var assignedUploadId = await _mediator.Send(new UploadFileChunkCommand(
                Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                uploadId ?? Guid.NewGuid(),
                chunk.OpenReadStream(),
                fileName,
                directoryId,
                chunkIndex,
                totalChunks,
                fileSize));

            return Ok(assignedUploadId);
        }

        [HttpGet("upload/status/{uploadId}")]
        public async Task<IActionResult> GetUploadStatus(Guid uploadId)
        {
            var query = new GetFileUploadQuery(uploadId);

            var respone = await _mediator.Send(query);

            return Ok(respone);
        }

        [HttpGet("download/{fileId}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var query = new DownloadFileQuery(fileId, userId);
            var fileStream = await _mediator.Send(query);

            if (fileStream == null)
            {
                return NotFound();
            }

            // Pobierz nazwę pliku z bazy danych
            var file = await _mediator.Send(new GetFileQuery(fileId, userId));

            if (file == null)
            {
                return NotFound();
            }

            return new FileStreamResult(fileStream, "application/octet-stream")
            {
                FileDownloadName = file.Name
            };
        }

    }
}
