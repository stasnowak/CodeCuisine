using CodeCuisine.Brokers;
using CodeCuisine.Features;
using CodeCuisine.Options;

namespace CodeCuisine.Tests.Snapshot;

public class GitIgnoreServiceTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public async Task ShouldGenerateGitIgnoreFileWhenCalled(bool force, bool dryRun)
    {
        var defaultOption = new DefaultOption
        {
            Force = force,
            DryRun = dryRun,
        };
        
        await new Gitignore(
            new SystemBroker(),
            new ConsoleBroker()
        ).Default(defaultOption);
        
        await VerifyFile("../../../../.gitignore");
    }
}