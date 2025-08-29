using CodeCuisine.Brokers;
using CodeCuisine.Gits;

namespace CodeCuisine.Tests.Snapshot;

public class GitIgnoreServiceTests
{
    [Fact]
    public async Task ShouldGenerateGitIgnoreFileWhenCalled()
    {
        var actTask = new GitIgnoreService(
            new SystemBroker(),
            new ConsoleBroker()
            ).WriteGitIgnoreAsync();
        
        await VerifyFile("../../../../.gitignore");
    }
}
