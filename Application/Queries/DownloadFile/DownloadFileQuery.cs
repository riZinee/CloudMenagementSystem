using MediatR;

namespace Application.Queries.DownloadFile
{
    public record DownloadFileQuery(Guid FileId, Guid UserId) : IRequest<Stream>;
}
