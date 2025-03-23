using MediatR;

namespace Application.Commands.UploadFileChunk
{
    public record UploadFileChunkCommand(Guid UserId, Guid UploadId, Stream FileStream, string FileName, Guid DirectoryId, int ChunkIndex, int TotalChunks, long FileSize) : IRequest<Guid>;
}
