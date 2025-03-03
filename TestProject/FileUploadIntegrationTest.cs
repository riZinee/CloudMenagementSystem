using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

public class FileUploadIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly string _uploadEndpoint = "/api/files/upload";
    private readonly string _signalrEndpoint = "http://localhost:5000/uploadProgressHub";

    public FileUploadIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UploadFile_Should_TrackProgress_To_100()
    {
        // ✅ 1. Inicjalizacja SignalR Client
        var progressUpdates = new List<int>();
        var connection = new HubConnectionBuilder()
            .WithUrl(_signalrEndpoint)
            .WithAutomaticReconnect()
            .Build();

        connection.On<object>("UploadProgress", data =>
        {
            var progress = Convert.ToInt32(data.GetType().GetProperty("Progress")?.GetValue(data, null));
            progressUpdates.Add(progress);
        });

        await connection.StartAsync();

        // ✅ 2. Wysłanie pliku do API
        var fileContent = new ByteArrayContent(new byte[1024 * 100]); // 100 KB dummy file
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

        var formData = new MultipartFormDataContent
        {
            { fileContent, "file", "testfile.txt" },
            { new StringContent("test-user"), "userId" },
            { new StringContent("uploads/testfile.txt"), "destinationPath" }
        };

        var response = await _client.PostAsync(_uploadEndpoint, formData);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        var uploadId = result.UploadId.ToString();

        // ✅ 3. Oczekiwanie na 100% progresu
        var maxWaitTime = TimeSpan.FromSeconds(10);
        var startTime = DateTime.UtcNow;

        while (!progressUpdates.Contains(100))
        {
            if (DateTime.UtcNow - startTime > maxWaitTime)
            {
                throw new TimeoutException("Nie otrzymano 100% progresu.");
            }

            await Task.Delay(500); // Sprawdzanie co 500ms
        }

        // ✅ 4. Weryfikacja
        progressUpdates.Should().Contain(100);
        await connection.StopAsync();
    }
}

