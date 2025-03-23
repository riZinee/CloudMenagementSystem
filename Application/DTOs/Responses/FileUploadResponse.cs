namespace Application.DTOs.Responses
{
    public record FileUploadResponse(int UploadedChunks, int TotalChunks, bool IsCompleted);
}
