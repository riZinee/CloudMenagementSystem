using MediatR;

namespace Application.Commands.StatUploadFile
{
    public record StartUploadFileCommand(string FileName, Stream FileStream, string DestinationPath, string UserId) : IRequest<Guid>;

}