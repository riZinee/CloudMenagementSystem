using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Infrastructure.SignalR
{
    [Authorize]
    public class UploadProgressHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UploadProgressHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                Console.WriteLine("Brak `HttpContext` w SignalR.");
                return;
            }

            var accessToken = httpContext.Request.Headers["Authorization"];

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Clients.All.SendAsync(Context.ConnectionId);


            await base.OnConnectedAsync();
        }

    }
}
