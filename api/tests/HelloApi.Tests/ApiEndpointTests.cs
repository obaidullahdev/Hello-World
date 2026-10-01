using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HelloApi.Tests;

public class ApiEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("ok", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Version_ReturnsVersionJson()
    {
        var response = await _client.GetAsync("/version");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        using var doc = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("0.1.0", doc.RootElement.GetProperty("version").GetString());
    }

    [Fact]
    public async Task Greet_ReturnsMessage_ForValidName()
    {
        var response = await _client.GetAsync("/greet/Ada");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Hello, Ada!", await response.Content.ReadAsStringAsync());
    }
}
