using Application.DTOs.Responses;
using Application.Interfaces;
using MediatR;

namespace Application.Queries.GetFileUpload
{
    public class GetFileUploadQueryHandler : IRequestHandler<GetFileUploadQuery, FileUploadResponse>
    {
        private readonly IFileUploadRepository _fileUploadRepository;

        public GetFileUploadQueryHandler(IFileUploadRepository fileUploadRepository)
        {
            _fileUploadRepository = fileUploadRepository;
        }

        async Task<FileUploadResponse> IRequestHandler<GetFileUploadQuery, FileUploadResponse>.Handle(GetFileUploadQuery request, CancellationToken cancellationToken)
        {
            var fileUpload = await _fileUploadRepository.GetByIdAsync(request.id);

            return new FileUploadResponse(fileUpload.UploadedChunks, fileUpload.TotalChunks, fileUpload.IsCompleted);
        }
    }
}
