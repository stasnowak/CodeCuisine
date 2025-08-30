using CodeCuisine.Brokers;
using CodeCuisine.Services;

namespace CodeCuisine.Tests.Snapshot;

public class GitIgnoreServiceTests
{
    [Fact]
    public async Task ShouldGenerateGitIgnoreFileWhenCalled()
    {
        await new GitIgnoreService(
            new SystemBroker(),
            new ConsoleBroker()
            ).WriteAsync();
        
        await VerifyFile("../../../../.gitignore");
    }
}
