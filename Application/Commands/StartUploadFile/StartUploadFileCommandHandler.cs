﻿using Application.Interfaces;
using MediatR;

namespace Application.Commands.StatUploadFile
{
    public class StartUploadFileCommandHandler : IRequestHandler<StartUploadFileCommand, Guid>
    {
        private readonly IFileService _storageService;

        public StartUploadFileCommandHandler(IFileService storageService)
        {

            _storageService = storageService;
        }

        public async Task<Guid> Handle(StartUploadFileCommand request, CancellationToken cancellationToken)
        {
            var uploadId = Guid.NewGuid();

            await _storageService.UploadFileAsync(request.FileStream, request.DestinationPath, Guid.Parse(request.UserId));
            return uploadId;
        }
    }

}
