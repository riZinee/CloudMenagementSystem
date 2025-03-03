using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace WebUi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        Console.WriteLine("aaaaaaaaaaaaaaaaaaa");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5087") });
        builder.Services.AddScoped<SignalRService>();

        await builder.Build().RunAsync();
    }
}
