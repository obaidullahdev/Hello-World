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

    [Fact]
    public void TryGreet_ReturnsFalse_WhenNameLongerThan50()
    {
        var ok = _sut.TryGreet(new string('a', 51), out var message);

        Assert.False(ok);
        Assert.Equal(string.Empty, message);
    }

    [Fact]
    public void TryGreet_ReturnsGreeting_WhenNameExactly50()
    {
        Assert.True(_sut.TryGreet(new string('a', 50), out _));
    }

    [Fact]
    public void Validate_ReturnsBlankError_WhenBlank()
    {
        Assert.Equal(GreetingService.BlankNameError, _sut.Validate("  "));
    }

    [Fact]
    public void Validate_ReturnsTooLongError_WhenOver50()
    {
        Assert.Equal(GreetingService.NameTooLongError, _sut.Validate(new string('a', 51)));
    }

    [Fact]
    public void Validate_ReturnsNull_WhenValid()
    {
        Assert.Null(_sut.Validate("Ada"));
    }
}
