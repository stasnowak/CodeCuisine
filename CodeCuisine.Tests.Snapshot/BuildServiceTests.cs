using CodeCuisine.Brokers;
using CodeCuisine.Services;

namespace CodeCuisine.Tests.Snapshot;

public class BuildServiceTests
{
    [Fact]
    public async Task ShouldGenerateDirectoryBuildPropsFileWhenCalled()
    {
        await new BuildService(
            new SystemBroker(),
            new ConsoleBroker()
        ).WriteAsync();

        await VerifyFile("../../../../Directory.Build.props");
    }
}