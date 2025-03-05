using Application.DTOs.Responses;
using MediatR;

namespace Application.Queries.ListDirectory
{
    public record ListDirectoryQuery(Guid UserId, Guid DirectoryId) : IRequest<List<StorageResponse>>;
}
