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
    public async Task Greet_ReturnsMessage_ForValidName()
    {
        var response = await _client.GetAsync("/greet/Ada");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Hello, Ada!", await response.Content.ReadAsStringAsync());
    }
}
