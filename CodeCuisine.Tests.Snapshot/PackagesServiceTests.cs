using CodeCuisine.Brokers;
using CodeCuisine.Services;

namespace CodeCuisine.Tests.Snapshot;

public class PackagesServiceTests
{
    [Fact]
    public async Task ShouldGenerateDirectoryPackagesPropsFileWhenCalled()
    {
        await new PackagesService(
            new SystemBroker(),
            new ConsoleBroker()
        ).WriteAsync();
        
        await VerifyFile("../../../../Directory.Packages.props");
    }
}