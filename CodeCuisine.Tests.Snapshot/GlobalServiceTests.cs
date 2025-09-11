using CodeCuisine.Brokers;
using CodeCuisine.Features;
using CodeCuisine.Options;

namespace CodeCuisine.Tests.Snapshot;

public class GlobalServiceTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public async Task ShouldGenerateGlobalJsonFileWhenCalled(bool force, bool dryRun)
    {
        var defaultOption = new DefaultOption
        {
            Force = force,
            DryRun = dryRun,
        };

        await new Global(
            new SystemBroker(),
            new ConsoleBroker()
        ).Default(defaultOption);

        await VerifyFile("../../../../global.json");
    }
}
