using Domain.Entities;
using MediatR;

namespace Application.Queries.GetFile
{
    public record GetFileQuery(Guid FileId, Guid UserId) : IRequest<FileMetadata>;
}
