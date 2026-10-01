using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

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
    public async Task Ping_ReturnsPongJson()
    {
        var response = await _client.GetAsync("/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        using var doc = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(System.Text.Json.JsonValueKind.True, doc.RootElement.GetProperty("pong").ValueKind);
    }

    [Fact]
    public async Task Root_ReturnsHelloWorld()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Hello World!", await response.Content.ReadAsStringAsync());
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

    [Theory]
    [InlineData("%20")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    public async Task Greet_ReturnsProblemDetails400_ForInvalidName(string name)
    {
        var response = await _client.GetAsync($"/greet/{name}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var doc = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.False(string.IsNullOrEmpty(doc.RootElement.GetProperty("title").GetString()));
        Assert.False(string.IsNullOrEmpty(doc.RootElement.GetProperty("detail").GetString()));
        Assert.Equal(400, doc.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task UnhandledException_ReturnsProblemDetails500_WithoutInternals()
    {
        using var throwing = factory.WithWebHostBuilder(b => b.ConfigureServices(
            services => services.AddSingleton<VersionService, ThrowingVersionService>()));
        var response = await throwing.CreateClient().GetAsync("/version");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = System.Text.Json.JsonDocument.Parse(body);
        Assert.Equal(500, doc.RootElement.GetProperty("status").GetInt32());
        Assert.DoesNotContain("secret internal detail", body);
        Assert.DoesNotContain("ThrowingVersionService", body);
        Assert.DoesNotContain("   at ", body);
    }

    private class ThrowingVersionService : VersionService
    {
        public override string GetVersion() => throw new InvalidOperationException("secret internal detail");
    }
}
