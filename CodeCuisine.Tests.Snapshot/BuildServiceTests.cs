using CodeCuisine.Brokers;
using CodeCuisine.Features;
using CodeCuisine.Options;

namespace CodeCuisine.Tests.Snapshot;

public class BuildServiceTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public async Task ShouldGenerateDirectoryBuildPropsFileWhenCalled(bool force, bool dryRun)
    {
        var defaultOption = new DefaultOption
        {
            Force = force,
            DryRun = dryRun,
        };
        
        await new Build(
            new SystemBroker(),
            new ConsoleBroker()
        ).Default(defaultOption);

        await VerifyFile("../../../../Directory.Build.props");
    }
}