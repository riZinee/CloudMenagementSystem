using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class SignalRService
{
    private HubConnection _hubConnection;
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private static readonly string apiUrl = "http://localhost:5087/api/files/upload"; // Adres API
    private static readonly string hubUrl = "http://localhost:5087/uploadProgressHub"; // Adres SignalR
    public string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1ZDg3MjFmNy02NTVmLTQwZWMtYjhhNi1iM2ZhYzI2NjYxOGMiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjAxNmE0OGY1LTYzZjEtNDU1OC05ZDUwLTZiNTI4NjMzYzVjMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzQyMzEzMjg0LCJpc3MiOiJteUFwcCIsImF1ZCI6Im15QXBwIn0.m-dLPnqXaOAq_q1KnjKMkmCJlQwd_uK6Vs14UZjzRTM";


    public event Action<int> OnProgressUpdated; // Dodaj event dla UI

    public SignalRService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }

    public async Task UploadFileAsync(string fileName, IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10L * 1024 * 1024 * 1024)); // Otwórz strumień

        // 📌 Wymuszenie poprawnego Content-Type
        string contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        content.Add(fileContent, "file", file.Name);
        content.Add(new StringContent($"uploads/{fileName}"), "destinationPath");

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }

        Console.WriteLine("Wysyłanie pliku...");

        var response = await _httpClient.PostAsync("https://localhost:7132/api/files/upload/chunk", content);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await refreshTokenAsync();
            }
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($" Błąd przesyłania pliku: {response.StatusCode} - {error}");
        }
        else
        {
            Console.WriteLine(" Plik przesłany pomyślnie!");
        }
    }

    public async Task refreshTokenAsync()
    {
        var apiUrl = "https://localhost:7132/api/auth/refresh"; // Zmień na prawidłowy adres API

        var loginDTO = new
        {
            jwt = token,
            refreshToken = "AB6ns171KK0CnjaggrmrLKnRsegrQ7g9JxTHBBSuto5ZxNz9TBKcUzlCVdGFETca4SXTH+JcdTEquqt6RPM5/PQsIO51yWfhxwh0IOUKhAFx0XyIL9MI5WeoKGKGvK+IOqAL29UdlDFzu5X7vzl1Zee351DF21/jTd/2T6SeujZs+RrZYECdHD6ffZCU/e49O2Zldu8f6DfJ39nUEQXrC7Kitp/7eidHscpM9ifq8/cEUxgmGB3z3rIuyxejXH4CbKWM2/sZmM4FS63GVQ131SwgmdiRlfLY4A8/11hZzYwhiuoYVQwBfUPRGM787+M3jSO2X82S5bVqH5W3nNSI5GcuulXCoiD8dtxmgXQR7b1c40jUymSIqwN0zMhGPjGVjUF8eq07DH2y5XYFXqOquDWJiVLGzCsngN2SjM32Pcdvp7AugAdN+GCFtzxSQbvAfI2Ki0DH5VT5ywJHu5BcPuZSDJbhfReb3cMFEYoiH2VsmKDDreyyiuO99VozzvoHYEyOz1QgDbgttsYOT9kdgvPerpNPKOWP/FFvsGa7nFBTg/Ep9WwUx/CJSqb/KG3vEHdSF/KBS46+Rk+YJE6tZy7Y8J88TRfD5AdDgZqhnyCcU0hucJ5QOoeE/6MimBClUfPngTi6fWHSzcD18MZXfLol22RX9baP8DqwOS4HO3YrgXLwBF+44zilJWhU3hkk+4nDYTspZftQqvthz5mWtj2Y0R1c1jThsaR6JUZidTs0wTM/p3G/saXuBS4nwR98igmSpIoRmlNOSKZntvstDPmdyUoctC8MSTP/0HdQcyltDIuasiggPqh7NNa26iw89BG4UdmL9/2/s5G6WebcvyMhg17igYgdWQYKi7UbjagoN7P1K9vZP5Xx++mRZ6jW1GvrJyP0u6knNudyyRzzmRFIlOVuuTiiUJxBQ1vxWuI9CGfJmJmoqkR7LK7zE13FTblroPBuBsRSvGmHT7UoW4Og6XpjmY5ha55cs6LUUZ2+gdg2yKAYcc/dGc4vk47nDm8jnWz5X1KRQmGbnqPVaJhWjn01aKmdM+aX96FRaqU2YahS0ug9dQ/UUKEuHL0i24jSxJDaOPLHwZXJ6feHLAgwPYZ1+RqR1rp8kteXV+Cu1CLwV1/AY6pcHM6A3maBVXrZ7d9pkmOUU0H1bEe2TYyViyKFHrp/YFWmKF8hACOfyWhryzL2qDq9lLiSTEIb+FDaeVl+FlQNUEPN9UsKYk9WW5kU+tQalNlpS5SA6jfnWWbfh6SJ91Ix+V4K1lT0chf9MVoFJfVgOVnaHdnmwiVFMy0GlJlZXiu4CG3c+91aQZ9Kp5ON8B2dWXYlQ+8VhNhXpt0yo5earKdIlQtX/A=="
        };

        using var httpClient = new HttpClient();

        var jsonContent = JsonSerializer.Serialize(loginDTO);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            token = responseContent;
            Console.WriteLine("Otrzymany nowy token: " + responseContent);
        }
        else
        {
            Console.WriteLine($"Błąd: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
        }
    }


}
