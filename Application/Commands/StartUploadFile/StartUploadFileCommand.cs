using MediatR;

namespace Application.Commands.StatUploadFile
{
    public record StartUploadFileCommand(string fileName, Stream fileStream, string destinationPath, string userId) : IRequest<Guid>;

}