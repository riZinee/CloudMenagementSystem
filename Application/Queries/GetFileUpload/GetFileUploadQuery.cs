using Application.DTOs.Responses;
using MediatR;

namespace Application.Queries.GetFileUpload
{
    public record GetFileUploadQuery(Guid id) : IRequest<FileUploadResponse>;
}
