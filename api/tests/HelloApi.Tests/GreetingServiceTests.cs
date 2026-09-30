using HelloApi;

namespace HelloApi.Tests;

public class GreetingServiceTests
{
    private readonly GreetingService _sut = new();

    [Fact]
    public void TryGreet_ReturnsGreeting_WhenNameProvided()
    {
        var ok = _sut.TryGreet("  Ada ", out var message);

        Assert.True(ok);
        Assert.Equal("Hello, Ada!", message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryGreet_ReturnsFalse_WhenNameBlank(string? name)
    {
        var ok = _sut.TryGreet(name, out var message);

        Assert.False(ok);
        Assert.Equal(string.Empty, message);
    }
}
