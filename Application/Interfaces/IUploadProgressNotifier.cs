namespace Application.Interfaces
{
    public interface IUploadProgressNotifier
    {
        Task NotifyProgressAsync(string userId, string fileName, int progress);
    }

}
