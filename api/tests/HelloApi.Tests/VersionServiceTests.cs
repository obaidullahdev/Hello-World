using HelloApi;

namespace HelloApi.Tests;

public class VersionServiceTests
{
    [Fact]
    public void Version_Constant_Is010()
    {
        Assert.Equal("0.1.0", VersionService.Version);
    }

    [Fact]
    public void GetVersion_ReturnsConstant()
    {
        Assert.Equal(VersionService.Version, new VersionService().GetVersion());
    }
}
