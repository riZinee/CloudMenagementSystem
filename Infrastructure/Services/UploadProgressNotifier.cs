using Application.Interfaces;
using Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class UploadProgressNotifier : IUploadProgressNotifier
    {
        private readonly IHubContext<UploadProgressHub> _hubContext;

        public UploadProgressNotifier(IHubContext<UploadProgressHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyProgressAsync(string userId, string fileName, int progress)
        {
            Console.WriteLine($"[UploadProgressNotifier] Wysyłanie postępu do użytkownika: {userId}, progress={progress}%");
            var data = new { userId, fileName, progress };


            await _hubContext.Clients.User(userId).SendAsync("UploadProgress", data);
            //await _hubContext.Clients.All.SendAsync("UploadProgress", data);
        }
    }
}
