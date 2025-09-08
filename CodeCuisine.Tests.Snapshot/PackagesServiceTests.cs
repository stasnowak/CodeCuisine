using CodeCuisine.Brokers;
using CodeCuisine.Features;
using CodeCuisine.Options;

namespace CodeCuisine.Tests.Snapshot;

public class PackagesServiceTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public async Task ShouldGenerateDirectoryPackagesPropsFileWhenCalled(bool force, bool dryRun)
    {
        var defaultOption = new DefaultOption
        {
            Force = force,
            DryRun = dryRun,
        };
        
        await new Packages(
            new SystemBroker(),
            new ConsoleBroker()
        ).Default(defaultOption);
        
        await VerifyFile("../../../../Directory.Packages.props");
    }
}